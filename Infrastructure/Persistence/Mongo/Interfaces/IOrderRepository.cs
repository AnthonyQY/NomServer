using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Infrastructure.Persistence.Mongo.Interfaces;

public interface IOrderRepository
{
    Task<OrderDocument?> GetByIdAsync(string id);
    Task<OrderDocument?> DeleteByIdAsync(string id);
    Task<OrderDocument> CreateAsync(OrderDocument orderDocument);
    Task<OrderDocument?> UpdateAsync(string id, OrderDocument orderDocument);
    Task<List<OrderDocument>> GetAllAsync();
    Task<List<OrderDocument>> GetAllByUserNameAsync(string username);
    Task<List<OrderDocument>> GetAllByStatus(int status);
    Task<List<OrderDocument>> GetAllByUserNameAndStatusAsync(string username, int status);
}