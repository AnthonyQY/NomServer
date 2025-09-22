using NomServer.Core.Entities;

namespace NomServer.Application.Interfaces;

public interface IMenuItemService
{
    Task<MenuItem?> GetByIdAsync(string id);
    Task<MenuItem?> DeleteByIdAsync(string id);
    Task<MenuItem> CreateAsync(MenuItem item);
    Task<MenuItem?> UpdateAsync(string id, MenuItem item);
    Task<List<MenuItem>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name);
    Task<MenuItem?> UpdateQuantityAsync(string id, int quantity);
}