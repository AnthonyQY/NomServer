namespace NomServer.Application.DTOs.Responses;

public class OrderResponseDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime DatePlaced { get; set; }
    public int Status { get; set; }
    public List<MenuItemResponseDto> MenuItems { get; set; }
}