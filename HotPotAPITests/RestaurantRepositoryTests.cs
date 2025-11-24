using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class RestaurantRepositoryTests
    {
        private HotPotDbContext _context;
        private RestaurantRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Seed data
            _context.Restaurants.AddRange(
                new Restaurant
                {
                    RestaurantId = 1,
                    RestaurantName = "Spice House",
                    Location = "Chennai",
                    ContactNumber = "9998887770",
                    Email = "spice@house.com",
                    Restaurantlogo = "spice.png"
                },
                new Restaurant
                {
                    RestaurantId = 2,
                    RestaurantName = "Curry Corner",
                    Location = "Coimbatore",
                    ContactNumber = "9988776655",
                    Email = "curry@corner.com",
                    Restaurantlogo = "curry.png"
                }
            );
            _context.SaveChanges();

            _repository = new RestaurantRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnAllRestaurants()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, ((List<Restaurant>)result).Count);
        }

        [Test]
        public async Task GetById_ExistingId_ShouldReturnRestaurant()
        {
            var result = await _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Spice House", result.RestaurantName);
        }

        [Test]
        public void GetById_InvalidId_ShouldThrowException()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.That(ex.Message, Is.EqualTo("Restaurant not found"));
        }

        [Test]
        public async Task GetAll_EmptyDatabase_ShouldReturnEmptyList()
        {
            // Clear data
            _context.Restaurants.RemoveRange(_context.Restaurants);
            _context.SaveChanges();

            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

    }
}
