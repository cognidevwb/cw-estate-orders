namespace CwEstate.Orders.Models;

/// A price snapshot of a catalog product, kept here so checkout never waits on
/// the catalog service. Refreshed by ProductPriceChanged events.
public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }
}
