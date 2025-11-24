using HotPotAPI.Contexts;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    public class OrderServiceTests
    {
        private OrderService _orderService;
        private HotPotDbContext _context;
        private Mock<IOrderRepository> _orderRepoMock;
        private Mock<IRepository<int, Cart>> _cartRepoMock;
        private Mock<IRepository<int, CartItem>> _cartItemRepoMock;
        private Mock<IDeliveryPartnerService> _deliveryPartnerServiceMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new HotPotDbContext(options);

            _orderRepoMock = new Mock<IOrderRepository>();
            _cartRepoMock = new Mock<IRepository<int, Cart>>();
            _cartItemRepoMock = new Mock<IRepository<int, CartItem>>();
            _deliveryPartnerServiceMock = new Mock<IDeliveryPartnerService>();

            _orderService = new OrderService(_context,
                _orderRepoMock.Object,
                _cartRepoMock.Object,
                _cartItemRepoMock.Object,
                _deliveryPartnerServiceMock.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task PlaceOrder_WithValidData_ShouldReturnOrder()
        {
            // Arrange
            int customerId = 1;
            int addressId = 10;
            int menuItemId = 1001;

            var address = new CustomerAddress
            {
                AddressId = addressId,
                CustomerId = customerId,
                Label = "Home",
                Street = "No 1, Street Name",
                City = "Chennai",
                Pincode = "600001"
            };

            var customer = new Customer
            {
                Id = customerId,
                Name = "Swetha",
                Gender = "Female",
                Email = "swetha@example.com",
                Phone = "9876543210"
            };

            var menuItem = new MenuItem
            {
                Id = menuItemId,
                Name = "Paneer Butter Masala",
                Description = "Rich creamy curry",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 150,
                CuisineId = 1,
                Cuisine = new Cuisine { Id = 1, Name = "Indian" },
                MealTypeId = 1,
                MealType = new MealType { Id = 1, Name = "Dinner" },
                RestaurantId = 1,
                Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Test Restaurant", ContactNumber="6789012346", 
                Email="testrestaurant@gmail.com", Location="chennai", Restaurantlogo="image.png"},
                IsAvailable = true,
                AvailabilityTime="11:00 AM-11:PM",
                TasteInfo="spicy"
            };

            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    CartItemId = 1,
                    MenuItemId = menuItemId,
                    Quantity = 2,
                    MenuItem = menuItem
                }
            };

            var cart = new Cart
            {
                CustomerId = customerId,
                CartItems = cartItems
            };

            var orderStatus = new OrderStatus { OrderStatusId = 1, StatusName = "Placed" };
            var paymentStatus = new OrderStatus { OrderStatusId = 2, StatusName = "Paid" };
            var paymentMethod = new PaymentMethod { PaymentMethodId = 1, MethodName = "Cash on Delivery" };

            // Add required data to in-memory DB
            await _context.Customers.AddAsync(customer);
            await _context.CustomerAddresses.AddAsync(address);
            await _context.Cuisines.AddAsync(menuItem.Cuisine);
            await _context.MealTypes.AddAsync(menuItem.MealType);
            await _context.Restaurants.AddAsync(menuItem.Restaurant);
            await _context.MenuItems.AddAsync(menuItem);
            await _context.OrderStatuses.AddRangeAsync(orderStatus, paymentStatus);
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            _cartRepoMock.Setup(r => r.GetById(customerId)).ReturnsAsync(cart);
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(cartItems);
            _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            // Act
            var result = await _orderService.PlaceOrder(customerId, addressId, orderStatus.OrderStatusId, paymentMethod.PaymentMethodId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customerId, result.CustomerId);
            Assert.AreEqual(addressId, result.CustomerAddressId);
            Assert.AreEqual(300, result.TotalAmount); // 2 * 150
            Assert.AreEqual(1, result.OrderItems.Count);
        }

        [Test]
        public async Task PlaceOrder_InvalidAddress_ShouldReturnNull()
        {
            // Arrange
            int customerId = 1, addressId = 99;
            var cartItems = new List<CartItem>
        {
            new CartItem
            {
                CartItemId = 1,
                MenuItemId = 101,
                Quantity = 1,
                MenuItem = new MenuItem { Id = 101,  Name = "Paneer Tikka", Description = "Rich creamy curry",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 150,
                CuisineId = 1,
                Cuisine = new Cuisine { Id = 1, Name = "Indian" },
                MealTypeId = 1,
                MealType = new MealType { Id = 1, Name = "Dinner" },
                RestaurantId = 1,
                Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Test Restaurant", ContactNumber="6789012346",
                Email="testrestaurant@gmail.com", Location="chennai", Restaurantlogo="image.png"},
                IsAvailable = true,
                AvailabilityTime="11:00 AM-11:PM",
                TasteInfo="spicy" }
            }
        };
            var cart = new Cart { CustomerId = customerId, CartItems = cartItems };

            _cartRepoMock.Setup(r => r.GetById(customerId)).ReturnsAsync(cart);
            // Address not added to DB — invalid address

            // Act
            var result = await _orderService.PlaceOrder(customerId, addressId, 1, 1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOrderById_ValidId_ReturnsOrder()
        {
            // Arrange
            int orderId = 1;
            var sampleOrder = new Order
            {
                OrderId = orderId,
                CustomerId = 2,
                TotalAmount = 250,
                OrderDate = DateTime.Now,
                Customer = new Customer { Name = "Swetha",Gender = "Female",Email = "swetha@example.com",Phone = "9876543210"
                },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 1,
                        MenuItemId = 1,
                        Quantity = 2,
                        Price = 125,
                        MenuItem = new MenuItem 
                        { Name = "Paneer Tikka", Description = "Rich creamy curry",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 150,
                CuisineId = 1,
                Cuisine = new Cuisine { Id = 1, Name = "Indian" },
                MealTypeId = 1,
                MealType = new MealType { Id = 1, Name = "Dinner" },
                RestaurantId = 1,
                Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Test Restaurant", ContactNumber="6789012346",
                Email="testrestaurant@gmail.com", Location="chennai", Restaurantlogo="image.png"},
                IsAvailable = true,
                AvailabilityTime="11:00 AM-11:PM",
                TasteInfo="spicy"  }
                    }
                }
            };

            _orderRepoMock.Setup(repo => repo.GetOrderById(orderId)).ReturnsAsync(sampleOrder);

            // Act
            var result = await _orderService.GetOrderById(orderId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.OrderId);
            Assert.AreEqual("Swetha", result.Customer.Name);
            Assert.AreEqual(1, result.OrderItems.Count);
        }

        [Test]
        public async Task GetOrderById_InvalidId_ReturnsNull()
        {
            // Arrange
            int invalidOrderId = 999;

            _orderRepoMock.Setup(repo => repo.GetOrderById(invalidOrderId)).ReturnsAsync((Order?)null);

            // Act
            var result = await _orderService.GetOrderById(invalidOrderId);

            // Assert
            Assert.IsNull(result);
        }

       
    }
}

