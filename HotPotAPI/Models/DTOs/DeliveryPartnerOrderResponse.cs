namespace HotPotAPI.Models.DTOs
{
    public class DeliveryPartnerOrderResponse
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
        public List<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();
        public decimal Total { get; set; }
    }

    public class OrderedItem
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }

}

