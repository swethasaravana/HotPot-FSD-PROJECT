using HotPotAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPotAPI.Interfaces
{
    public interface IOrderRepository : IRepository<int, Order>
    {
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrdersByCustomerId(int customerId);
        Task<List<Order>> GetOrdersByStatus(int orderStatusId);
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> GetOrdersByDeliveryPartnerId(int deliveryPartnerId);
        Task SaveChangesAsync();

    }
}
