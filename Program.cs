using CwEstate.Orders.Clients;
using CwEstate.Orders.Consumers;
using CwEstate.Orders.Data;
using CwEstate.Orders.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<OrdersDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Orders")));
builder.Services.AddHttpClient<CatalogClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:Catalog"]!));
builder.Services.AddHttpClient<InventoryClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:Inventory"]!));
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductPriceChangedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost");
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.MapControllers();
app.Run();
