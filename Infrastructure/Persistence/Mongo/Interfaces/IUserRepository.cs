using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Infrastructure.Persistence.Mongo.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task<UserDocument?> GetByNameAsync(string name);
    Task<UserDocument?> GetByIdAsync(string id);
    Task<List<UserDocument>> GetAllAsync();
    Task<UserDocument?> AddAsync(UserDocument user);
    Task<UserDocument?> UpdateAsync(UserDocument user);
    Task<UserDocument?> DeleteAsync(string id);
}