using AutoMapper;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Application.Services;

public class MenuItemService(IMenuItemRepository repository, IMapper mapper) : IMenuItemService
{
    public async Task<MenuItem?> GetByIdAsync(string id)
    {
        var doc = await repository.GetByIdAsync(id);
        return mapper.Map<MenuItem>(doc);
    }

    public async Task<MenuItem?> DeleteByIdAsync(string id)
    {
        var doc = await repository.DeleteAsync(id);
        return mapper.Map<MenuItem>(doc);
    }

    public async Task<MenuItem> CreateAsync(MenuItem item)
    {
        var doc = mapper.Map<MenuItemDocument>(item);
        var created = await repository.AddAsync(doc);
        return mapper.Map<MenuItem>(created);
    }

    public async Task<MenuItem?> UpdateAsync(string id, MenuItem item)
    {
        var doc = mapper.Map<MenuItemDocument>(item);
        var updated = await repository.UpdateAsync(id, doc);
        return mapper.Map<MenuItem>(updated);
    }

    public async Task<List<MenuItem>> GetAllAsync()
    {
        var docs = await repository.GetAllAsync();
        return docs.Select(mapper.Map<MenuItem>).ToList();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await repository.ExistsByNameAsync(name);
    }

    public async Task<MenuItem?> UpdateQuantityAsync(string id, int quantity)
    {
        var updatedDoc = await repository.UpdateQuantityAsync(id, quantity);
        return mapper.Map<MenuItem>(updatedDoc);
    }
}