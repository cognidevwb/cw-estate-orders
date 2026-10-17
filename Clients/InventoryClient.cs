using System.Net.Http.Json;

namespace CwEstate.Orders.Clients;

public record ReservationRequest(Guid OrderId, string Sku, int Quantity);
public record ReservationResult(Guid ReservationId, bool Reserved);
public record StockLevel(string Sku, int OnHand, int Reserved);

/// Typed client for cw-estate-inventory. BaseAddress comes from Services:Inventory.
public class InventoryClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task<ReservationResult?> ReserveAsync(ReservationRequest request, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/inventory/reservations", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReservationResult>(cancellationToken: ct);
    }

    public Task<StockLevel?> StockAsync(string sku, CancellationToken ct) =>
        _http.GetFromJsonAsync<StockLevel>($"/api/inventory/stock/{sku}", ct);
}
