namespace HotPotAPI.Models.DTOs
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CreateCartItemResponse> Items { get; set; } = new List<CreateCartItemResponse>();
        public decimal TotalPrice { get; set; }
    }
}
