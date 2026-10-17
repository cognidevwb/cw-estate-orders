using CwEstate.Orders.Clients;
using CwEstate.Orders.Data;
using CwEstate.Orders.Messages;
using CwEstate.Orders.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Orders.Services;

public record CheckoutLine(Guid ProductId, int Quantity);
public record CheckoutRequest(string CustomerEmail, List<CheckoutLine> Lines);

public class CheckoutService(OrdersDbContext db, CatalogClient catalog, InventoryClient inventory, IPublishEndpoint publishEndpoint)
{
    private readonly OrdersDbContext _db = db;
    private readonly CatalogClient _catalog = catalog;
    private readonly InventoryClient _inventory = inventory;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task<Order> PlaceAsync(CheckoutRequest request, CancellationToken ct)
    {
        var order = new Order { Id = Guid.NewGuid(), CustomerEmail = request.CustomerEmail, PlacedAt = DateTime.UtcNow };
        foreach (var line in request.Lines)
        {
            // Prefer the local snapshot; fall back to the catalog when a product is new here.
            var snapshot = await _db.Products.FirstOrDefaultAsync(p => p.Id == line.ProductId, ct);
            if (snapshot is null)
            {
                var dto = await _catalog.GetAsync(line.ProductId, ct)
                    ?? throw new InvalidOperationException($"product {line.ProductId} is not in the catalog");
                snapshot = new Product { Id = dto.Id, Sku = dto.Sku, Name = dto.Name, Price = dto.Price, UpdatedAt = DateTime.UtcNow };
                _db.Products.Add(snapshot);
            }
            var reservation = await _inventory.ReserveAsync(new ReservationRequest(order.Id, snapshot.Sku, line.Quantity), ct);
            if (reservation is not { Reserved: true })
                throw new InvalidOperationException($"no stock for {snapshot.Sku}");
            order.Lines.Add(new OrderLine { Id = Guid.NewGuid(), OrderId = order.Id, ProductId = snapshot.Id, Sku = snapshot.Sku, Quantity = line.Quantity, UnitPrice = snapshot.Price });
        }
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        await _publishEndpoint.Publish(new OrderPlaced(order.Id, order.CustomerEmail, order.Lines.Select(l => new OrderPlacedLine(l.Sku, l.Quantity)).ToList(), order.PlacedAt), ct);
        return order;
    }
}
