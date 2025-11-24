using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HotPotAPI.Tests
{
    public class CartItemRepositoryTests
    {
        private HotPotDbContext _context;
        private CartItemRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Seed the database with MenuItems, CartItems, and related entities
            var restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Palace", Location="raja street",
                ContactNumber="7890123456",Email= "pizza@gmail.com",
                Restaurantlogo="image.png"};
            var cuisine = new Cuisine { Id = 1, Name = "Italian" };
            var mealType = new MealType { Id = 1, Name = "Main Course" };

            _context.Restaurants.Add(restaurant);
            _context.Cuisines.Add(cuisine);
            _context.MealTypes.Add(mealType);

            _context.MenuItems.AddRange(
                new MenuItem
                {
                    Id = 1,
                    Name = "Pizza",
                    Description = "Delicious cheese pizza",
                    CookingTime = TimeSpan.FromMinutes(15),
                    Price = 12.5m,
                    CuisineId = cuisine.Id,
                    MealTypeId = mealType.Id,
                    RestaurantId = restaurant.RestaurantId,
                    IsAvailable = true,
                    AvailabilityTime = "10:00 AM - 10:00 PM",
                    TasteInfo = "Spicy",
                    Calories = 300,
                    Proteins = 10,
                    Fats = 5,
                    Carbohydrates = 40
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Pasta",
                    Description = "Creamy pasta with sauce",
                    CookingTime = TimeSpan.FromMinutes(20),
                    Price = 12m,
                    ImagePath = "pasta.png",
                    AvailabilityTime = "10:00 AM - 11:00 PM",
                    IsAvailable = true,
                    CuisineId = cuisine.Id,
                    MealTypeId = mealType.Id,
                    RestaurantId = restaurant.RestaurantId,
                    Calories = 350,
                    Proteins = 15.0f,
                    Fats = 12.0f,
                    Carbohydrates = 45.0f,
                    TasteInfo = "Creamy and rich"
                }
            );

            _context.CartItems.AddRange(
                new CartItem
                {
                    CartItemId = 1,
                    CartId = 1,
                    MenuItemId = 1,
                    Quantity = 2,
                    PriceAtPurchase = 20.00M
                },
                new CartItem
                {
                    CartItemId = 2,
                    CartId = 1,
                    MenuItemId = 2,
                    Quantity = 3,
                    PriceAtPurchase = 15.00M
                }
            );

            _context.SaveChanges();

            _repository = new CartItemRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_WhenItemsExist_ReturnsAllCartItems()
        {
            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, result.First().Quantity); 
            Assert.AreEqual(20.00M, result.First().PriceAtPurchase); 
        }

        [Test]
        public async Task GetAll_WhenNoItemsExist_ReturnsEmptyList()
        {
            // Arrange
            _context.CartItems.RemoveRange(_context.CartItems);
            _context.SaveChanges();

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetById_ExistingId_ReturnsCartItemWithMenuItem()
        {
            // Act
            var result = await _repository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CartItemId);
            Assert.IsNotNull(result.MenuItem); 
            Assert.AreEqual("Pizza", result.MenuItem.Name);
            Assert.AreEqual(2, result.Quantity); 
            Assert.AreEqual(20.00M, result.PriceAtPurchase); 
        }

        [Test]
        public async Task GetById_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetById(999);

            // Assert
            Assert.IsNull(result);
        }
    }
}
