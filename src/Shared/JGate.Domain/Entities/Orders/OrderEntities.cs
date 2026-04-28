using JGate.Domain.Common;
using JGate.Domain.Enums;

namespace JGate.Domain.Entities.Orders;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class PriceList : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<PriceListItem> Items { get; set; } = new List<PriceListItem>();
}

public class PriceListItem : BaseEntity
{
    public Guid PriceListId { get; set; }
    public PriceList? PriceList { get; set; }
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
}

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? RequiredDate { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
}
