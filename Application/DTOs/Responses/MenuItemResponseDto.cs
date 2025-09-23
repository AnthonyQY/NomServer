namespace NomServer.Application.DTOs.Responses;

public class MenuItemResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
}