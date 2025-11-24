using HotPotAPI.Models;
using HotPotAPI.Repositories;
using HotPotAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    public class OrderRepositoryTests
    {
        private HotPotDbContext _context;
        private OrderRepository _repository;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            var restaurant = new Restaurant
            {
                RestaurantId = 1,
                RestaurantName = "FoodZone",
                Location = "City",
                ContactNumber = "9999999999",
                Email = "f@f.com",
                Restaurantlogo = "logo.png"
            };

            var customer = new Customer
            {
                Id = 1,
                Name = "John",
                Gender = "Male",
                Email = "john@mail.com",
                Phone = "1234567890"
            };

            var address = new CustomerAddress
            {
                AddressId = 1,
                CustomerId = 1,
                Label = "Home",
                Street = "123 Street",
                City = "City",
                Pincode = "123456",
                Customer = customer
            };

            var menuItem = new MenuItem
            {
                Id = 1,
                Name = "Pizza",
                Description = "Cheesy Pizza",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 200,
                AvailabilityTime = "10:00 AM - 10:00 PM",
                IsAvailable = true,
                CuisineId = 1,
                Cuisine = new Cuisine { Id = 1, Name = "Italian" },
                MealTypeId = 1,
                MealType = new MealType { Id = 1, Name = "Dinner" },
                RestaurantId = 1,
                Restaurant = restaurant,
                TasteInfo = "Spicy"
            };

            var order = new Order
            {
                OrderId = 1,
                CustomerId = 1,
                Customer = customer,
                OrderDate = DateTime.Now,
                TotalAmount = 400,
                CustomerAddressId = 1,
                CustomerAddress = address,
                OrderStatusId = 1,
                PaymentMethodId = 1,
                PaymentStatusId = 1,
                DeliveryPartnerId = null,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 1, OrderId = 1, MenuItemId = 1, Quantity = 2, Price = 200, MenuItem = menuItem }
                }
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _repository = new OrderRepository(_context);
        }
        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetOrderById_ShouldReturnOrderWithItems()
        {
            var result = await _repository.GetOrderById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.OrderId);
            Assert.IsNotEmpty(result.OrderItems);
        }

        [Test]
        public async Task GetOrdersByCustomerId_ShouldReturnOrdersForCustomer()
        {
            var result = await _repository.GetOrdersByCustomerId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].CustomerId);
        }

        [Test]
        public async Task AddOrder_ShouldAddOrder()
        {
            var newOrder = new Order
            {
                OrderId = 2,
                CustomerId = 1,
                OrderDate = DateTime.Now,
                TotalAmount = 250,
                CustomerAddressId = 1,
                OrderStatusId = 1,
                PaymentMethodId = 1,
                PaymentStatusId = 1,
                OrderItems = new List<OrderItem>()
            };

            var result = await _repository.Add(newOrder);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.OrderId);
        }

        [Test]
        public async Task UpdateOrder_ShouldUpdateSuccessfully()
        {
            var updateData = new Order
            {
                OrderStatusId = 3,
                PaymentMethodId = 2,
                PaymentStatusId = 2,
                TotalAmount = 500,
                CustomerAddressId = 1,
                OrderDate = DateTime.Now
            };

            var result = await _repository.Update(1, updateData);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.OrderStatusId);
            Assert.AreEqual(500, result.TotalAmount);
        }

        [Test]
        public async Task DeleteOrder_ShouldRemoveOrder()
        {
            var deleted = await _repository.Delete(1);
            var afterDelete = await _repository.GetById(1);

            Assert.IsNotNull(deleted);
            Assert.IsNull(afterDelete);
        }

        [Test]
        public async Task GetOrderById_InvalidId_ShouldReturnNull()
        {
            var result = await _repository.GetOrderById(999); // Non-existent ID
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOrdersByCustomerId_InvalidCustomer_ShouldReturnEmptyList()
        {
            var result = await _repository.GetOrdersByCustomerId(999); // Non-existent customer
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeleteOrder_NonExistentOrder_ShouldReturnNull()
        {
            var result = await _repository.Delete(999); // Non-existent order
            Assert.IsNull(result);
        }

    }
}
