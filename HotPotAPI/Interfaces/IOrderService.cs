using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;

public interface IOrderService
{
    Task<Order> PlaceOrder(int customerId, int customerAddressId, int paymentMethodId, int paymentStatusId);
    Task<Order> GetOrderById(int orderId);
    Task<List<OrderResponseDTO>> GetOrdersByCustomer(int customerId);

    Task<List<Order>> GetOrdersByStatus(int statusId);
    Task UpdateOrderStatus(int orderId, int statusId);

    //Task UpdateOrderStatusByRestaurant(int orderId, int statusId);
    Task UpdateOrderStatusByRestaurant(int orderId, int statusId, int restaurantManagerId);


    Task UpdateOrderStatusByDeliveryPartner(int orderId, int statusId);
    Task AssignDeliveryPartnerAsync(int orderId, int deliveryPartnerId);
    Task<List<Order>> GetAllOrders();
    Task<List<DeliveryPartnerOrderResponse>> GetDeliveryPartnerOrders(int deliveryPartnerId);

    Task<List<Order>> GetOrdersByRestaurantManager(int restaurantManagerId);

}
