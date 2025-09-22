using AutoMapper;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;
using NomServer.Core.Enums;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Application.Services;

public class OrderService(IOrderRepository repository, IMapper mapper) : IOrderService
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