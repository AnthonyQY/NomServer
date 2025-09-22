using MongoDB.Driver;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Infrastructure.Persistence.Mongo.Repositories;

public class MenuItemRepository(IMongoDatabase database) : IMenuItemRepository
{
    private readonly IMongoCollection<MenuItemDocument> _collection = database.GetCollection<MenuItemDocument>("MenuItems");

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await (await _collection.FindAsync(x => x.Name == name)).AnyAsync();
    }


    public async Task<MenuItemDocument?> GetByIdAsync(string id)
    {
        return await (await _collection.FindAsync(x => x.Id == id)).FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<MenuItemDocument>> GetAllAsync()
    {
        return await (await _collection.FindAsync(_ => true)).ToListAsync();
    }
    
    public async Task<MenuItemDocument> AddAsync(MenuItemDocument item)
    {
        await _collection.InsertOneAsync(item);
        return item;
    }

    public async Task<MenuItemDocument?> UpdateAsync(string id, MenuItemDocument item)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == id, item);
        return result.ModifiedCount > 0 ? item : null;
    }

    public async Task<MenuItemDocument?> DeleteAsync(string id)
    {
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }

    public async Task<MenuItemDocument?> UpdateQuantityAsync(string id, int quantity)
    {
        var update = Builders<MenuItemDocument>.Update.Set(x => x.Quantity, quantity);
        return await _collection.FindOneAndUpdateAsync(
            x => x.Id == id,
            update,
            new FindOneAndUpdateOptions<MenuItemDocument> { ReturnDocument = ReturnDocument.After });
    }
}