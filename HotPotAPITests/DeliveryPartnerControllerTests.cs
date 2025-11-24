using Moq;
using NUnit.Framework;
using HotPotAPI.Controllers;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class DeliveryPartnerControllerTests
    {
        private Mock<IDeliveryPartnerService> _mockDeliveryPartnerService;
        private Mock<IOrderService> _mockOrderService;

        private DeliveryPartnerController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockDeliveryPartnerService = new Mock<IDeliveryPartnerService>();
            _mockOrderService = new Mock<IOrderService>();
            _controller = new DeliveryPartnerController(_mockDeliveryPartnerService.Object, null);
        }

        [Test]
        public async Task Register_ReturnsOk_WhenDeliveryPartnerIsRegisteredSuccessfully()
        {
            // Arrange
            var request = new CreateDeliveryPartnerRequest
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                Password = "securepassword123",
                Phone = "1234567890",
                VehicleNumber = "ABC1234",
            };

            var response = new CreateDeliveryPartnerResponse
            {
                PartnerId = 1,
            };

            _mockDeliveryPartnerService.Setup(s => s.AddDeliveryPartner(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);  // Check if the result is OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);  // Ensure the result is not null
            Assert.AreEqual(200, okResult.StatusCode);  // Ensure status code is 200 (OK)
            var returnedResponse = okResult.Value as CreateDeliveryPartnerResponse;
            Assert.IsNotNull(returnedResponse);  // Ensure the response is not null
            Assert.AreEqual(1, returnedResponse.PartnerId);  // Ensure PartnerId matches
        }

        [Test]
        public async Task UpdateDeliveryPartner_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            int partnerId = 1;
            var updateRequest = new DeliveryPartnerUpdate
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                Phone = "1234567890",
                VehicleNumber = "TN12AB1234",
                IsAvailable = true
            };

            var updatedPartner = new DeliveryPartner
            {
                DeliveryPartnerId = partnerId,
                FullName = updateRequest.FullName,
                Email = updateRequest.Email,
                Phone = updateRequest.Phone,
                VehicleNumber = updateRequest.VehicleNumber,
                IsAvailable = updateRequest.IsAvailable
            };

            _mockDeliveryPartnerService
                .Setup(service => service.UpdateDeliveryPartner(partnerId, updateRequest))
                .ReturnsAsync(updatedPartner);

            // Act
            var result = await _controller.UpdateDeliveryPartner(partnerId, updateRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(updatedPartner, okResult.Value);
        }

        [Test]
        public async Task UpdateDeliveryPartner_ReturnsNotFound_WhenPartnerNotFound()
        {
            // Arrange
            int partnerId = 99;
            var updateRequest = new DeliveryPartnerUpdate
            {
                FullName = "Name",
                Email = "email@example.com",
                Phone = "9876543210",
                VehicleNumber = "XYZ1234",
                IsAvailable = false
            };

            _mockDeliveryPartnerService
                .Setup(service => service.UpdateDeliveryPartner(partnerId, updateRequest))
                .ReturnsAsync((DeliveryPartner?)null);

            // Act
            var result = await _controller.UpdateDeliveryPartner(partnerId, updateRequest);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Delivery partner with ID {partnerId} not found", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteDeliveryPartner_ReturnsOk_WhenPartnerIsDeleted()
        {
            // Arrange
            int partnerId = 1;

            _mockDeliveryPartnerService
                .Setup(service => service.DeleteDeliveryPartner(partnerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteDeliveryPartner(partnerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Delivery partner with ID {partnerId} deleted successfully", okResult.Value);
        }

        [Test]
        public async Task DeleteDeliveryPartner_ReturnsNotFound_WhenPartnerDoesNotExist()
        {
            // Arrange
            int partnerId = 99;

            _mockDeliveryPartnerService
                .Setup(service => service.DeleteDeliveryPartner(partnerId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteDeliveryPartner(partnerId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Delivery partner with ID {partnerId} not found", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateOrderStatusByDeliveryPartner_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int orderId = 1, statusId = 2;
            _mockOrderService.Setup(s => s.GetOrderById(orderId)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateOrderStatusByDeliveryPartner(orderId, statusId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            StringAssert.Contains("Internal server error", objectResult.Value.ToString());
        }

        [Test]
        public async Task GetAllDeliveryPartners_ReturnsOk_WhenPartnersExist()
        {
            // Arrange
            var mockDeliveryPartners = new List<DeliveryPartner>
    {
        new DeliveryPartner
        {
            DeliveryPartnerId = 1,
            FullName = "John Doe",
            Email = "johndoe@example.com",
            Phone = "1234567890",
            VehicleNumber = "XYZ123",
            IsAvailable = true,
            Username = "john_doe"
        },
        new DeliveryPartner
        {
            DeliveryPartnerId = 2,
            FullName = "Jane Smith",
            Email = "janesmith@example.com",
            Phone = "9876543210",
            VehicleNumber = "ABC456",
            IsAvailable = true,
            Username = "jane_smith"
        }
    };

            // Mock the service to return the list of delivery partners
            _mockDeliveryPartnerService.Setup(s => s.GetAllDeliveryPartners())
                .ReturnsAsync(mockDeliveryPartners);

            // Act
            var result = await _controller.GetAllDeliveryPartners();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result); 
            var okResult = result.Result as OkObjectResult;  
            Assert.IsNotNull(okResult); 
            Assert.AreEqual(200, okResult.StatusCode); 
            Assert.AreEqual(mockDeliveryPartners, okResult.Value);
        }

        [Test]
        public async Task GetAllDeliveryPartners_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockDeliveryPartnerService.Setup(s => s.GetAllDeliveryPartners())
                .ThrowsAsync(new System.Exception("Database connection failed"));

            // Act
            var result = await _controller.GetAllDeliveryPartners();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);  
            var objectResult = result.Result as ObjectResult;  
            Assert.IsNotNull(objectResult); 
            Assert.AreEqual(500, objectResult.StatusCode); 
            Assert.AreEqual("Internal server error", objectResult.Value); 
        }











    }
}
