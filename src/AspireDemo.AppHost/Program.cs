var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var productsDb = postgres.AddDatabase("productsdb");
var ordersDb = postgres.AddDatabase("ordersdb");

builder.AddProject("ProductService", "../ProductService/ProductService.csproj")
    .WithReference(productsDb)
    .WaitFor(postgres);

builder.AddProject("OrderService", "../OrderService/OrderService.csproj")
    .WithReference(ordersDb)
    .WaitFor(postgres);

builder.Build().Run();
