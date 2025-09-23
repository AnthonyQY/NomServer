using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Makaretu.Dns;
using MongoDB.Driver;
using NomServer.Application.Interfaces;
using NomServer.Application.Services;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Repositories;
using NomServer.Mappings;

namespace NomServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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


        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new NullReferenceException());

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        // Configure external port
        var applicationSettings = builder.Configuration.GetSection("Application");
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
        
        // MDNS
        var mdnsSettings = builder.Configuration.GetSection("Mdns");
        var mdnsService = new MulticastService();
        var serviceDiscovery = new ServiceDiscovery(mdnsService);
        var nomnomServiceProfile = new ServiceProfile(mdnsSettings["InstanceName"], mdnsSettings["ServiceType"], ushort.Parse(mdnsSettings["Port"]!));
        
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
        app.Run();
    }
}