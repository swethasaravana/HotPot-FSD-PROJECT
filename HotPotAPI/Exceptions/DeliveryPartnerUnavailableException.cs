namespace HotPotAPI.Exceptions
{
    public class DeliveryPartnerUnavailableException : Exception
    {
        public DeliveryPartnerUnavailableException()
            : base("No delivery partner is currently available. Cannot proceed to 'Out for Delivery'.")
        {
        }
    }
}
