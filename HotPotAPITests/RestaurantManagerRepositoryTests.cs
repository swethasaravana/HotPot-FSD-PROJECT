using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HotPotAPI.Tests
{
    public class RestaurantManagerRepositoryTests
    {
        private HotPotDbContext _context;
        private RestaurantManagerRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            var restaurant1 = new Restaurant
            {
                RestaurantId = 1,
                RestaurantName = "The Spicy Spoon",
                Location = "Chennai",
                ContactNumber = "9876543210",
                Email="thespicy@gmail.com",
                Restaurantlogo="spicyspoon.png"
            };

            var restaurant2 = new Restaurant
            {
                RestaurantId = 2,
                RestaurantName = "Hot Plate",
                Location = "Coimbatore",
                ContactNumber = "9123456780",
                Email = "hotplate@gmail.com",
                Restaurantlogo = "hotplate.png"

            };

            var manager1 = new RestaurantManager
            {
                ManagerId = 1,
                Username = "rm1",
                Password = "pass1",
                Email = "rm1@example.com",
                PhoneNumber = "9999999991",
                RestaurantId = 1,
                Restaurant = restaurant1
            };

            var manager2 = new RestaurantManager
            {
                ManagerId = 2,
                Username = "rm2",
                Password = "pass2",
                Email = "rm2@example.com",
                PhoneNumber = "9999999992",
                RestaurantId = 2,
                Restaurant = restaurant2
            };

            _context.Restaurants.AddRange(restaurant1, restaurant2);
            _context.RestaurantManagers.AddRange(manager1, manager2);
            _context.SaveChanges();

            _repository = new RestaurantManagerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnManager()
        {
            var manager = await _repository.GetById(1);
            Assert.IsNotNull(manager);
            Assert.AreEqual("rm1", manager.Username);
        }

        [Test]
        public void GetById_InvalidId_ShouldThrowException()
        {
            Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(100));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllManagers()
        {
            var managers = await _repository.GetAll();
            Assert.AreEqual(2, managers.Count());
        }

        [Test]
        public async Task GetRestaurantByManagerId_ShouldReturnRestaurant()
        {
            var restaurant = await _repository.GetRestaurantByManagerId(2);
            Assert.IsNotNull(restaurant);
            Assert.AreEqual("Hot Plate", restaurant.RestaurantName);
        }

        [Test]
        public void GetRestaurantByManagerId_InvalidManager_ShouldThrowException()
        {
            Assert.ThrowsAsync<Exception>(async () => await _repository.GetRestaurantByManagerId(999));
        }

        [Test]
        public void GetRestaurantByManagerId_NullRestaurant_ShouldThrowException()
        {
            var manager = new RestaurantManager
            {
                ManagerId = 3,
                Username = "rm3",
                Password = "pass3",
                Email = "rm3@example.com",
                PhoneNumber = "9000000003",
                RestaurantId = 999 // non-existing
            };
            _context.RestaurantManagers.Add(manager);
            _context.SaveChanges();

            Assert.ThrowsAsync<Exception>(async () => await _repository.GetRestaurantByManagerId(3));
        }
    }
}
