using HotPotAPI.Controllers;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<IOrderService> _mockOrderService;
        private OrdersController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockOrderService.Object);
        }

        [Test]
        public async Task PlaceOrder_ReturnsOk_WhenOrderIsPlaced()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = 1,
                CustomerAddressId = 2,
                PaymentMethodId = 1,
                PaymentStatusId = 1
            };

            var expectedOrder = new Order(); 

            _mockOrderService
                .Setup(s => s.PlaceOrder(request.CustomerId, request.CustomerAddressId, request.PaymentMethodId, request.PaymentStatusId))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.PlaceOrder(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedOrder, okResult.Value);
        }

        [Test]
        public async Task PlaceOrder_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Arrange
            CreateOrderRequest request = null;

            // Act
            var result = await _controller.PlaceOrder(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid order data", badRequestResult.Value);
        }

        [Test]
        public async Task PlaceOrder_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = 1,
                CustomerAddressId = 2,
                PaymentMethodId = 3,
                PaymentStatusId = 1
            };

            _mockOrderService
                .Setup(s => s.PlaceOrder(request.CustomerId, request.CustomerAddressId, request.PaymentMethodId, request.PaymentStatusId))
                .ReturnsAsync((Order)null); // service returns null

            // Act
            var result = await _controller.PlaceOrder(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Unable to place order", badRequestResult.Value);
        }

        [Test]
        public async Task GetOrderById_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            var mockOrder = new Order { OrderId = orderId, CustomerId = 10 };

            _mockOrderService.Setup(s => s.GetOrderById(orderId))
                             .ReturnsAsync(mockOrder);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(mockOrder, okResult.Value);
        }

        [Test]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;

            _mockOrderService.Setup(s => s.GetOrderById(orderId))
                             .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Order not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetOrderById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int orderId = 1;
            _mockOrderService.Setup(s => s.GetOrderById(orderId))
                             .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value.ToString().Contains("Database failure"));
        }

        [Test]
        public async Task GetOrdersByCustomer_ReturnsOk_WhenOrdersExist()
        {
            // Arrange
            int customerId = 1;
            var mockOrders = new List<OrderResponseDTO>
            {
                new OrderResponseDTO { OrderId = 101, CustomerId = customerId },
                new OrderResponseDTO { OrderId = 102, CustomerId = customerId }
            };

            _mockOrderService.Setup(s => s.GetOrdersByCustomer(customerId))
                             .ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(mockOrders, okResult.Value);
        }

        [Test]
        public async Task GetOrdersByCustomer_ReturnsNotFound_WhenNoOrdersExist()
        {
            // Arrange
            int customerId = 2;

            _mockOrderService.Setup(s => s.GetOrdersByCustomer(customerId))
                             .ReturnsAsync(new List<OrderResponseDTO>());

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No orders found for this customer.", notFoundResult.Value);
        }

        [Test]
        public async Task GetOrdersByCustomer_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int customerId = 3;

            _mockOrderService.Setup(s => s.GetOrdersByCustomer(customerId))
                             .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetOrdersByCustomer(customerId);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value.ToString().Contains("Service error"));
        }

        [Test]
        public async Task GetOrdersByStatus_ReturnsOk_WhenOrdersExist()
        {
            // Arrange
            int statusId = 1;
            var mockOrders = new List<Order>
            {
                new Order
                {
                    OrderId = 101,
                    CustomerId = 1,
                    Customer = new Customer { Id = 1, Name = "John" },
                    CustomerAddressId = 1,
                    CustomerAddress = new CustomerAddress
                    {
                        AddressId = 1,
                        CustomerId = 1,
                        Label = "Home",
                        Street = "123 Main St",
                        City = "Cityville",
                        Pincode = "123456"
                    },
                    OrderDate = DateTime.Now,
                    TotalAmount = 150.00m,
                    OrderStatusId = statusId,
                    OrderStatus = new OrderStatus { OrderStatusId = statusId, StatusName = "Pending" },
                    PaymentMethodId = 1,
                    PaymentMethod = new PaymentMethod { PaymentMethodId = 1, MethodName = "Card" },
                    PaymentStatusId = 2,
                    PaymentStatus = new OrderStatus { OrderStatusId = 2, StatusName = "Paid" },
                    OrderItems = new List<OrderItem>
                    {
                    new OrderItem
                    {
                    OrderItemId = 1,
                    OrderId = 101,
                    MenuItemId = 1,
                    Quantity = 2,
                    Price = 12.5m,
                    MenuItem = new MenuItem
                    {
                        Id = 1,
                        Name = "Pizza",
                        Description = "Delicious cheese pizza",
                        CookingTime = TimeSpan.FromMinutes(15),
                        Price = 12.5m,
                        CuisineId = 1,
                        MealTypeId = 1,
                        RestaurantId = 1,
                        IsAvailable = true,
                        AvailabilityTime = "10:00 AM - 10:00 PM",
                        TasteInfo = "Spicy",
                        Calories = 300,
                        Proteins = 10,
                        Fats = 5,
                        Carbohydrates = 40
                    }
                }
            }
        }
    };

            _mockOrderService.Setup(s => s.GetOrdersByStatus(statusId)).ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetOrdersByStatus(statusId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedOrders = okResult.Value as List<Order>;

            Assert.NotNull(returnedOrders);
            Assert.AreEqual(1, returnedOrders.Count);
            Assert.AreEqual("John", returnedOrders[0].Customer.Name);
            Assert.AreEqual("Pizza", returnedOrders[0].OrderItems.First().MenuItem.Name);
        }

        [Test]
        public async Task GetOrdersByStatus_ReturnsNotFound_WhenNoOrdersExist()
        {
            // Arrange
            int statusId = 999; 
            var emptyOrderList = new List<Order>();

            _mockOrderService.Setup(s => s.GetOrdersByStatus(statusId)).ReturnsAsync(emptyOrderList);

            // Act
            var result = await _controller.GetOrdersByStatus(statusId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("No orders found with the specified status.", notFoundResult.Value);
        }

        [Test]
        public async Task GetOrdersByStatus_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int statusId = 1;
            string errorMessage = "Database connection failed";

            _mockOrderService.Setup(s => s.GetOrdersByStatus(statusId))
                             .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.GetOrdersByStatus(statusId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;

            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.That(objectResult.Value.ToString(), Does.Contain(errorMessage));
        }

        [Test]
        public async Task UpdateOrderStatus_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            int orderId = 101;
            int statusId = 2;
            var order = new Order
            {
                OrderId = orderId,
                CustomerId = 1,
                Customer = new Customer { Id = 1, Name = "John" },
                OrderStatusId = 1,
                OrderStatus = new OrderStatus { OrderStatusId = 1, StatusName = "Pending" }
            };

            _mockOrderService.Setup(s => s.GetOrderById(orderId)).ReturnsAsync(order);
            _mockOrderService.Setup(s => s.UpdateOrderStatus(orderId, statusId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, statusId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Order status updated successfully", okResult.Value);
        }

        [Test]
        public async Task UpdateOrderStatus_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 101;
            int statusId = 2;
            _mockOrderService.Setup(s => s.GetOrderById(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, statusId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Order not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetDeliveryPartnerOrders_ReturnsOk_WhenOrdersExist()
        {
            // Arrange
            int deliveryPartnerId = 1;

            var mockOrders = new List<DeliveryPartnerOrderResponse>
            {
                new DeliveryPartnerOrderResponse
                {
                    OrderId = 101,
                    RestaurantName = "Pizza Hut",
                    CustomerName = "John Doe",
                    CustomerAddress = "123 Pizza Street",
                    OrderedItems = new List<OrderedItem>
                {
                        new OrderedItem { ItemName = "Pizza", Quantity = 2 },
                        new OrderedItem { ItemName = "Coke", Quantity = 1 }
                }
            }
        };

            _mockOrderService.Setup(s => s.GetDeliveryPartnerOrders(deliveryPartnerId))
                .ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetDeliveryPartnerOrders(deliveryPartnerId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedOrders = okResult.Value as List<DeliveryPartnerOrderResponse>;
            Assert.AreEqual(1, returnedOrders.Count);
            Assert.AreEqual("John Doe", returnedOrders[0].CustomerName);
            Assert.AreEqual("Pizza", returnedOrders[0].OrderedItems[0].ItemName);
        }


        [Test]
        public async Task GetDeliveryPartnerOrders_ReturnsNotFound_WhenNoOrdersExist()
        {
            // Arrange
            int deliveryPartnerId = 1;

            // Mocking that there are no orders for the given delivery partner
            _mockOrderService.Setup(s => s.GetDeliveryPartnerOrders(deliveryPartnerId))
                .ReturnsAsync(new List<DeliveryPartnerOrderResponse>());

            // Act
            var result = await _controller.GetDeliveryPartnerOrders(deliveryPartnerId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("No orders found for this delivery partner", notFoundResult.Value);
        }
    }
}
