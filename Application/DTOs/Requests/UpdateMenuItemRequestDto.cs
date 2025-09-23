namespace NomServer.Application.DTOs;

public class UpdateMenuItemRequestDto
{
    public string Name { get; set; } 
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
}