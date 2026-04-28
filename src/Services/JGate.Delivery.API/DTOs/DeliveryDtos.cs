namespace JGate.Delivery.API.DTOs;

public record DriverDto(Guid Id, string FirstName, string LastName, string? Email, string? Phone, string? LicenseNumber, bool IsAvailable, bool IsActive);
public record CreateDriverRequest(string FirstName, string LastName, string? Email, string? Phone, string? LicenseNumber);

public record VehicleDto(Guid Id, string PlateNumber, string Make, string Model, int Year, string Type, decimal? MaxLoadKg, bool HasTemperatureControl, bool IsAvailable, bool IsActive);
public record CreateVehicleRequest(string PlateNumber, string Make, string Model, int Year, string Type, decimal? MaxLoadKg, bool HasTemperatureControl);

public record DeliveryRouteDto(Guid Id, string RouteCode, string Name, DateTime PlannedDate, Guid? DriverId, string? DriverName, Guid? VehicleId, string? VehiclePlate, string? Notes);
public record CreateRouteRequest(string Name, DateTime PlannedDate, Guid? DriverId, Guid? VehicleId, string? Notes);

public record DeliveryStopDto(Guid Id, Guid RouteId, int StopOrder, string Address, string? ContactName, string? ContactPhone, string? Notes);
public record CreateStopRequest(Guid RouteId, int StopOrder, string Address, string? ContactName, string? ContactPhone, string? Notes);

public record DeliveryOrderDto(Guid Id, Guid? StopId, Guid OrderId, string OrderNumber, string Status, DateTime? DeliveredAt, string? ProofOfDelivery, string? FailureReason);
public record CreateDeliveryOrderRequest(Guid? StopId, Guid OrderId, string OrderNumber);
public record UpdateDeliveryStatusRequest(string Status, string? ProofOfDelivery, string? FailureReason);

public record TemperatureLogDto(Guid Id, Guid DeliveryOrderId, decimal Temperature, DateTime LoggedAt, string? Notes);
public record CreateTemperatureLogRequest(Guid DeliveryOrderId, decimal Temperature, string? Notes);
