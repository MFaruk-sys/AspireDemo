using AspireDemo.ServiceDefaults;
using OrderService.Data;
using OrderService.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<OrderDbContext>("ordersdb");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOrderEndpoints();

app.Run();
