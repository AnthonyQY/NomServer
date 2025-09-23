namespace NomServer.Application.DTOs.Requests;

public class MenuItemRequestDto
{
    public string Name { get; set; } 
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
}