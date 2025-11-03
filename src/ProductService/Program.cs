using AspireDemo.ServiceDefaults;
using ProductService.Data;
using ProductService.Features;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>("productsdb");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapProductEndpoints();

app.Run();
