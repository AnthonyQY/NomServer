using NomServer.Core.Entities;
using NomServer.Core.Enums;

namespace NomServer.Application.Interfaces;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(string id);
    Task<Order?> DeleteByIdAsync(string id);
    Task<Order> CreateAsync(Order order);
    Task<Order?> UpdateAsync(string id, Order order);
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetAllByUserNameAsync(string username);
    Task<List<Order>> GetAllByStatusAsync(OrderEnums.OrderStatus status);
    Task<List<Order>> GetAllByUsernameAndStatusAsync(string username, OrderEnums.OrderStatus status);
}