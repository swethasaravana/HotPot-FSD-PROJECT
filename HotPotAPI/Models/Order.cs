using HotPotAPI.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign key to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Foreign key to CustomerAddress
        public int CustomerAddressId { get; set; }
        public CustomerAddress CustomerAddress { get; set; }

        // Foreign key to OrderStatus
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        // Foreign key to PaymentMethod
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        // Foreign key to OrderStatus
        public int PaymentStatusId { get; set; }
        public OrderStatus PaymentStatus { get; set; }

        public int? DeliveryPartnerId { get; set; }
        public DeliveryPartner DeliveryPartner { get; set; }


        // Navigation property for order items
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
