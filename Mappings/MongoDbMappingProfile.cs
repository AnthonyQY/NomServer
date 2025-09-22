using AutoMapper;
using NomServer.Core.Entities;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Mappings;

public class MongoDbMappingProfile : Profile
{
    public MongoDbMappingProfile()
    {
        CreateMap<Order, OrderDocument>().ReverseMap();
        CreateMap<MenuItem, MenuItemDocument>().ReverseMap();
        CreateMap<User, UserDocument>().ReverseMap();
    }
}