public class CreateOrderRequest
{
    public int CustomerId { get; set; }  // Added CustomerId
    public int CustomerAddressId { get; set; }
    public int PaymentMethodId { get; set; }
    public int PaymentStatusId { get; set; } = 2; // Adding PaymentStatusId as it's necessary
}
