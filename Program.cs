using System.Security.Claims;
using System.Text;
using Makaretu.Dns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NomServer.Application.Interfaces;
using NomServer.Application.Services;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Repositories;

namespace NomServer;

public class Program
{
    private const string PlaceholderKeyValue = "PlaceholderReplaceMe";
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Check for existing instances via mDNS before proceeding
        var mdnsSettings = builder.Configuration.GetSection("Mdns");
        var serviceType = mdnsSettings["ServiceType"];

        if (await CheckForExistingInstance(serviceType!))
        {
            Console.WriteLine($"Another instance of the service '{serviceType}' is already running. Terminating startup.");
            Environment.Exit(1);
            return;
        }

        // DI
        
        // DBs
        var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
        builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings!.ConnectionString));
        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings!.DatabaseName);
        });
        
        // Repositories
        // Note: MongoDb client is thread-safe for singleton
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
        builder.Services.AddSingleton<IMenuItemRepository, MenuItemRepository>();
        
        // Services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IOrderService, OrderService>();
        builder.Services.AddSingleton<IMenuItemService, MenuItemService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        
        // Automapper
        builder.Services.AddAutoMapper(_ => { }, typeof(Program));

        using var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        var logger = loggerFactory.CreateLogger<Program>();

        // Setup auth
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        if (jwtSettings["Key"] == PlaceholderKeyValue)
        {
            if (builder.Environment.IsDevelopment())
                logger.LogWarning(
                    "Placeholder key detected in development. Replace Jwt__Key in environment or appsettings.json.");
            if (builder.Environment.IsProduction())
            {
                logger.LogError(
                    "Placeholder key detected in production. Replace Jwt__Key in environment or appsettings.json.");
                throw new Exception("Startup aborted due to placeholder JWT key in production.");
            }
        }

        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new NullReferenceException());
        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RoleClaimType = ClaimTypes.Role
                };
            });

        // Setup controllers
        
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        // Configure external port
        var applicationSettings = builder.Configuration.GetSection("Application");
        if (applicationSettings["Key"] == PlaceholderKeyValue)
        {
            if (builder.Environment.IsDevelopment())
                logger.LogWarning(
                    "Placeholder key detected in development. Replace Application__Key in environment or appsettings.json.");
            if (builder.Environment.IsProduction())
            {
                logger.LogError(
                    "Placeholder key detected in production. Replace Application__KKey in environment or appsettings.json.");
                throw new Exception("Startup aborted due to placeholder JWT key in production.");
            }
        }
        var httpPort = ushort.Parse(applicationSettings["HttpPort"]!);
        var httpsPort = ushort.Parse(applicationSettings["HttpsPort"]!);
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(httpPort);
            if (builder.Environment.IsProduction())
            {
                options.ListenAnyIP(httpsPort, listenOptions =>
                {
                    listenOptions.UseHttps("C:\\certs\\kiosk.pfx", "YourPassword");
                });
            }
        });

        // Bind to concrete class
        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);
        builder.Services.AddSingleton(appSettings);

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        // MDNS - Start advertising this instance
        var mdnsService = new MulticastService();
        var serviceDiscovery = new ServiceDiscovery(mdnsService);
        var nomnomServiceProfile = new ServiceProfile(mdnsSettings["InstanceName"], serviceType, ushort.Parse(mdnsSettings["Port"]!));
        
        mdnsService.Start();
        serviceDiscovery.Advertise(nomnomServiceProfile);
        
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            serviceDiscovery.Dispose();
            mdnsService.Stop();
            mdnsService.Dispose();
        });
        
        // Finish
        await app.RunAsync();
    }

    private static async Task<bool> CheckForExistingInstance(string serviceType)
    {
        using var mdnsService = new MulticastService();
        using var serviceDiscovery = new ServiceDiscovery(mdnsService);
        
        var taskCompletionSource = new TaskCompletionSource<bool>();
        
        serviceDiscovery.ServiceInstanceDiscovered += (_, e) =>
        {
            if (e.ServiceInstanceName.ToString().Contains(serviceType))
            {
                taskCompletionSource.TrySetResult(true);
            }
        };

        try
        {
            mdnsService.Start();
            
            serviceDiscovery.QueryServiceInstances(serviceType);

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetResult(false));
            
            return await taskCompletionSource.Task;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for existing instances: {ex.Message}");
            return false;
        }
        finally
        {
            mdnsService.Stop();
        }
    }
}