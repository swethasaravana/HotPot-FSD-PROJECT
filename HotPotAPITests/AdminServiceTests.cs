using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotPotAPI.Contexts;
using Microsoft.Extensions.Logging;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<int, Admin>> _adminRepoMock;
        private Mock<IRepository<int, Restaurant>> _restaurantRepoMock;
        private Mock<IRepository<int, Customer>> _customerRepoMock;
        private Mock<HotPotDbContext> _dbContextMock;
        private AdminService _adminService;
        private Mock<ILogger<AdminService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<string, User>>();
            _adminRepoMock = new Mock<IRepository<int, Admin>>();
            _restaurantRepoMock = new Mock<IRepository<int, Restaurant>>();
            _customerRepoMock = new Mock<IRepository<int, Customer>>();
            _mockLogger = new Mock<ILogger<AdminService>>();

            // Use real in-memory DB context
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContextMock = new Mock<HotPotDbContext>(options);
            _adminService = new AdminService(
                _dbContextMock.Object,
                _userRepoMock.Object,
                _adminRepoMock.Object,
                _restaurantRepoMock.Object,
                _customerRepoMock.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task CreateAdmin_ShouldReturnAdminResponse()
        {
            var request = new CreateAdminRequest
            {
                Name = "Admin User",
                Email = "admin@test.com",
                Password = "password123"
            };

            _userRepoMock.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(new User { Username = request.Email });
            _adminRepoMock.Setup(x => x.Add(It.IsAny<Admin>())).ReturnsAsync(new Admin { Id = 1 });

            var result = await _adminService.CreateAdmin(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async Task AddRestaurant_ShouldReturnCreatedRestaurant()
        {
            var dto = new CreateRestaurant
            {
                Name = "Food Zone",
                Address = "123 Street",
                Contact = "9999999999",
                Email = "food@zone.com",
                Restaurantlogo = "logo.png"
            };

            var restaurant = new Restaurant { RestaurantId = 1, RestaurantName = dto.Name };
            _restaurantRepoMock.Setup(x => x.Add(It.IsAny<Restaurant>())).ReturnsAsync(restaurant);

            var result = await _adminService.AddRestaurant(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.RestaurantName);
        }

        [Test]
        public async Task UpdateRestaurant_ShouldUpdateAndReturnRestaurant()
        {
            int id = 1;
            var dto = new CreateRestaurant
            {
                Name = "Updated Name",
                Address = "New Location",
                Contact = "1112223333",
                Email = "new@email.com",
                Restaurantlogo = "updatedlogo.png"
            };
            var existingRestaurant = new Restaurant { RestaurantId = id };

            _restaurantRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(existingRestaurant);
            _restaurantRepoMock.Setup(x => x.Update(id, It.IsAny<Restaurant>()))
                               .ReturnsAsync(new Restaurant { RestaurantId = id, RestaurantName = dto.Name });

            var result = await _adminService.UpdateRestaurant(id, dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.RestaurantName);
        }

        [Test]
        public async Task DeleteRestaurant_ShouldReturnTrue_WhenDeleted()
        {
            int id = 1;
            _restaurantRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(new Restaurant { RestaurantId = id });
            _restaurantRepoMock.Setup(x => x.Delete(id)).ReturnsAsync(new Restaurant { RestaurantId = id });

            var result = await _adminService.DeleteRestaurant(id);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetAllRestaurants_ShouldReturnList()
        {
            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "R1" },
                new Restaurant { RestaurantId = 2, RestaurantName = "R2" }
            };

            _restaurantRepoMock.Setup(x => x.GetAll()).ReturnsAsync(restaurants);

            var result = await _adminService.GetAllRestaurants();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetRestaurantById_ShouldReturnRestaurant()
        {
            int id = 1;
            var restaurant = new Restaurant { RestaurantId = id };
            _restaurantRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(restaurant);

            var result = await _adminService.GetRestaurantById(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.RestaurantId);
        }

        [Test]
        public async Task GetTotalRestaurants_ShouldReturnCorrectCount()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new HotPotDbContext(options))
            {
                context.Restaurants.AddRange(
                    new Restaurant
                    {
                        RestaurantName = "R1",
                        Location = "Location 1",
                        ContactNumber = "1234567890",
                        Email = "r1@example.com",
                        Restaurantlogo = "logo1.png"
                    }
                );

                await context.SaveChangesAsync();

                var adminService = new AdminService(
                    context,
                    _userRepoMock.Object,
                    _adminRepoMock.Object,
                    _restaurantRepoMock.Object,
                    _customerRepoMock.Object,
                    _mockLogger.Object
                );

                var result = await adminService.GetTotalRestaurants();

                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public async Task GetRestaurantById_ShouldThrowException_WhenNotFound()
        {
            int id = 999;
            _restaurantRepoMock.Setup(x => x.GetById(id)).ReturnsAsync((Restaurant)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _adminService.GetRestaurantById(id));
        }

        [Test]
        public async Task AddRestaurant_ShouldThrowException_WhenMissingRequiredFields()
        {
            var dto = new CreateRestaurant
            {
                Name = "",
                Address = "123 Street",
                Contact = "9999999999",
                Email = "food@zone.com",
                Restaurantlogo = "logo.png"
            };

            Assert.ThrowsAsync<ArgumentException>(async () => await _adminService.AddRestaurant(dto));
        }

        [Test]
        public async Task GetAllRestaurants_ShouldHandleEmptyList()
        {
            var restaurants = new List<Restaurant>();
            _restaurantRepoMock.Setup(x => x.GetAll()).ReturnsAsync(restaurants);

            var result = await _adminService.GetAllRestaurants();

            Assert.AreEqual(0, result.Count);
        }
    }
}
