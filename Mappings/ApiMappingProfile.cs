using AutoMapper;
using NomServer.Application.DTOs;
using NomServer.Application.DTOs.Requests;
using NomServer.Application.DTOs.Responses;
using NomServer.Core.Entities;
using NomServer.Infrastructure.Persistence.Mongo.Models;

namespace NomServer.Mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        #region Requests

        CreateMap<Order, OrderRequestDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemRequestDto>().ReverseMap();
        CreateMap<MenuItem, OrderMenuItemRequestDto>().ReverseMap();

        #endregion


        #region Responses
        
        CreateMap<Order, OrderResponseDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemResponseDto>().ReverseMap();
        
        #endregion

    }
}