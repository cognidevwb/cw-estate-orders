namespace CwEstate.Orders.Messages;

public record OrderPlacedLine(string Sku, int Quantity);

/// Raised once an order is accepted. Inventory consumes it to commit the
/// reservation.
public record OrderPlaced(Guid OrderId, string CustomerEmail, List<OrderPlacedLine> Lines, DateTime PlacedAt);
