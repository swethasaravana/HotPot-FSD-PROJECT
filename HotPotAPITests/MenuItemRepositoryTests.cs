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
    public class MenuItemRepositoryTests
    {
        private HotPotDbContext _context;
        private MenuItemRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            var cuisine = new Cuisine { Id = 1, Name = "Indian" };
            var mealType = new MealType { Id = 1, Name = "Lunch" };
            var restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Spicy Treat",Email="spicytreat@gmail.com", Location = "Salem", ContactNumber = "9876543210", Restaurantlogo="image.png" };

            var menuItem1 = new MenuItem
            {
                Id = 1,
                Name = "Paneer Butter Masala",
                Description = "Creamy paneer curry",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 200,
                AvailabilityTime = "12PM - 3PM",
                IsAvailable = true,
                CuisineId = 1,
                MealTypeId = 1,
                RestaurantId = 1,
                TasteInfo = "Spicy"
            };

            var menuItem2 = new MenuItem
            {
                Id = 2,
                Name = "Veg Biryani",
                Description = "Flavored rice with vegetables",
                CookingTime = TimeSpan.FromMinutes(25),
                Price = 180,
                AvailabilityTime = "12PM - 3PM",
                IsAvailable = false,
                CuisineId = 1,
                MealTypeId = 1,
                RestaurantId = 1,
                TasteInfo = "Spicy"
            };

            _context.Cuisines.Add(cuisine);
            _context.MealTypes.Add(mealType);
            _context.Restaurants.Add(restaurant);
            _context.MenuItems.AddRange(menuItem1, menuItem2);
            _context.SaveChanges();

            _repository = new MenuItemRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetAll_ShouldReturnAllMenuItems_WithRelatedData()
        {
            var items = await _repository.GetAll();
            Assert.AreEqual(2, items.Count());
            Assert.IsTrue(items.All(i => i.Cuisine != null && i.MealType != null && i.Restaurant != null));
        }

        [Test]
        public async Task GetAll_WhenNoItemsExist_ShouldReturnEmptyList()
        {
            _context.MenuItems.RemoveRange(_context.MenuItems);
            await _context.SaveChangesAsync();

            var items = await _repository.GetAll();
            Assert.IsEmpty(items);
        }

   
        [Test]
        public async Task GetById_ValidId_ShouldReturnMenuItem()
        {
            var item = await _repository.GetById(1);
            Assert.IsNotNull(item);
            Assert.AreEqual("Paneer Butter Masala", item.Name);
        }

        [Test]
        public void GetById_InvalidId_ShouldThrowException()
        {
            Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(999));
        }


        [Test]
        public async Task GetByCuisineId_Valid_ShouldReturnMatchingItems()
        {
            var items = await _repository.GetByCuisineId(1);
            Assert.AreEqual(2, items.Count());
        }

 
        [Test]
        public async Task GetByCuisineId_Invalid_ShouldReturnEmptyList()
        {
            var items = await _repository.GetByCuisineId(999);
            Assert.IsEmpty(items);
        }

    
        [Test]
        public async Task GetByMealTypeId_Valid_ShouldReturnMatchingItems()
        {
            var items = await _repository.GetByMealTypeId(1);
            Assert.AreEqual(2, items.Count());
        }

        
        [Test]
        public async Task GetByMealTypeId_Invalid_ShouldReturnEmptyList()
        {
            var items = await _repository.GetByMealTypeId(999);
            Assert.IsEmpty(items);
        }

        [Test]
        public async Task GetAvailableMenuItems_ShouldReturnOnlyAvailable()
        {
            var items = await _repository.GetAvailableMenuItems();
            Assert.AreEqual(1, items.Count());
            Assert.IsTrue(items.All(i => i.IsAvailable));
        }


        [Test]
        public async Task GetAvailableMenuItems_WhenNoneAvailable_ShouldReturnEmptyList()
        {
            foreach (var item in _context.MenuItems)
                item.IsAvailable = false;

            await _context.SaveChangesAsync();

            var items = await _repository.GetAvailableMenuItems();
            Assert.IsEmpty(items);
        }
    }
}
