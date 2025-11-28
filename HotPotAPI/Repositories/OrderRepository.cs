using HotPotAPI.Models;
using HotPotAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotPotAPI.Contexts;

namespace HotPotAPI.Repositories
{
    public class OrderRepository : IRepository<int, Order>, IOrderRepository
    {
        private readonly HotPotDbContext _context;

        public OrderRepository(HotPotDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetOrdersByCustomerId(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatus(int orderStatusId)
        {
            return await _context.Orders
                .Where(o => o.OrderStatusId == orderStatusId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task<Order> GetById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order> Add(Order entity)
        {
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Order> Update(int key, Order entity)
        {
            var existingOrder = await _context.Orders.FindAsync(key);
            if (existingOrder == null)
            {
                return null;
            }

            // Update fields
            existingOrder.OrderStatusId = entity.OrderStatusId;
            existingOrder.PaymentMethodId = entity.PaymentMethodId;
            existingOrder.PaymentStatusId = entity.PaymentStatusId;
            existingOrder.TotalAmount = entity.TotalAmount;
            existingOrder.CustomerAddressId = entity.CustomerAddressId;
            existingOrder.OrderDate = entity.OrderDate;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<Order> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return null;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetOrdersByDeliveryPartnerId(int deliveryPartnerId)
        {
            return await _context.Orders
                .Where(o => o.DeliveryPartnerId == deliveryPartnerId &&
                (o.OrderStatusId == 3 || o.OrderStatusId == 4 || o.OrderStatusId == 5))
                .Include(o => o.Customer)
                .Include(o => o.CustomerAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem.Restaurant)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();  // Commit changes to the database
        }

    }
}
