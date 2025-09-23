using NomServer.Application.Models;

namespace NomServer.Application.Interfaces;

public interface IAuthService
{
    public Task<AuthBundle> RegisterAsync(string name, IEnumerable<string> roles);
    public Task<AuthBundle> RecoverAsync(string name, string code);
}