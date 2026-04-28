using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources
var sqlServer = builder.AddSqlServer("sqlserver");
var redis = builder.AddRedis("redis");
var rabbitmq = builder.AddRabbitMQ("rabbitmq");

// Databases
var identityDb = sqlServer.AddDatabase("IdentityDb");
var inventoryDb = sqlServer.AddDatabase("InventoryDb");
var ordersDb = sqlServer.AddDatabase("OrdersDb");
var deliveryDb = sqlServer.AddDatabase("DeliveryDb");
var tasksDb = sqlServer.AddDatabase("TasksDb");

// Identity Service
var identityApi = builder.AddProject<Projects.JGate_Identity_API>("identity-api")
    .WithReference(identityDb)
    .WaitFor(identityDb);

// Inventory Service
var inventoryApi = builder.AddProject<Projects.JGate_Inventory_API>("inventory-api")
    .WithReference(inventoryDb)
    .WithReference(redis)
    .WaitFor(inventoryDb);

// Orders Service
var ordersApi = builder.AddProject<Projects.JGate_Orders_API>("orders-api")
    .WithReference(ordersDb)
    .WithReference(rabbitmq)
    .WaitFor(ordersDb);

// Delivery Service
var deliveryApi = builder.AddProject<Projects.JGate_Delivery_API>("delivery-api")
    .WithReference(deliveryDb)
    .WithReference(rabbitmq)
    .WaitFor(deliveryDb);

// Tasks Service
var tasksApi = builder.AddProject<Projects.JGate_Tasks_API>("tasks-api")
    .WithReference(tasksDb)
    .WaitFor(tasksDb);

// API Gateway
builder.AddProject<Projects.JGate_ApiGateway>("api-gateway")
    .WithReference(identityApi)
    .WithReference(inventoryApi)
    .WithReference(ordersApi)
    .WithReference(deliveryApi)
    .WithReference(tasksApi);

builder.Build().Run();
