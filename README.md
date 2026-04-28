# JGate — Meat Manufacturing Management System

A full-stack enterprise application for managing **Inventory, Orders, Delivery, and Tasks** in a meat manufacturing company.

---

## 🏗 Technology Stack

| Layer | Technology |
|---|---|
| **Frontend** | Angular 19 (standalone components, signals), Bootstrap 5.3 |
| **Backend** | .NET 10, ASP.NET Core Web API (microservices) |
| **Orchestration** | .NET Aspire 9 (via NuGet) |
| **Database** | SQL Server (via Entity Framework Core 10) |
| **Auth** | ASP.NET Core Identity + JWT Bearer |
| **API Gateway** | YARP Reverse Proxy 2.3 |
| **Caching** | Redis (via Aspire) |
| **Messaging** | RabbitMQ (via Aspire) |
| **Observability** | OpenTelemetry (metrics, traces, logs) |

---

## 📂 Solution Structure

```
JGate/
├── JGate.AppHost/                  # .NET Aspire AppHost (orchestrator)
├── JGate.ServiceDefaults/          # Shared service defaults (OTEL, health checks)
├── src/
│   ├── Shared/
│   │   ├── JGate.Domain/           # Domain entities, enums, interfaces
│   │   └── JGate.Infrastructure/   # EF Core DbContexts, repositories
│   ├── Services/
│   │   ├── JGate.Identity.API/     # Auth service (register, login, JWT)
│   │   ├── JGate.Inventory.API/    # Inventory, stock, suppliers, warehouses
│   │   ├── JGate.Orders.API/       # Orders, customers, price lists
│   │   ├── JGate.Delivery.API/     # Routes, drivers, fleet, deliveries
│   │   └── JGate.Tasks.API/        # Task management with Kanban workflow
│   └── Gateway/
│       └── JGate.ApiGateway/       # YARP reverse proxy gateway
└── frontend/
    └── jgate-app/                  # Angular 19 SPA
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for SQL Server, Redis, RabbitMQ via Aspire)

### Run with Aspire (recommended)

```bash
# From repo root
dotnet run --project JGate.AppHost
```

This will:
- Start SQL Server, Redis, and RabbitMQ in Docker containers
- Launch all 5 backend services + API Gateway
- Open the Aspire Dashboard at `http://localhost:15001`

### Run individually (development)

```bash
# Identity API (port 5001)
dotnet run --project src/Services/JGate.Identity.API

# Inventory API (port 5002)
dotnet run --project src/Services/JGate.Inventory.API

# Orders API (port 5003)
dotnet run --project src/Services/JGate.Orders.API

# Delivery API (port 5004)
dotnet run --project src/Services/JGate.Delivery.API

# Tasks API (port 5005)
dotnet run --project src/Services/JGate.Tasks.API

# API Gateway (port 5000)
dotnet run --project src/Gateway/JGate.ApiGateway
```

### Run Angular frontend

```bash
cd frontend/jgate-app
npm install
ng serve --open
# Opens at http://localhost:4200
```

---

## 📋 API Modules

### Identity Service (`/auth`)
| Endpoint | Method | Description |
|---|---|---|
| `/auth/register` | POST | Register new user |
| `/auth/login` | POST | Login and get JWT token |
| `/auth/refresh` | POST | Refresh access token |

**Roles**: Admin, Manager, Warehouse, Driver, Supervisor

### Inventory Service (`/api`)
| Endpoint | Method | Description |
|---|---|---|
| `/api/products` | GET, POST | List/create products |
| `/api/products/{id}` | GET, PUT, DELETE | Product detail CRUD |
| `/api/categories` | GET, POST | Product categories |
| `/api/suppliers` | GET, POST | Supplier management |
| `/api/stock` | GET | Stock levels |
| `/api/stock/low` | GET | Low stock alerts |
| `/api/stock-movements` | GET, POST | Record stock movements |
| `/api/batches` | GET, POST | Lot/batch tracking |
| `/api/warehouses` | GET, POST | Warehouse/cold storage zones |

### Orders Service (`/api`)
| Endpoint | Method | Description |
|---|---|---|
| `/api/orders` | GET, POST | List/create orders |
| `/api/orders/{id}` | GET, DELETE | Order detail |
| `/api/orders/{id}/status` | PATCH | Update order status |
| `/api/customers` | GET, POST, PUT | Customer management |
| `/api/price-lists` | GET, POST | Price list management |

**Order lifecycle**: Draft → Confirmed → Processing → Ready → Delivered

### Delivery Service (`/api`)
| Endpoint | Method | Description |
|---|---|---|
| `/api/drivers` | GET, POST | Driver management |
| `/api/vehicles` | GET, POST | Fleet management |
| `/api/routes` | GET, POST | Route planning |
| `/api/deliveries` | GET, POST | Delivery orders |
| `/api/deliveries/{id}/status` | PATCH | Update delivery status |
| `/api/deliveries/temperature-logs` | POST | Cold chain logging |

**Delivery lifecycle**: Pending → Assigned → InTransit → Delivered/Failed

### Tasks Service (`/api`)
| Endpoint | Method | Description |
|---|---|---|
| `/api/tasks` | GET, POST | Task management |
| `/api/tasks/{id}` | GET | Task detail |
| `/api/tasks/{id}/status` | PATCH | Update task status |
| `/api/tasks/{id}/comments` | POST | Add comment |
| `/api/tasks/{id}/assignments` | POST | Assign user |
| `/api/task-categories` | GET, POST | Task categories |

**Task workflow**: Open → InProgress → Review → Done

---

## 🖥 Frontend Modules

- **Dashboard** — KPI cards: low stock alerts, pending orders, active deliveries, open tasks; recent activity tables
- **Inventory** — Products CRUD, stock level monitoring, supplier management, low stock alerts
- **Orders** — Order lifecycle management, customer management, status-based filtering
- **Delivery** — Kanban-style delivery board, route planning, driver/fleet management, status tracking
- **Tasks** — Kanban board with drag-to-advance, list view, priority badges, due dates

---

## 🗄 Database Schemas

| Database | Tables |
|---|---|
| JGate_Identity | AspNetUsers, AspNetRoles, AspNetUserRoles |
| JGate_Inventory | Products, Categories, Suppliers, Warehouses, Batches, StockLevels, StockMovements |
| JGate_Orders | Orders, OrderItems, Customers, PriceLists, PriceListItems |
| JGate_Delivery | Drivers, Vehicles, DeliveryRoutes, DeliveryStops, DeliveryOrders, TemperatureLogs |
| JGate_Tasks | TaskCategories, Tasks (WorkTask), TaskAssignments, TaskComments |

---

## ⚙️ Configuration

### JWT Settings (update in production!)

```json
{
  "Jwt": {
    "Key": "YOUR-SECRET-KEY-AT-LEAST-32-CHARS",
    "Issuer": "JGate",
    "Audience": "JGate"
  }
}
```

### Connection Strings (per service appsettings.json)

Each service uses its own database with `EnsureCreated()` for easy local setup:
```json
{
  "ConnectionStrings": {
    "InventoryDb": "Server=localhost;Database=JGate_Inventory;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

When running with Aspire AppHost, connection strings are automatically injected.

---

## 🔐 Security Notes

- JWT keys in `appsettings.json` are for **development only** — use environment variables or Azure Key Vault in production
- CORS is open (`AllowAnyOrigin`) for development — restrict in production
- All API endpoints (except `/auth/*`) require Bearer JWT authentication
