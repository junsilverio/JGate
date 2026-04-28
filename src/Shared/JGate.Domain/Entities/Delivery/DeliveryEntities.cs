using JGate.Domain.Common;
using JGate.Domain.Enums;

namespace JGate.Domain.Entities.Delivery;

public class Driver : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public ICollection<DeliveryRoute> Routes { get; set; } = new List<DeliveryRoute>();
}

public class Vehicle : BaseEntity
{
    public string PlateNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public VehicleType Type { get; set; }
    public decimal? MaxLoadKg { get; set; }
    public bool HasTemperatureControl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public ICollection<DeliveryRoute> Routes { get; set; } = new List<DeliveryRoute>();
}

public class DeliveryRoute : BaseEntity
{
    public string RouteCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public Guid? DriverId { get; set; }
    public Driver? Driver { get; set; }
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public string? Notes { get; set; }
    public ICollection<DeliveryStop> Stops { get; set; } = new List<DeliveryStop>();
}

public class DeliveryStop : BaseEntity
{
    public Guid RouteId { get; set; }
    public DeliveryRoute? Route { get; set; }
    public int StopOrder { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
    public ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();
}

public class DeliveryOrder : BaseEntity
{
    public Guid? StopId { get; set; }
    public DeliveryStop? Stop { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public DateTime? DeliveredAt { get; set; }
    public string? ProofOfDelivery { get; set; }
    public string? FailureReason { get; set; }
    public ICollection<TemperatureLog> TemperatureLogs { get; set; } = new List<TemperatureLog>();
}

public class TemperatureLog : BaseEntity
{
    public Guid DeliveryOrderId { get; set; }
    public DeliveryOrder? DeliveryOrder { get; set; }
    public decimal Temperature { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
