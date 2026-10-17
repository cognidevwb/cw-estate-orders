using CwEstate.Orders.Data;
using CwEstate.Orders.Models;
using CwEstate.Orders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrdersDbContext db, CheckoutService checkout) : ControllerBase
{
    private readonly OrdersDbContext _db = db;
    private readonly CheckoutService _checkout = checkout;

    [HttpGet]
    public async Task<List<Order>> List([FromQuery] OrderStatus? status, CancellationToken ct)
    {
        var q = _db.Orders.Include(o => o.Lines).AsNoTracking();
        if (status is OrderStatus s) q = q.Where(o => o.Status == s);
        return await q.OrderByDescending(o => o.PlacedAt).ToListAsync(ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> Get(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct);
        return order is null ? NotFound() : order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Place(CheckoutRequest request, CancellationToken ct)
    {
        var order = await _checkout.PlaceAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> Pay(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null) return NotFound();
        order.Status = OrderStatus.Paid;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
