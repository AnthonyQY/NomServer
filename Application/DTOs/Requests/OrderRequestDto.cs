using System.Text.Json.Serialization;
using NomServer.Core.Entities;

namespace NomServer.Application.DTOs.Requests;

public class OrderRequestDto
{
    public List<OrderMenuItemRequestDto> MenuItems { get; set; }
}