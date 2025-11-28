public class OrderResponseDTO
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }
    public string CustomerName { get; set; }

    public int CustomerAddressId { get; set; }
    public string CustomerAddress { get; set; }

    public string OrderStatus { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }

    public string DeliveryPartnerName { get; set; }
    public string DeliveryPartnerPhone { get; set; }

    public List<OrderItemDTO> OrderItems { get; set; }
}

public class OrderItemDTO
{
    public string MenuItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
