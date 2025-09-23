using AutoMapper;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;
using NomServer.Core.Enums;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Application.Services;

public class OrderService(IOrderRepository repository, IMapper mapper, IMenuItemService menuItemService) : IOrderService
{
    public async Task<Order?> GetByIdAsync(string id)
    {
        var doc = await repository.GetByIdAsync(id);
        return mapper.Map<Order>(doc);
    }

    public async Task<Order?> DeleteByIdAsync(string id)
    {
        var doc = await repository.DeleteByIdAsync(id);
        return mapper.Map<Order>(doc);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var doc = mapper.Map<OrderDocument>(order);
        
        var totalQuantities = order.MenuItems
            .GroupBy(mi => mi.Id)
            .ToDictionary(g => g.Key, g => g.Sum(mi => mi.Quantity));
        
        var menuItems = await Task.WhenAll(order.MenuItems
            .Select(async mi => 
            {
                var itemFromDb = await menuItemService.GetByIdAsync(mi.Id);
                if (itemFromDb == null) return null;
                if (mi.Quantity == 0) throw new InvalidOperationException($"Cannot order zero quantity of {itemFromDb.Name} ({itemFromDb.Id})");
                if (!itemFromDb.IsAvailable) throw new InvalidOperationException($"Item {itemFromDb.Name} ({itemFromDb.Id}) is not available.");
                var totalRequested = totalQuantities[mi.Id];
                if (itemFromDb.Quantity < totalRequested)
                    throw new InvalidOperationException($"Item {itemFromDb.Name} ({itemFromDb.Id}) does not have enough stock.");

                itemFromDb.Quantity = mi.Quantity;
                return itemFromDb;
            }));
        
        foreach (var kvp in totalQuantities)
        {
            await menuItemService.UpdateQuantityAsync(kvp.Key, -kvp.Value);
        }

        doc.MenuItems = mapper.Map<List<MenuItemDocument>>(menuItems.Where(mi => mi != null).ToList());
        
        var createdDoc = await repository.CreateAsync(doc);
        
        return mapper.Map<Order>(createdDoc);
    }


    public async Task<Order?> UpdateAsync(string id, Order order)
    {
        var doc = mapper.Map<OrderDocument>(order);
        var updatedDoc = await repository.UpdateAsync(id, doc);
        return updatedDoc is null ? null : mapper.Map<Order>(updatedDoc);
    }

    public async Task<List<Order>> GetAllAsync()
    {
        var docs = await repository.GetAllAsync();
        return docs.Select(mapper.Map<Order>).ToList();
    }

    public async Task<List<Order>> GetAllByUserNameAsync(string userName)
    {
        var docs = await repository.GetAllByUserNameAsync(userName);
        return docs.Select(mapper.Map<Order>).ToList();
    }

    public async Task<List<Order>> GetAllByStatusAsync(OrderEnums.OrderStatus status)
    {
        var docs = await repository.GetAllByStatus((int)status);
        return docs.Select(mapper.Map<Order>).ToList();
    }

    public async Task<List<Order>> GetAllByUsernameAndStatusAsync(string userName, OrderEnums.OrderStatus status)
    {
        var docs = await repository.GetAllByUserNameAndStatusAsync(userName, (int)status);
        return docs.Select(mapper.Map<Order>).ToList();
    }
}