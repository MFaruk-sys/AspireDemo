using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using System.Net.Http.Json;

namespace OrderService.Features;

public static class Orders
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var orders = app.MapGroup("/api/orders");

        orders.MapGet("/", GetAllOrders);
        orders.MapGet("/{id:int}", GetOrderById);
        orders.MapPost("/", CreateOrder);
        orders.MapPut("/{id:int}/status", UpdateOrderStatus);
        orders.MapDelete("/{id:int}", DeleteOrder);
    }

    private static async Task<IResult> GetAllOrders(OrderDbContext db)
    {
        var orders = await db.Orders.Include(o => o.Items).ToListAsync();
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetOrderById(int id, OrderDbContext db)
    {
        var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        return order is not null ? Results.Ok(order) : Results.NotFound();
    }

    private static async Task<IResult> CreateOrder(CreateOrderRequest request, OrderDbContext db, HttpClient httpClient)
    {
        // Validate products exist by calling ProductService
        var productValidationTasks = request.Items.Select(async item =>
        {
            var response = await httpClient.GetAsync($"http://productservice/api/products/{item.ProductId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            }
            var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
            return (item.ProductId, product!.Name, product!.Price, item.Quantity);
        });

        var validatedItems = await Task.WhenAll(productValidationTasks);

        // Create order items with validated product data
        var orderItems = validatedItems.Select(item => new OrderItem
        {
            ProductId = item.ProductId,
            ProductName = item.Name,
            Quantity = item.Quantity,
            UnitPrice = item.Price,
            TotalPrice = item.Price * item.Quantity
        }).ToList();

        var totalAmount = orderItems.Sum(item => item.TotalPrice);

        var order = new Order
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Items = orderItems,
            TotalAmount = totalAmount,
            Status = "Pending"
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Results.Created($"/api/orders/{order.Id}", order);
    }

    private static async Task<IResult> UpdateOrderStatus(int id, UpdateOrderStatusRequest request, OrderDbContext db)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return Results.NotFound();

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(order);
    }

    private static async Task<IResult> DeleteOrder(int id, OrderDbContext db)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return Results.NotFound();

        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}

public record CreateOrderRequest(
    string CustomerName,
    string CustomerEmail,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(int ProductId, int Quantity);

public record UpdateOrderStatusRequest(string Status);

public record ProductResponse(int Id, string Name, decimal Price);
