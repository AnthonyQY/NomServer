using MongoDB.Driver;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Infrastructure.Persistence.Mongo.Repositories;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection = database.GetCollection<UserDocument>("Users");

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await (await _collection.FindAsync(x => x.Name == name)).AnyAsync();
    }

    public async Task<UserDocument?> GetByNameAsync(string name)
    {
        return await _collection.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task<UserDocument?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<UserDocument>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<UserDocument?> AddAsync(UserDocument user)
    {
        await _collection.InsertOneAsync(user);
        return user;
    }

    public async Task<UserDocument?> UpdateAsync(UserDocument user)
    {
        return await _collection.FindOneAndReplaceAsync(
            x => x.Id == user.Id,
            user,
            new FindOneAndReplaceOptions<UserDocument> { ReturnDocument = ReturnDocument.After }
        );
    }

    public async Task<UserDocument?> DeleteAsync(string id)
    {
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}