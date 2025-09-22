namespace NomServer.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(string name, List<string> roles);
}