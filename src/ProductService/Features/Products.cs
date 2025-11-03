using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService.Features;

public static class Products
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var products = app.MapGroup("/api/products");

        products.MapGet("/", GetAllProducts);
        products.MapGet("/{id:int}", GetProductById);
        products.MapPost("/", CreateProduct);
        products.MapPut("/{id:int}", UpdateProduct);
        products.MapDelete("/{id:int}", DeleteProduct);
    }

    private static async Task<IResult> GetAllProducts(ProductDbContext db)
    {
        var products = await db.Products.ToListAsync();
        return Results.Ok(products);
    }

    private static async Task<IResult> GetProductById(int id, ProductDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        return product is not null ? Results.Ok(product) : Results.NotFound();
    }

    private static async Task<IResult> CreateProduct(Product product, ProductDbContext db)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return Results.Created($"/api/products/{product.Id}", product);
    }

    private static async Task<IResult> UpdateProduct(int id, Product updatedProduct, ProductDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();

        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.StockQuantity = updatedProduct.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(product);
    }

    private static async Task<IResult> DeleteProduct(int id, ProductDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
