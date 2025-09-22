namespace NomServer.Core.Entities;

public class Order
{
    public string Id { get; set; }
    public string Username { get; set; } 
    public DateTime DatePlaced { get; set; }
    public int Status { get; set; }
    public List<MenuItem> MenuItems { get; set; }
}