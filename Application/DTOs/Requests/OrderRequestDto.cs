namespace NomServer.Application.DTOs.Requests;

public class OrderRequestDto
{
    public List<OrderMenuItemRequestDto> MenuItems { get; set; }
}