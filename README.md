# cw-estate-orders

Checkout and orders for the estate.

- ASP.NET Core 9 Web API, EF Core (SQLite), MassTransit over RabbitMQ.
- Owns `Orders` and `OrderLines`; keeps a `Products` price snapshot refreshed from catalog events.
- Calls `cw-estate-catalog` (product lookups) and `cw-estate-inventory` (stock reservations).
- Publishes `OrderPlaced`; consumes `ProductPriceChanged`.
- Called by `cw-estate-storefront` and `cw-estate-admin`.

Run: `dotnet run` (listens on http://localhost:5002).
