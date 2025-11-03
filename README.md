# .NET Aspire Microservices Demo

A clean architecture demo showcasing .NET Aspire with PostgreSQL, featuring two microservices that communicate via REST APIs.

## 🏗️ Architecture

```
├── src/
│   ├── AspireDemo.AppHost          # Application Host (Orchestrator)
│   ├── AspireDemo.ServiceDefaults   # Shared Service Configuration
│   ├── ProductService              # Product Management Microservice
│   │   ├── Data/                   # Entity Framework Context & Entities
│   │   └── Features/               # Minimal API Endpoints
│   └── OrderService                # Order Management Microservice
│       ├── Data/                   # Entity Framework Context & Entities
│       └── Features/               # Minimal API Endpoints
├── AspireDemo.postman_collection.json  # API Testing Collection
├── AspireDemo.sln                      # Solution File
└── README.md                          # This Documentation
```

## 🚀 Technologies

- **.NET 8.0** - Runtime framework
- **ASP.NET Core Minimal APIs** - REST endpoints
- **Entity Framework Core** - ORM with PostgreSQL
- **.NET Aspire** - Cloud-ready app development
- **OpenTelemetry** - Observability & tracing
- **PostgreSQL** - Primary database
- **PgAdmin** - Database administration

## 🗄️ Database Schema

### ProductService Database (productsdb)
```sql
Products:
- Id (int, PK)
- Name (varchar(200), not null)
- Description (varchar(1000))
- Price (decimal(18,2), not null)
- StockQuantity (int, not null)
- CreatedAt (datetime, not null)
- UpdatedAt (datetime, nullable)
```

### OrderService Database (ordersdb)
```sql
Orders:
- Id (int, PK)
- CustomerName (varchar(200), not null)
- CustomerEmail (varchar(200), not null)
- TotalAmount (decimal(18,2), not null)
- Status (varchar(50), not null)
- CreatedAt (datetime, not null)
- UpdatedAt (datetime, nullable)

OrderItems:
- Id (int, PK)
- OrderId (int, FK → Orders.Id)
- ProductId (int, not null)
- ProductName (varchar(200), not null)
- Quantity (int, not null)
- UnitPrice (decimal(18,2), not null)
- TotalPrice (decimal(18,2), not null)
```

## 🏃‍♂️ Running the Application

### Prerequisites
- .NET 8.0 SDK
- Docker (for PostgreSQL & PgAdmin)

### Steps
1. **Clone & Navigate:**
   ```bash
   cd /path/to/AspireDemo
   ```

2. **Run the Application:**
   ```bash
   cd src/AspireDemo.AppHost
   dotnet run
   ```

3. **Access Services:**
   - **Aspire Dashboard:** http://localhost:15888
   - **PgAdmin:** http://localhost:5050 (admin@admin.com / admin)
   - **ProductService:** http://localhost:PORT1
   - **OrderService:** http://localhost:PORT2

## 📡 API Endpoints

### ProductService
```
GET    /api/products           # Get all products
GET    /api/products/{id}      # Get product by ID
POST   /api/products           # Create product
PUT    /api/products/{id}      # Update product
DELETE /api/products/{id}      # Delete product
```

### OrderService
```
GET    /api/orders             # Get all orders
GET    /api/orders/{id}        # Get order by ID
POST   /api/orders             # Create order (validates products)
PUT    /api/orders/{id}/status # Update order status
DELETE /api/orders/{id}        # Delete order
```

### Health Checks
```
GET /health  # Application health
GET /alive   # Liveness probe
```

## 🔄 Service Communication

OrderService ↔ ProductService:
- **Order Creation:** Validates product existence via REST call
- **Data Flow:** Retrieves product details for order items
- **Error Handling:** Fails gracefully for invalid products

## 🧪 Testing the APIs

### Postman Collection
Import `AspireDemo.postman_collection.json` into Postman for a complete set of pre-configured requests.

The collection includes:
- **Individual API tests** for all endpoints
- **Sample workflow** demonstrating the complete order process
- **Error handling tests** (invalid product IDs)
- **Dynamic port configuration** via environment variables

### Manual Testing with curl

#### Create a Product
```bash
curl -X POST http://localhost:PORT1/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "Gaming laptop",
    "price": 1299.99,
    "stockQuantity": 10
  }'
```

#### Create an Order
```bash
curl -X POST http://localhost:PORT2/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "items": [
      {
        "productId": 1,
        "quantity": 2
      }
    ]
  }'
```

## 📊 Monitoring & Observability

- **OpenTelemetry:** Traces, metrics, and logs
- **Health Checks:** Application and dependency monitoring
- **Service Discovery:** Automatic service location
- **Resilience:** HTTP client retry policies

## 🏗️ Clean Architecture

Each microservice follows clean architecture principles:

- **Data Layer:** EF Core contexts and entities
- **API Layer:** Minimal API endpoints in Features/
- **Cross-cutting:** ServiceDefaults for shared concerns

## 🔧 Development

### Adding Migrations
```bash
# ProductService
cd src/ProductService
dotnet ef migrations add MigrationName

# OrderService
cd src/OrderService
dotnet ef migrations add MigrationName
```

### Building All Projects
```bash
dotnet build AspireDemo.sln
```

## 📝 Notes

- Databases auto-migrate on startup
- Services communicate via service discovery
- PgAdmin provides database UI access
- All services include health checks and metrics
