using HotPotAPI.Contexts;
using HotPotAPI.Exceptions;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HotPotDbContext _context;
        private readonly IOrderRepository _orderRepo;
        private readonly IRepository<int, Cart> _cartRepo;
        private readonly IRepository<int, CartItem> _cartItemRepo;
        private readonly IDeliveryPartnerService _deliveryPartnerService;

        public OrderService(HotPotDbContext context,
                            IOrderRepository orderRepo,
                            IRepository<int, Cart> cartRepo,
                            IRepository<int, CartItem> cartItemRepo,
                            IDeliveryPartnerService deliveryPartnerService)
        {
            _context = context;
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _deliveryPartnerService = deliveryPartnerService;
        }

        public async Task<Order> PlaceOrder(int customerId, int customerAddressId,
                                    int paymentMethodId, int paymentStatusId)
        {
            var cart = await _cartRepo.GetById(customerId);
            if (cart == null) return null;

            var cartItems = cart.CartItems?.ToList() ?? new List<CartItem>();
            if (!cartItems.Any()) return null;

            // Validate that the address belongs to the customer
            var address = await _context.CustomerAddresses
                .FirstOrDefaultAsync(a => a.AddressId == customerAddressId && a.CustomerId == customerId);
            if (address == null) return null;

            decimal totalAmount = cartItems.Sum(ci => ci.Quantity * ci.MenuItem.Price);

            var newOrder = new Order
            {
                CustomerId = customerId,
                CustomerAddressId = customerAddressId,
                OrderDate = DateTime.Now,
                PaymentMethodId = paymentMethodId,
                PaymentStatusId = 2,
                OrderStatusId = 1,
                TotalAmount = totalAmount,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    MenuItemId = ci.MenuItemId,
                    Quantity = ci.Quantity,
                    Price = ci.MenuItem.Price
                }).ToList()
            };

            var placedOrder = await _orderRepo.Add(newOrder);

            foreach (var item in cartItems)
            {
                await _cartItemRepo.Delete(item.CartItemId);
            }

            return placedOrder;
        }


        // 2. Get Order by Id
        public async Task<Order> GetOrderById(int orderId)
        {
            return await _orderRepo.GetOrderById(orderId);
        }

        // 3. Get all Orders for a Customer
        public async Task<List<OrderResponseDTO>> GetOrdersByCustomer(int customerId)
        {
            var orders = await _orderRepo.GetOrdersByCustomerId(customerId); // No Include()

            var result = orders.Select(o =>
            {
                var customer = _context.Customers.Find(o.CustomerId);
                var address = _context.CustomerAddresses.Find(o.CustomerAddressId);
                var orderStatus = _context.OrderStatuses.Find(o.OrderStatusId);
                var paymentMethod = _context.PaymentMethods.Find(o.PaymentMethodId);
                var paymentStatus = _context.PaymentStatuses.Find(o.PaymentStatusId);
                var deliveryPartner = o.DeliveryPartnerId != null
                    ? _context.DeliveryPartners.Find(o.DeliveryPartnerId)
                    : null;

                return new OrderResponseDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    CustomerId = o.CustomerId,
                    CustomerName = customer?.Name,
                    CustomerAddressId = o.CustomerAddressId,
                    CustomerAddress = address != null
                        ? $"{address.Label}, {address.Street}, {address.City} - {address.Pincode}"
                        : null,
                    OrderStatus = orderStatus?.StatusName,
                    PaymentMethod = paymentMethod?.MethodName,
                    PaymentStatus = paymentStatus?.StatusName,
                    DeliveryPartnerName = deliveryPartner?.FullName,
                    DeliveryPartnerPhone = deliveryPartner?.Phone,
                    OrderItems = o.OrderItems.Select(i => new OrderItemDTO
                    {
                        MenuItemName = _context.MenuItems.Find(i.MenuItemId)?.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };
            }).ToList();

            return result;
        }



        // 4. Get Orders by Status (e.g., Delivered, Cancelled)
        public async Task<List<Order>> GetOrdersByStatus(int statusId)
        {
            return await _orderRepo.GetOrdersByStatus(statusId);
        }

        // 5. Update Order Status (Admin / Manager use)
        public async Task UpdateOrderStatus(int orderId, int statusId)
        {
            var order = await _orderRepo.GetById(orderId);
            if (order == null) return;

            order.OrderStatusId = statusId;
            await _orderRepo.Update(orderId, order);
        }

        //public async Task UpdateOrderStatusByRestaurant(int orderId, int statusId)
        //{
        //    var order = await _orderRepo.GetById(orderId);
        //    if (order == null) return;

        //    var validStatuses = new List<int> { 2, 3, 4 }; // Placed, Confirmed, Preparing, Ready for Pickup
        //    if (!validStatuses.Contains(statusId)) return;

        //    order.OrderStatusId = statusId;

        //    // Assign delivery partner only when status is "Preparing" (3)
        //    if (statusId == 3 && order.DeliveryPartnerId == null)
        //    {
        //        var availablePartner = await _context.DeliveryPartners
        //            .Where(p => p.IsAvailable)
        //            .OrderBy(p => p.DeliveryPartnerId)
        //            .FirstOrDefaultAsync();

        //        if (availablePartner != null)
        //        {
        //            order.DeliveryPartnerId = availablePartner.DeliveryPartnerId;
        //            availablePartner.IsAvailable = false;
        //            _context.DeliveryPartners.Update(availablePartner);
        //            await _context.SaveChangesAsync(); // update partner availability
        //        }
        //    }

        //    await _orderRepo.Update(orderId, order);
        //}

        public async Task UpdateOrderStatusByRestaurant(int orderId, int statusId, int restaurantManagerId)
        {
            try
            {
                // 1. Get restaurant ID
                var restaurantId = await _context.RestaurantManagers
                    .Where(m => m.ManagerId == restaurantManagerId)
                    .Select(m => m.RestaurantId)
                    .FirstOrDefaultAsync();

                if (restaurantId == 0) return;

                // 2. Verify order belongs to restaurant
                var orderBelongsToRestaurant = await _context.OrderItems
                    .AnyAsync(oi => oi.OrderId == orderId && oi.MenuItem.RestaurantId == restaurantId);

                if (!orderBelongsToRestaurant) return;

                // 3. Proceed with status update
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null) return;

                var validStatuses = new List<int> { 2, 3, 4, 7};
                if (!validStatuses.Contains(statusId)) return;

                if (statusId == 3 && order.DeliveryPartnerId == null)
                {
                    var availablePartner = await _context.DeliveryPartners
                        .Where(p => p.IsAvailable)
                        .OrderBy(p => p.DeliveryPartnerId)
                        .FirstOrDefaultAsync();

                    if (availablePartner == null)
                    {
                        // Throw the original message directly
                        throw new DeliveryPartnerUnavailableException();
                    }

                    order.DeliveryPartnerId = availablePartner.DeliveryPartnerId;
                    availablePartner.IsAvailable = false;
                }

                order.OrderStatusId = statusId;
                order.OrderDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (for example, using a logging library)
                // Log.Error(ex, "Error updating order status");

                // Rethrow the original exception
                throw ex;
            }
        }



        public async Task UpdateOrderStatusByDeliveryPartner(int orderId, int statusId)
        {
            var order = await _orderRepo.GetById(orderId);
            if (order == null) return;

            // Only allow delivery partner statuses
            var validStatuses = new List<int> { 5, 6, 7 }; // Out for Delivery, Delivered, Cancelled
            if (!validStatuses.Contains(statusId)) return;

            // Update the order status
            order.OrderStatusId = statusId;

            // If order status is Delivered or Cancelled, set IsAvailable = true for the delivery partner
            if (statusId == 6 || statusId == 7)  // Delivered or Cancelled
            {
                if (order.DeliveryPartnerId.HasValue)  // Check if DeliveryPartnerId is not null
                {
                    var deliveryPartner = await _deliveryPartnerService.GetById(order.DeliveryPartnerId.Value); // Use .Value to get the actual int
                    if (deliveryPartner != null)
                    {
                        deliveryPartner.IsAvailable = true;  // Set IsAvailable to true
                        await _deliveryPartnerService.Update(order.DeliveryPartnerId.Value, deliveryPartner);
                    }
                }
            }

            // Update the order status in the repository
            await _orderRepo.Update(orderId, order);


            await _orderRepo.SaveChangesAsync();
        }

        // 6. Get All Orders (for Admin)
        public async Task<List<Order>> GetAllOrders()
        {
            return await _orderRepo.GetAllOrders();
        }
        public async Task<Order> GetOrderDetails(int orderId)
        {
            return await _orderRepo.GetOrderById(orderId);
        }

        public async Task AssignDeliveryPartnerAsync(int orderId, int deliveryPartnerId)
        {
            var order = await _orderRepo.GetById(orderId);
            if (order == null) return;

            var deliveryPartner = await _deliveryPartnerService.GetById(deliveryPartnerId);
            if (deliveryPartner == null || !deliveryPartner.IsAvailable) return;

            order.DeliveryPartnerId = deliveryPartnerId;
            deliveryPartner.IsAvailable = false;

            await _deliveryPartnerService.Update(deliveryPartnerId, deliveryPartner);
            await _orderRepo.Update(orderId, order);
        }

        public async Task<List<DeliveryPartnerOrderResponse>> GetDeliveryPartnerOrders(int deliveryPartnerId)
        {
            var orders = await _context.Orders
                .Where(o => o.DeliveryPartnerId == deliveryPartnerId &&
                    (o.OrderStatusId == 3 || o.OrderStatusId == 4 || o.OrderStatusId == 5))
                .Include(o => o.Customer)
                .Include(o => o.CustomerAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem.Restaurant)
                .ToListAsync();

            var response = orders.Select(order => new DeliveryPartnerOrderResponse
            {
                OrderId = order.OrderId,
                RestaurantName = order.OrderItems.FirstOrDefault()?.MenuItem?.Restaurant?.RestaurantName ?? "Unknown",
                CustomerName = order.Customer.Name,
                PhoneNumber = order.Customer.Phone,
                CustomerAddress = order.CustomerAddress != null
                    ? $"{order.CustomerAddress.Street}, {order.CustomerAddress.City} - {order.CustomerAddress.Pincode}"
                    : "No Address",
                OrderedItems = order.OrderItems.Select(oi => new OrderedItem
                {
                    ItemName = oi.MenuItem?.Name ?? "Unknown Item",
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.Price
                }).ToList(),
                Total = order.OrderItems.Sum(oi => oi.Price * oi.Quantity)
            }).ToList();

            return response;
        }

        public async Task<List<Order>> GetOrdersByRestaurantManager(int restaurantManagerId)
        {
            // 1. Get the restaurant ID from manager
            var restaurantId = await _context.RestaurantManagers
                .Where(m => m.ManagerId == restaurantManagerId)
                .Select(m => m.RestaurantId)
                .FirstOrDefaultAsync();

            if (restaurantId == 0) return new List<Order>();

            // 2. Get orders that have items from this restaurant
            var orderIds = await _context.OrderItems
                .Where(oi => oi.MenuItem.RestaurantId == restaurantId)
                .Select(oi => oi.OrderId)
                .Distinct()
                .ToListAsync();

            // 3. Get full order details
            return await _context.Orders
                .Where(o => orderIds.Contains(o.OrderId))
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Customer)
                .ToListAsync();
        }

    }
}
