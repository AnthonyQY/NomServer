namespace NomServer.Application.Models;

public class AuthBundle
{
    public string JwtToken { get; set; }
    public string RecoveryCode  { get; set; }
}