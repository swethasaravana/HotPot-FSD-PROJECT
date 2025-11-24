using HotPotAPI.Controllers;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    public class AdminControllerTests
    {
        private Mock<IAdminService> _mockAdminService;
        private Mock<IOrderService> _mockOrderService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IRestaurantManagerService> _mockRestaurantManagerService;
        private Mock<IDeliveryPartnerService> _mockDeliveryPartnerService;

        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminService>();
            _mockOrderService = new Mock<IOrderService>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockRestaurantManagerService = new Mock<IRestaurantManagerService>();
            _mockDeliveryPartnerService = new Mock<IDeliveryPartnerService>();

            _controller = new AdminController(
                _mockAdminService.Object,
                _mockOrderService.Object,
                _mockCustomerService.Object,
                _mockRestaurantManagerService.Object,
                _mockDeliveryPartnerService.Object
            );
        }

        [Test]
        public async Task CreateAdmin_ReturnsOk_WhenAdminCreated()
        {
            var request = new CreateAdminRequest { Name = "admin1",Email="admin@gmail.com", Password = "pass123" };
            var response = new CreateAdminResponse { Id = 1 };

            _mockAdminService.Setup(s => s.CreateAdmin(request)).ReturnsAsync(response);

            var result = await _controller.CreateAdmin(request);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var data = okResult.Value as CreateAdminResponse;
            Assert.AreEqual(response.Id, data.Id);
        }

        [Test]
        public async Task CreateAdmin_ReturnsBadRequest_WhenExceptionThrown()
        {
            var request = new CreateAdminRequest
            {
                Name = "admin1",
                Email = "admin@gmail.com",
                Password = "pass123"
            };

            _mockAdminService.Setup(s => s.CreateAdmin(request))
                             .ThrowsAsync(new Exception("Something went wrong"));

            var result = await _controller.CreateAdmin(request);

            var badRequestResult = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.That(badRequestResult.Value?.ToString(), Does.Contain("Something went wrong"));
        }

        [Test]
        public async Task AddRestaurant_ReturnsOk_WhenRestaurantAdded()
        {
            var request = new CreateRestaurant
            {
                Name = "Test Restaurant",
                Address = "Test Address",
                Contact = "1234567890",
                Email = "test@restaurant.com",
                Restaurantlogo = "logo.png"
            };

            var expectedRestaurant = new Restaurant
            {
                RestaurantId = 1,
                RestaurantName = "Test Restaurant",
                Location = "Test Address",
                ContactNumber = "1234567890",
                Email = "test@restaurant.com",
                Restaurantlogo = "logo.png"
            };

            _mockAdminService.Setup(s => s.AddRestaurant(request)).ReturnsAsync(expectedRestaurant);

            var result = await _controller.AddRestaurant(request);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var data = okResult.Value as Restaurant;
            Assert.IsNotNull(data);
            Assert.AreEqual(expectedRestaurant.RestaurantId, data.RestaurantId);
            Assert.AreEqual(expectedRestaurant.RestaurantName, data.RestaurantName);
        }

        [Test]
        public async Task AddRestaurant_ReturnsBadRequest_WhenExceptionThrown()
        {
            var request = new CreateRestaurant
            {
                Name = "FailRestaurant",
                Address = "FailAddress",
                Contact = "9999999999",
                Email = "fail@restaurant.com",
                Restaurantlogo = "fail.png"
            };

            _mockAdminService.Setup(s => s.AddRestaurant(request))
                             .ThrowsAsync(new Exception("Add failed"));

            var result = await _controller.AddRestaurant(request);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.That(badRequestResult.Value?.ToString(), Does.Contain("Add failed"));
        }

        [Test]
        public async Task UpdateRestaurant_ReturnsOk_WhenUpdateSuccessful()
        {
            var restaurantId = 1;
            var updateDto = new CreateRestaurant
            {
                Name = "Updated Name",
                Address = "Updated Address",
                Contact = "9876543210",
                Email = "updated@restaurant.com",
                Restaurantlogo = "updatedlogo.png"
            };

            var updatedRestaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                RestaurantName = "Updated Name",
                Location = "Updated Address",
                ContactNumber = "9876543210",
                Email = "updated@restaurant.com",
                Restaurantlogo = "updatedlogo.png"
            };

            _mockAdminService.Setup(s => s.UpdateRestaurant(restaurantId, updateDto)).ReturnsAsync(updatedRestaurant);

            var result = await _controller.UpdateRestaurant(restaurantId, updateDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var data = okResult.Value as Restaurant;
            Assert.IsNotNull(data);
            Assert.AreEqual(updatedRestaurant.RestaurantId, data.RestaurantId);
            Assert.AreEqual(updatedRestaurant.RestaurantName, data.RestaurantName);
        }

        [Test]
        public async Task UpdateRestaurant_ReturnsNotFound_WhenRestaurantNotFound()
        {
            var restaurantId = 99;
            var updateDto = new CreateRestaurant
            {
                Name = "NonExistent",
                Address = "Nowhere",
                Contact = "0000000000",
                Email = "none@restaurant.com",
                Restaurantlogo = "none.png"
            };

            _mockAdminService.Setup(s => s.UpdateRestaurant(restaurantId, updateDto)).ReturnsAsync((Restaurant)null);

            var result = await _controller.UpdateRestaurant(restaurantId, updateDto);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.That(notFoundResult.Value?.ToString(), Does.Contain("not found"));
        }

        [Test]
        public async Task UpdateRestaurant_ReturnsBadRequest_WhenExceptionThrown()
        {
            var restaurantId = 1;
            var updateDto = new CreateRestaurant
            {
                Name = "WillFail",
                Address = "ErrorZone",
                Contact = "1111111111",
                Email = "fail@restaurant.com",
                Restaurantlogo = "fail.png"
            };

            _mockAdminService.Setup(s => s.UpdateRestaurant(restaurantId, updateDto))
                             .ThrowsAsync(new Exception("Update failed"));

            var result = await _controller.UpdateRestaurant(restaurantId, updateDto);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.That(badRequestResult.Value?.ToString(), Does.Contain("Update failed"));
        }

        [Test]
        public async Task DeleteRestaurant_ReturnsNoContent_WhenDeletionSuccessful()
        {
            int restaurantId = 1;

            _mockAdminService.Setup(s => s.DeleteRestaurant(restaurantId)).ReturnsAsync(true);

            var result = await _controller.DeleteRestaurant(restaurantId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteRestaurant_ReturnsNotFound_WhenRestaurantNotFound()
        {
            int restaurantId = 999;

            _mockAdminService.Setup(s => s.DeleteRestaurant(restaurantId)).ReturnsAsync(false);

            var result = await _controller.DeleteRestaurant(restaurantId);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.That(notFoundResult.Value?.ToString(), Does.Contain("not found"));
        }

        [Test]
        public async Task DeleteRestaurant_ReturnsBadRequest_WhenExceptionThrown()
        {
            int restaurantId = 1;

            _mockAdminService.Setup(s => s.DeleteRestaurant(restaurantId))
                             .ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteRestaurant(restaurantId);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.That(badRequestResult.Value?.ToString(), Does.Contain("Delete failed"));
        }

        [Test]
        public async Task GetAllRestaurants_ReturnsOkResult_WithListOfRestaurants()
        {
            // Arrange
            var restaurants = new List<Restaurant>
    {
        new Restaurant
        {
            RestaurantId = 1,
            RestaurantName = "Spice Villa",
            Location = "Chennai",
            ContactNumber = "9876543210",
            Email = "spice@example.com",
            Restaurantlogo = "logo1.png"
        },
        new Restaurant
        {
            RestaurantId = 2,
            RestaurantName = "Tasty Treats",
            Location = "Coimbatore",
            ContactNumber = "9123456780",
            Email = "tasty@example.com",
            Restaurantlogo = "logo2.png"
        }
        };

            _mockAdminService.Setup(s => s.GetAllRestaurants()).ReturnsAsync(restaurants);

            // Act
            var result = await _controller.GetAllRestaurants();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedRestaurants = okResult.Value as List<Restaurant>;
            Assert.IsNotNull(returnedRestaurants);
            Assert.AreEqual(2, returnedRestaurants.Count);
        }

        [Test]
        public async Task GetAllRestaurants_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockAdminService.Setup(s => s.GetAllRestaurants())
                             .ThrowsAsync(new Exception("Service failure"));

            // Act
            var result = await _controller.GetAllRestaurants();

            // Assert
            var errorResult = result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
            Assert.That(errorResult.Value?.ToString(), Does.Contain("Service failure"));
        }

        [Test]
        public async Task GetRestaurantById_ReturnsOk_WhenRestaurantExists()
        {
            // Arrange
            int restaurantId = 1;
            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                RestaurantName = "Spice Villa",
                Location = "Chennai",
                ContactNumber = "9876543210",
                Email = "spice@example.com",
                Restaurantlogo = "logo.png"
            };

            _mockAdminService.Setup(s => s.GetRestaurantById(restaurantId)).ReturnsAsync(restaurant);

            // Act
            var result = await _controller.GetRestaurantById(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(restaurant, okResult.Value);
        }

        [Test]
        public async Task GetRestaurantById_ReturnsNotFound_WhenRestaurantDoesNotExist()
        {
            // Arrange
            int restaurantId = 99;

            _mockAdminService.Setup(s => s.GetRestaurantById(restaurantId)).ReturnsAsync((Restaurant)null);

            // Act
            var result = await _controller.GetRestaurantById(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Restaurant not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetRestaurantById_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int restaurantId = 1;
            _mockAdminService.Setup(s => s.GetRestaurantById(restaurantId))
                             .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetRestaurantById(restaurantId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.That(badRequestResult.Value?.ToString(), Does.Contain("Database error"));
        }

        [Test]
        public async Task GetDashboardStats_ReturnsOk_WithValidStats()
        {
            // Arrange
            _mockCustomerService.Setup(s => s.GetTotalCustomers()).ReturnsAsync(100);
            _mockAdminService.Setup(s => s.GetTotalRestaurants()).ReturnsAsync(20);
            _mockRestaurantManagerService.Setup(s => s.GetTotalManagers()).ReturnsAsync(15);
            _mockDeliveryPartnerService.Setup(s => s.GetTotalDeliveryPartners()).ReturnsAsync(25);

            // Act
            var result = await _controller.GetDashboardStats();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var stats = okResult.Value;

            // Use reflection to extract anonymous properties
            var type = stats.GetType();
            Assert.AreEqual(100, type.GetProperty("totalCustomers")?.GetValue(stats));
            Assert.AreEqual(20, type.GetProperty("totalRestaurants")?.GetValue(stats));
            Assert.AreEqual(15, type.GetProperty("totalRestaurantManagers")?.GetValue(stats));
            Assert.AreEqual(25, type.GetProperty("totalDeliveryPartners")?.GetValue(stats));
        }


        [Test]
        public async Task GetDashboardStats_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockCustomerService.Setup(s => s.GetTotalCustomers())
                                .ThrowsAsync(new Exception("Service failed"));

            // Act
            var result = await _controller.GetDashboardStats();

            // Assert
            var errorResult = result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
            Assert.That(errorResult.Value.ToString(), Does.Contain("Service failed"));
        }















    }
}
