using AutoMapper;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;
using NomServer.Infrastructure.Persistence.Mongo.Interfaces;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Application.Services;

public class UserService(IUserRepository repository, IMapper mapper) : IUserService
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var doc = await repository.GetByIdAsync(id);
        return mapper.Map<User>(doc);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        var doc = await repository.GetByNameAsync(name);
        return mapper.Map<User>(doc);
    }

    public async Task<User?> DeleteByIdAsync(string id)
    {
        return mapper.Map<User>(await repository.DeleteAsync(id));
    }

    public async Task<User?> CreateAsync(User user)
    {
        var doc = mapper.Map<UserDocument>(user);
        await repository.AddAsync(doc);
        return mapper.Map<User>(doc);
    }

    public async Task<User?> UpdateAsync(string id, User user)
    {
        var doc = mapper.Map<UserDocument>(user);
        await repository.UpdateAsync(id, doc);
        return mapper.Map<User>(doc);
    }

    public async Task<List<User>> GetAll()
    {
        var docs = await repository.GetAllAsync();
        return docs.Select(mapper.Map<User>).ToList();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await repository.ExistsByNameAsync(name);
    }
}