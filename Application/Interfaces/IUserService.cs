using NomServer.Core.Entities;

namespace NomServer.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByNameAsync(string name);
    Task<User?> DeleteByIdAsync(string id);
    Task<User?> CreateAsync(User user);
    Task<User?> UpdateAsync(string id, User user);
    Task<List<User>> GetAll();
    Task<bool> ExistsByNameAsync(string name);
}