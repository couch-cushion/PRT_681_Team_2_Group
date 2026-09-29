using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Week4Week5.Api;

/// <summary>Request payload for creating an order.</summary>
public sealed record CreateOrderRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public required string Customer { get; init; }
    [Required, EmailAddress, StringLength(254)]
    public required string Email { get; init; }
    [Required, StringLength(160, MinimumLength = 2)]
    public required string Product { get; init; }
    [Range(1, 10000)]
    public int Quantity { get; init; }
    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal UnitPrice { get; init; }
    [Required]
    public required string Status { get; init; }
}

/// <summary>Request payload for updating an order.</summary>
public sealed record UpdateOrderRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public required string Customer { get; init; }
    [Required, EmailAddress, StringLength(254)]
    public required string Email { get; init; }
    [Required, StringLength(160, MinimumLength = 2)]
    public required string Product { get; init; }
    [Range(1, 10000)]
    public int Quantity { get; init; }
    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal UnitPrice { get; init; }
    [Required]
    public required string Status { get; init; }
}

/// <summary>Order data returned by the management API.</summary>
public sealed record OrderResponse(
    Guid Id, string Customer, string Email, string Product, int Quantity,
    decimal UnitPrice, string Status, DateTimeOffset CreatedAt, bool EmailWorkflowStarted)
{
    public decimal Total => Quantity * UnitPrice;
}

public sealed class OrderStore
{
    private readonly ConcurrentDictionary<Guid, OrderResponse> _orders = new();

    public OrderStore()
    {
        AddSeed("Jordan Lee", "jordan.lee@example.test", "Enterprise license", 2, 1299m, "Pending");
        AddSeed("Morgan Chen", "morgan.chen@example.test", "Analytics suite", 1, 3480m, "Confirmed");
        AddSeed("Taylor Reed", "taylor.reed@example.test", "Team collaboration plan", 5, 680m, "Processing");
    }

    public OrderResponse[] List() => _orders.Values.OrderByDescending(order => order.CreatedAt).ToArray();
    public OrderResponse? Find(Guid id) => _orders.GetValueOrDefault(id);

    public OrderResponse Create(CreateOrderRequest request)
    {
        var order = new OrderResponse(Guid.NewGuid(), request.Customer, request.Email, request.Product,
            request.Quantity, request.UnitPrice, request.Status, DateTimeOffset.UtcNow, false);
        _orders[order.Id] = order;
        return order;
    }

    public OrderResponse? Update(Guid id, UpdateOrderRequest request)
    {
        if (!_orders.TryGetValue(id, out var existing)) return null;
        var updated = existing with
        {
            Customer = request.Customer, Email = request.Email, Product = request.Product,
            Quantity = request.Quantity, UnitPrice = request.UnitPrice, Status = request.Status,
        };
        return _orders.TryUpdate(id, updated, existing) ? updated : null;
    }

    public bool Delete(Guid id) => _orders.TryRemove(id, out _);

    private void AddSeed(string customer, string email, string product, int quantity, decimal unitPrice, string status)
    {
        var order = new OrderResponse(Guid.NewGuid(), customer, email, product, quantity, unitPrice,
            status, DateTimeOffset.UtcNow.AddMinutes(-_orders.Count * 23), false);
        _orders[order.Id] = order;
    }
}