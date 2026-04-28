namespace JGate.Domain.Enums;

public enum OrderStatus
{
    Draft = 0,
    Confirmed = 1,
    Processing = 2,
    Ready = 3,
    Delivered = 4,
    Cancelled = 5
}

public enum DeliveryStatus
{
    Pending = 0,
    Assigned = 1,
    InTransit = 2,
    Delivered = 3,
    Failed = 4,
    Cancelled = 5
}

public enum TaskStatus
{
    Open = 0,
    InProgress = 1,
    Review = 2,
    Done = 3,
    Cancelled = 4
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum StockMovementType
{
    Receipt = 0,
    Adjustment = 1,
    Consumption = 2,
    Transfer = 3,
    Return = 4,
    Waste = 5
}

public enum VehicleType
{
    RefrigeratedTruck = 0,
    Van = 1,
    Motorcycle = 2,
    Other = 3
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Warehouse = 2,
    Driver = 3,
    Supervisor = 4
}
