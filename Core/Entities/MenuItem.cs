namespace NomServer.Core.Entities;

public class MenuItem
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
}