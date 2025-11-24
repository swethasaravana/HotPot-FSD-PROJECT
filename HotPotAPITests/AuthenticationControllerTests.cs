using HotPotAPI.Controllers;
using HotPotAPI.Interfaces;
using HotPotAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _authServiceMock;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authServiceMock.Object);
        }

        [TestCase("Customer", "Customer login successful")]
        [TestCase("Admin", "Admin login successful")]
        [TestCase("DeliveryPartner", "Delivery Partner login successful")]
        [TestCase("RestaurantManager", "Restaurant Manager login successful")]
        public async Task Login_WithValidCredentials_ReturnsRoleSpecificSuccessMessage(string role, string expectedMessage)
        {
            // Arrange
            var loginRequest = new UserLoginRequest { Username = "testuser", Password = "password" };
            var loginResponse = new LoginResponse
            {
                Id = 1,
                Name = "Test User",
                Role = role,
                Token = "token123"
            };

            _authServiceMock.Setup(s => s.Login(It.IsAny<UserLoginRequest>())).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // Convert the anonymous object into JSON and parse
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var actualMessage = root.GetProperty("Message").GetString();
            var actualRole = root.GetProperty("Data").GetProperty("Role").GetString();

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.AreEqual(role, actualRole);
        }


        [Test]
        public async Task Login_WithInvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new UserLoginRequest { Username = "user1", Password = "pass123" };
            var loginResponse = new LoginResponse { Id = 1, Name = "user1", Role = "Unknown", Token = "token" };

            _authServiceMock
                .Setup(s => s.Login(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Invalid role.", badRequest.Value);
        }

        [Test]
        public async Task Login_WithUnauthorizedAccessException_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new UserLoginRequest { Username = "user1", Password = "wrongpass" };

            _authServiceMock
                .Setup(s => s.Login(loginRequest))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorized = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual("Invalid credentials", unauthorized.Value);
        }

        [Test]
        public async Task Login_WhenUnhandledExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var loginRequest = new UserLoginRequest { Username = "user1", Password = "pass123" };

            _authServiceMock
                .Setup(s => s.Login(loginRequest))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var errorResult = result.Result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);
            Assert.AreEqual("Something went wrong", errorResult.Value);
        }
    }
}
