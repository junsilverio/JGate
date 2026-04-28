namespace JGate.Orders.API.DTOs;

public record CustomerDto(Guid Id, string Name, string? ContactPerson, string? Email, string? Phone, string? Address, string? TaxId, bool IsActive);
public record CreateCustomerRequest(string Name, string? ContactPerson, string? Email, string? Phone, string? Address, string? TaxId);
public record UpdateCustomerRequest(string Name, string? ContactPerson, string? Email, string? Phone, string? Address, string? TaxId, bool IsActive);

public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, decimal Quantity, string Unit, decimal UnitPrice, decimal Discount, decimal LineTotal);
public record CreateOrderItemRequest(Guid ProductId, string ProductName, decimal Quantity, string Unit, decimal UnitPrice, decimal Discount);

public record OrderDto(Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, string Status, DateTime OrderDate, DateTime? RequiredDate, string? ShippingAddress, string? Notes, decimal SubTotal, decimal TaxAmount, decimal TotalAmount, IEnumerable<OrderItemDto> Items);
public record CreateOrderRequest(Guid CustomerId, DateTime? RequiredDate, string? ShippingAddress, string? Notes, IEnumerable<CreateOrderItemRequest> Items);
public record UpdateOrderStatusRequest(string Status);

public record PriceListDto(Guid Id, string Name, DateTime ValidFrom, DateTime? ValidTo, bool IsActive);
public record CreatePriceListRequest(string Name, DateTime ValidFrom, DateTime? ValidTo);
