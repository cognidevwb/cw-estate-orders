namespace CwEstate.Orders.Messages;

/// The catalog service's event, mirrored here (same name, same shape) so the
/// consumer binds to it without a shared assembly.
public record ProductPriceChanged(Guid ProductId, string Sku, decimal OldPrice, decimal NewPrice, DateTime ChangedAt);
