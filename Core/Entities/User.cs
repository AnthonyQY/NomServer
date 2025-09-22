namespace NomServer.Core.Entities;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public List<string> Roles {get; set;}
    public string RecoveryCodeHash { get; set; }
    public string RecoveryCodeSalt { get; set; }
    public bool IsActive { get; set; }
}