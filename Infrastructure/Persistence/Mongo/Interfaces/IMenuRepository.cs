using NomServer.Infrastructure.Persistence.Mongo.Models;

public interface IMenuItemRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task<MenuItemDocument?> GetByIdAsync(string id);
    Task<IEnumerable<MenuItemDocument>> GetAllAsync();
    Task<MenuItemDocument> AddAsync(MenuItemDocument item);
    Task<MenuItemDocument?> UpdateAsync(string id, MenuItemDocument item);
    Task<MenuItemDocument?> DeleteAsync(string id);
    Task<MenuItemDocument?> UpdateQuantityAsync(string id, int quantity);
}