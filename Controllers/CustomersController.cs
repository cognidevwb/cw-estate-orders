using CwEstate.Orders.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CwEstate.Orders.Controllers;

public record CustomerSummary(string Email, int Orders, decimal Spent);

[ApiController]
[Route("api/[controller]")]
public class CustomersController(OrdersDbContext db) : ControllerBase
{
    private readonly OrdersDbContext _db = db;

    /// The signed-in customer, as the storefront's account page shows it.
    [HttpGet("me")]
    public async Task<ActionResult<CustomerSummary>> Me([FromHeader(Name = "X-Customer-Email")] string email, CancellationToken ct)
    {
        var orders = await _db.Orders.Include(o => o.Lines).Where(o => o.CustomerEmail == email).ToListAsync(ct);
        return new CustomerSummary(email, orders.Count, orders.Sum(o => o.Total));
    }
}
