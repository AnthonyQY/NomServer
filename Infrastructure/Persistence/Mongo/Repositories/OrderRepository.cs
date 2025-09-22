using MongoDB.Driver;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Infrastructure.Persistence.Mongo.Repositories;

public class OrderRepository(AppSettings appSettings, IMongoDatabase database) : IOrderRepository
{
    private readonly IMongoCollection<OrderDocument> _collection = database.GetCollection<OrderDocument>(appSettings.MongoDbSettings.OrderCollectionName);

    public async Task<OrderDocument?> GetByIdAsync(string id)
    {
        return await _collection.Find(od => od.Id == id).FirstOrDefaultAsync();
    }

    public async Task<OrderDocument?> DeleteByIdAsync(string id)
    {
        return await _collection.FindOneAndDeleteAsync(od => od.Id == id);
    }

    public async Task<OrderDocument> CreateAsync(OrderDocument orderDocument)
    {
        await _collection.InsertOneAsync(orderDocument);
        return orderDocument;
    }

    public async Task<OrderDocument?> UpdateAsync(string id, OrderDocument orderDocument)
    {
        orderDocument.Id = id;
        return await _collection.FindOneAndReplaceAsync(
            od => od.Id == id, 
            orderDocument, 
            new FindOneAndReplaceOptions<OrderDocument> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<List<OrderDocument>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<List<OrderDocument>> GetAllByUserNameAsync(string username)
    {
        return await _collection.Find(od => od.Username == username).ToListAsync();
    }

    public async Task<List<OrderDocument>> GetAllByStatus(int status)
    {
        return await _collection.Find(od => od.Status == status).ToListAsync();
    }

    public async Task<List<OrderDocument>> GetAllByUserNameAndStatusAsync(string username, int status)
    {
        return await _collection.Find(od => od.Username == username && od.Status == status).ToListAsync();
    }
}