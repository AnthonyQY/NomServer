namespace NomServer;


public class AppSettings
{
    public LoggingSettings Logging { get; set; } = null!;
    public ApplicationSettings Application { get; set; } = null!;
    public JwtSettings Jwt { get; set; } = null!;
    public MdnsSettings Mdns { get; set; } = null!;
    public MongoDbSettings MongoDbSettings { get; set; } = null!;
}

public class LoggingSettings
{
    public LogLevelSettings LogLevel { get; set; } = null!;
}

public class LogLevelSettings
{
    public string Default { get; set; } = null!;
    public string MicrosoftAspNetCore { get; set; } = null!;
}

public class ApplicationSettings
{
    public int HttpPort { get; set; }
    public int HttpsPort { get; set; }
}

public class JwtSettings
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}

public class MdnsSettings
{
    public string InstanceName { get; set; } = null!;
    public string ServiceType { get; set; } = null!;
    public int Port { get; set; }
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
    public string MenuCollectionName { get; set; } = null!;
}