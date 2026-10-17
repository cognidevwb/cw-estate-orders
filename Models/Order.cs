namespace CwEstate.Orders.Models;

public enum OrderStatus { Pending, Paid, Shipped, Cancelled }

public class Order
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime PlacedAt { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
    public decimal Total => Lines.Sum(l => l.UnitPrice * l.Quantity);
}
