using CwEstate.Orders.Data;
using CwEstate.Orders.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Orders.Consumers;

/// Keeps the local Products price snapshot in step with the catalog.
public class ProductPriceChangedConsumer(OrdersDbContext db) : IConsumer<ProductPriceChanged>
{
    private readonly OrdersDbContext _db = db;

    public async Task Consume(ConsumeContext<ProductPriceChanged> context)
    {
        var msg = context.Message;
        var snapshot = await _db.Products.FirstOrDefaultAsync(p => p.Id == msg.ProductId, context.CancellationToken);
        if (snapshot is null)
        {
            _db.Products.Add(new Models.Product { Id = msg.ProductId, Sku = msg.Sku, Price = msg.NewPrice, UpdatedAt = msg.ChangedAt });
        }
        else
        {
            snapshot.Price = msg.NewPrice;
            snapshot.UpdatedAt = msg.ChangedAt;
            _db.Products.Update(snapshot);
        }
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
