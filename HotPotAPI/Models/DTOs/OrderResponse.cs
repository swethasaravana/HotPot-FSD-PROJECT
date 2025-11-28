public class OrderResponse
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string OrderStatus { get; set; }
    public string DeliveryStatus { get; set; }
    public DateTime OrderPlacedAt { get; set; }
    public DateTime OrderUpdatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
}

public class OrderItemResponse
{
    public string MenuItemName { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
}
