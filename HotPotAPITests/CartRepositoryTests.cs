using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class CartRepositoryTests
    {
        private HotPotDbContext _context;
        private CartRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Seed MenuItem
            var menuItem = new MenuItem
            {
                Name = "Burger",
                Description = "Tasty burger",
                CookingTime = TimeSpan.FromMinutes(10),
                Price = 5.99m,
                ImagePath = "/images/burger.jpg",
                AvailabilityTime = "10:00 AM - 10:00 PM",
                TasteInfo = "Spicy",
                IsAvailable = true,
                Calories = 300,
                Proteins = 15,
                Fats = 10,
                Carbohydrates = 30,
                CuisineId = 1,
                MealTypeId = 1,
                RestaurantId = 1
            };
            _context.MenuItems.Add(menuItem);
            _context.SaveChanges(); // Save to get MenuItemId

            // Create CartItem
            var cartItem = new CartItem
            {
                MenuItemId = menuItem.Id,
                Quantity = 2,
                PriceAtPurchase = 5.99m,
                MenuItem = menuItem
            };

            // Create Cart
            var cart = new Cart
            {
                CustomerId = 101,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem> { cartItem }
            };

            _context.Carts.Add(cart);
            _context.SaveChanges(); // Save to generate CartId and CartItemId

            _repository = new CartRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_WhenCartsExist_ReturnsAllCartsWithItems()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            var cartList = result.ToList();
            Assert.AreEqual(1, cartList.Count);
            Assert.AreEqual(1, cartList[0].CartItems.Count);
            Assert.AreEqual("Burger", cartList[0].CartItems.First().MenuItem.Name);
            Assert.AreEqual(5.99m, cartList[0].CartItems.First().PriceAtPurchase);
        }

        [Test]
        public async Task GetById_ExistingCartId_ReturnsCartWithItems()
        {
            var existingCart = _context.Carts.First();
            var result = await _repository.GetById(existingCart.CartId);

            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.CustomerId);
            Assert.AreEqual(1, result.CartItems.Count);
            Assert.AreEqual("Burger", result.CartItems.First().MenuItem.Name);
        }

        [Test]
        public async Task GetById_NonExistingCartId_ReturnsNull()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCartByUserId_ExistingCustomerId_ReturnsCartWithItems()
        {
            var result = await _repository.GetCartByUserId(101);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CartItems.Count);
            Assert.AreEqual("Burger", result.CartItems.First().MenuItem.Name);
        }

        [Test]
        public async Task GetCartByUserId_NonExistingCustomerId_ReturnsNull()
        {
            var result = await _repository.GetCartByUserId(999);

            Assert.IsNull(result);
        }
    }
}
