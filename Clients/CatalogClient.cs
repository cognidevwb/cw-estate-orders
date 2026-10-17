using System.Net.Http.Json;

namespace CwEstate.Orders.Clients;

public record ProductDto(Guid Id, string Sku, string Name, decimal Price);

/// Typed client for cw-estate-catalog. BaseAddress comes from Services:Catalog.
public class CatalogClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public Task<List<ProductDto>?> ListAsync(CancellationToken ct) =>
        _http.GetFromJsonAsync<List<ProductDto>>("/api/products", ct);

    public Task<ProductDto?> GetAsync(Guid productId, CancellationToken ct) =>
        _http.GetFromJsonAsync<ProductDto>($"/api/products/{productId}", ct);

    public async Task<ProductDto?> GetBySkuAsync(string sku, CancellationToken ct)
    {
        var response = await _http.GetAsync("/api/products/by-sku/" + sku, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
    }
}
