using HotPotAPI.Controllers;
using HotPotAPI.Interfaces;
using HotPotAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using HotPotAPI.Models;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class RestaurantManagerControllerTests
    {
        private Mock<IRestaurantManagerService> _restaurantManagerServiceMock;
        private Mock<IOrderService> _orderServiceMock;
        private RestaurantManagerController _controller;

        [SetUp]
        public void SetUp()
        {
            _restaurantManagerServiceMock = new Mock<IRestaurantManagerService>();
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new RestaurantManagerController(_restaurantManagerServiceMock.Object, _orderServiceMock.Object);
        }

        [Test]
        public async Task AddRestaurantManager_WhenValidRequest_ReturnsOkWithResponse()
        {
            // Arrange
            var request = new CreateRestaurantManagerRequest
            {
                FullName = "Arun Kumar",
                Email = "arun@example.com",
                Password = "Secure@123",
                PhoneNumber = "9876543210",
                RestaurantId = 5
            };

            var expectedResponse = new CreateResturantManagerResponse
            {
                Id = 1,
                FullName = "Arun Kumar",
                Email = "arun@example.com",
                Phone = "9876543210",
                RestaurantName = "HotSpot Restaurant"
            };

            _restaurantManagerServiceMock.Setup(s => s.AddRestaurantManager(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddRestaurantManager(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResponse, okResult.Value);
        }

        [Test]
        public async Task AddRestaurantManager_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateRestaurantManagerRequest
            {
                FullName = "Test Name",
                Email = "test@example.com",
                Password = "Test123@",
                PhoneNumber = "1234567890",
                RestaurantId = 1
            };

            var exceptionMessage = "Something went wrong";

            _restaurantManagerServiceMock
                .Setup(s => s.AddRestaurantManager(request))
                .ThrowsAsync(new System.Exception(exceptionMessage));

            // Act
            var result = await _controller.AddRestaurantManager(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
            Assert.AreEqual(exceptionMessage, objectResult?.Value);
        }

        [Test]
        public async Task AddMenuItem_WhenValidRequest_ReturnsCreatedWithResponse()
        {
            // Arrange
            int managerId = 1;

            var request = new CreateMenuItemRequest
            {
                Name = "Paneer Butter Masala",
                Description = "Rich paneer dish",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 180,
                ImagePath = "/images/paneer.jpg",
                AvailabilityTime = "Lunch",
                IsAvailable = true,
                CuisineId = 2,
                MealTypeId = 3,
                Calories = 300,
                Proteins = 15,
                Fats = 20,
                Carbohydrates = 25,
                TasteInfo = "Spicy"
            };

            var expectedResponse = new CreateMenuItemResponse
            {
                Id = 10,
                ItemName = "Paneer Butter Masala",
                Description = "Rich paneer dish",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 180,
                ImagePath = "/images/paneer.jpg",
                IsAvailable = true,
                Cuisine = "North Indian",
                MealType = "Lunch",
                RestaurantId = 1,
                RestaurantName = "HotPot",
                tasteInfo = "Spicy"
            };

            _restaurantManagerServiceMock
                .Setup(s => s.AddMenuItem(managerId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddMenuItem(managerId, request);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(nameof(_controller.GetMenuItemById), createdResult.ActionName);
            Assert.AreEqual(expectedResponse, createdResult.Value);
        }

        [Test]
        public async Task AddMenuItem_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int managerId = 1;

            var request = new CreateMenuItemRequest
            {
                Name = "Paneer Butter Masala",
                Description = "Rich paneer dish",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 180,
                ImagePath = "/images/paneer.jpg",
                AvailabilityTime = "Lunch",
                IsAvailable = true,
                CuisineId = 2,
                MealTypeId = 3,
                Calories = 300,
                Proteins = 15,
                Fats = 20,
                Carbohydrates = 25,
                TasteInfo = "Spicy"
            };

            string exceptionMessage = "Database connection failed";

            _restaurantManagerServiceMock
                .Setup(s => s.AddMenuItem(managerId, request))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.AddMenuItem(managerId, request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);

            var messageProperty = objectResult.Value.GetType().GetProperty("Message");
            Assert.IsNotNull(messageProperty);
            Assert.AreEqual(exceptionMessage, messageProperty.GetValue(objectResult.Value));
        }

        [Test]
        public async Task UpdateManagerById_WhenValidRequest_ReturnsOkWithResponse()
        {
            // Arrange
            int managerId = 1;
            var updateRequest = new RestaurantManagerUpdate
            {
                Username = "Updated Name",
                Email = "updated@example.com",
                Password = "1234",
                PhoneNumber = "9999999999"
            };

            var expectedResponse = new CreateResturantManagerResponse
            {
                Id = managerId,
                FullName = "Updated Name",
                Email = "updated@example.com",
                Phone = "9999999999",
                RestaurantName = "HotPot"
            };

            _restaurantManagerServiceMock
                .Setup(s => s.UpdateManagerById(managerId, updateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateManagerById(managerId, updateRequest);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult.Value);
        }

        [Test]
        public async Task UpdateManagerById_WhenManagerNotFound_ReturnsNotFound()
        {
            // Arrange
            int managerId = 1;
            var updateRequest = new RestaurantManagerUpdate
            {
                Username = "Non-existent",
                Email = "notfound@example.com",
                Password = "1234",
                PhoneNumber = "0000000000"
            };

            _restaurantManagerServiceMock
                .Setup(s => s.UpdateManagerById(managerId, updateRequest))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateManagerById(managerId, updateRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual("Manager not found", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateManagerById_WhenExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            int managerId = 1;
            var updateRequest = new RestaurantManagerUpdate
            {
                Username = "Error Case",
                Email = "error@example.com",
                Password="1234",
                PhoneNumber = "1234567890"
            };

            var errorMessage = "Validation failed";

            _restaurantManagerServiceMock
                .Setup(s => s.UpdateManagerById(managerId, updateRequest))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.UpdateManagerById(managerId, updateRequest);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.AreEqual(errorMessage, badRequest.Value);
        }

        [Test]
        public async Task DeleteManagerById_WhenManagerExists_ReturnsOk()
        {
            // Arrange
            int managerId = 1;
            _restaurantManagerServiceMock
                .Setup(s => s.DeleteManagerById(managerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteManagerById(managerId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("Restaurant manager deleted successfully.", (string)obj.Message);
        }

        [Test]
        public async Task DeleteManagerById_WhenManagerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int managerId = 99;
            _restaurantManagerServiceMock
                .Setup(s => s.DeleteManagerById(managerId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteManagerById(managerId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var json = JsonConvert.SerializeObject(notFoundResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("Restaurant manager not found.", (string)obj.Message);
        }


        [Test]
        public async Task GetAllMenuItems_WhenMenuItemsExist_ReturnsOkWithItems()
        {
            // Arrange
            int managerId = 1;
            var menuItems = new List<CreateMenuItemResponse>
        {
        new CreateMenuItemResponse
        {
            Id = 101,
            ItemName = "Veg Burger",
            ImagePath = "/images/veg-burger.jpg",
            Description = "Spicy veg burger with cheese",
            CookingTime = TimeSpan.FromMinutes(15),
            Price = 120.50m,
            IsAvailable = true,
            Cuisine = "Fast Food",
            MealType = "Lunch",
            RestaurantId = 10,
            RestaurantName = "HotPot Diner",
            tasteInfo = "Spicy"
        }
        };

            _restaurantManagerServiceMock
                .Setup(s => s.GetAvailableMenuItems(managerId))
                .ReturnsAsync(menuItems);

            // Act
            var result = await _controller.GetAllMenuItems(managerId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItems = okResult.Value as List<CreateMenuItemResponse>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(menuItems.Count, returnedItems.Count);
            Assert.AreEqual(menuItems[0].ItemName, returnedItems[0].ItemName);
        }

        [Test]
        public async Task GetAllMenuItems_WhenNoItemsExist_ReturnsNotFound()
        {
            // Arrange
            int managerId = 2;
            _restaurantManagerServiceMock
                .Setup(s => s.GetAvailableMenuItems(managerId))
                .ReturnsAsync(new List<CreateMenuItemResponse>());

            // Act
            var result = await _controller.GetAllMenuItems(managerId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var json = JsonConvert.SerializeObject(notFoundResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("No menu items found for this manager's restaurant.", (string)obj.Message);
        }

        [Test]
        public async Task GetMenuItemById_WhenItemExists_ReturnsOk()
        {
            // Arrange
            int menuItemId = 1;
            var expectedItem = new CreateMenuItemResponse
            {
                Id = menuItemId,
                ItemName = "Veg Burger",
                ImagePath = "/images/veg-burger.jpg",
                Description = "Spicy veg burger with cheese",
                CookingTime = TimeSpan.FromMinutes(15),
                Price = 120.50m,
                IsAvailable = true,
                Cuisine = "Fast Food",
                MealType = "Lunch",
                RestaurantId = 10,
                RestaurantName = "HotPot Diner",
                tasteInfo = "Spicy"
            };

            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemById(menuItemId))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _controller.GetMenuItemById(menuItemId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var item = okResult.Value as CreateMenuItemResponse;
            Assert.IsNotNull(item);
            Assert.AreEqual(menuItemId, item.Id);
        }

        [Test]
        public async Task GetMenuItemById_WhenItemDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int menuItemId = 999;
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemById(menuItemId))
                .ReturnsAsync((CreateMenuItemResponse)null);

            // Act
            var result = await _controller.GetMenuItemById(menuItemId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"MenuItem with ID {menuItemId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetMenuItemById_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int menuItemId = 1;
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemById(menuItemId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetMenuItemById(menuItemId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);

            var json = JsonConvert.SerializeObject(objectResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("Database error", (string)obj.Message);
        }

        [Test]
        public async Task GetMenuItemsByName_WhenItemsExist_ReturnsOk()
        {
            // Arrange
            string menuName = "Pizza";
            var menuItems = new List<CreateMenuItemResponse>
    {
        new CreateMenuItemResponse
        {
            Id = 1,
            ItemName = "Veg Pizza",
            Description = "Cheesy veggie pizza",
            Price = 199.99m,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Lunch",
            RestaurantId = 5,
            RestaurantName = "HotPot",
            tasteInfo = "Mild",
            CookingTime = TimeSpan.FromMinutes(20),
            ImagePath = "images/pizza.jpg"
        },
        new CreateMenuItemResponse
        {
            Id = 2,
            ItemName = "Paneer Pizza",
            Description = "Spicy paneer topping",
            Price = 229.99m,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Dinner",
            RestaurantId = 5,
            RestaurantName = "HotPot",
            tasteInfo = "Spicy",
            CookingTime = TimeSpan.FromMinutes(25),
            ImagePath = "images/paneerpizza.jpg"
        }
    };

            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByName(menuName))
                .ReturnsAsync(menuItems);

            // Act
            var result = await _controller.GetMenuItemsByName(menuName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItems = okResult.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [Test]
        public async Task GetMenuItemsByName_WhenNoItemsFound_ReturnsNotFound()
        {
            // Arrange
            string menuName = "Sushi";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByName(menuName))
                .ReturnsAsync(new List<CreateMenuItemResponse>());

            // Act
            var result = await _controller.GetMenuItemsByName(menuName);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"No menu items found with name containing '{menuName}'.", notFoundResult.Value);
        }

        [Test]
        public async Task GetMenuItemsByName_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            string menuName = "Burger";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByName(menuName))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetMenuItemsByName(menuName);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);

            var json = JsonConvert.SerializeObject(objectResult.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("Database error", (string)obj.Message);
        }

        [Test]
        public async Task GetMenuItemsByCuisine_WhenItemsExist_ReturnsOk()
        {
            // Arrange
            string cuisineName = "Italian";
            var items = new List<CreateMenuItemResponse>
        {
        new CreateMenuItemResponse
        {
            Id = 1,
            ItemName = "Pasta",
            Description = "Creamy white sauce",
            Price = 150,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Dinner",
            RestaurantId = 2,
            RestaurantName = "HotPot",
            tasteInfo = "Mild",
            CookingTime = TimeSpan.FromMinutes(15),
            ImagePath = "images/pasta.jpg"
        }
        };

            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByCuisine(cuisineName))
                .ReturnsAsync(items);

            // Act
            var result = await _controller.GetMenuItemsByCuisine(cuisineName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItems = okResult.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(1, returnedItems.Count());
        }

        [Test]
        public async Task GetMenuItemsByCuisine_WhenNoItemsFound_ReturnsNotFound()
        {
            // Arrange
            string cuisineName = "Mexican";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByCuisine(cuisineName))
                .ReturnsAsync(new List<CreateMenuItemResponse>());

            // Act
            var result = await _controller.GetMenuItemsByCuisine(cuisineName);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual($"No menu items found for cuisine '{cuisineName}'.", notFoundResult.Value);
        }

        [Test]
        public async Task GetMenuItemsByCuisine_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            string cuisineName = "Chinese";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByCuisine(cuisineName))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.GetMenuItemsByCuisine(cuisineName);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);

            var json = JsonConvert.SerializeObject(objectResult?.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("DB error", (string)obj.Message);
        }

        [Test]
        public async Task GetMenuItemsByMealType_WhenItemsExist_ReturnsOk()
        {
            // Arrange
            string mealTypeName = "Lunch";
            var items = new List<CreateMenuItemResponse>
        {
        new CreateMenuItemResponse
        {
            Id = 2,
            ItemName = "Paneer Butter Masala",
            Description = "Rich tomato gravy",
            Price = 180,
            IsAvailable = true,
            Cuisine = "Indian",
            MealType = "Lunch",
            RestaurantId = 3,
            RestaurantName = "HotPot Express",
            tasteInfo = "Spicy",
            CookingTime = TimeSpan.FromMinutes(20),
            ImagePath = "images/paneer.jpg"
        }
        };

            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByMealType(mealTypeName))
                .ReturnsAsync(items);

            // Act
            var result = await _controller.GetMenuItemsByMealType(mealTypeName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult?.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.AreEqual(1, returnedItems?.Count());
        }


        [Test]
        public async Task GetMenuItemsByMealType_WhenNoItemsFound_ReturnsNotFound()
        {
            // Arrange
            string mealTypeName = "Breakfast";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByMealType(mealTypeName))
                .ReturnsAsync(new List<CreateMenuItemResponse>());

            // Act
            var result = await _controller.GetMenuItemsByMealType(mealTypeName);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual($"No menu items found for meal type '{mealTypeName}'.", notFoundResult?.Value);
        }

        [Test]
        public async Task GetMenuItemsByMealType_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            string mealTypeName = "Dinner";
            _restaurantManagerServiceMock
                .Setup(s => s.GetMenuItemsByMealType(mealTypeName))
                .ThrowsAsync(new Exception("Unexpected failure"));

            // Act
            var result = await _controller.GetMenuItemsByMealType(mealTypeName);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);

            var json = JsonConvert.SerializeObject(objectResult?.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.AreEqual("Unexpected failure", (string)obj.Message);
        }

        
        [Test]
        public async Task SetAvailabilityForAll_WhenNoMenuItemsFound_ReturnsNotFound()
        {
            // Arrange
            _restaurantManagerServiceMock
                .Setup(s => s.SetAvailabilityForAllMenuItems())
                .ReturnsAsync(false); 

            // Act
            var result = await _controller.SetAvailabilityForAll();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result); 
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("No menu items found.", notFoundResult.Value);
        }

        [Test]
        public async Task SearchMenuItems_WhenValidParameters_ReturnsOkResult()
        {
            // Arrange
            var searchItemName = "Pizza";
            var searchCuisineName = "Italian";
            var searchMealTypeName = "Main Course";

            var mockMenuItems = new List<CreateMenuItemResponse>
        {
        new CreateMenuItemResponse
        {
            Id = 1,
            ItemName = "Margherita Pizza",
            Description = "Classic Margherita pizza",
            CookingTime = TimeSpan.FromMinutes(15),
            Price = 9.99m,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Main Course",
            RestaurantId = 1,
            RestaurantName = "Pizza Place",
            tasteInfo = "Tomato, Mozzarella, Basil"
        },
        new CreateMenuItemResponse
        {
            Id = 2,
            ItemName = "Pepperoni Pizza",
            Description = "Pepperoni pizza with extra cheese",
            CookingTime = TimeSpan.FromMinutes(20),
            Price = 12.99m,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Main Course",
            RestaurantId = 1,
            RestaurantName = "Pizza Place",
            tasteInfo = "Pepperoni, Mozzarella, Tomato"
        }
        };

            _restaurantManagerServiceMock
                .Setup(s => s.SearchMenuItems(searchItemName, searchCuisineName, searchMealTypeName))
                .ReturnsAsync(mockMenuItems);

            // Act
            var actionResult = await _controller.SearchMenuItems(searchItemName, searchCuisineName, searchMealTypeName);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<OkObjectResult>(okResult);

            var menuItems = okResult.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.IsNotNull(menuItems);
            Assert.AreEqual(2, menuItems.Count());
            Assert.AreEqual("Margherita Pizza", menuItems.First().ItemName);
            Assert.AreEqual("Italian", menuItems.First().Cuisine);
        }

        [Test]
        public async Task SearchMenuItems_WhenNoItemsFound_ReturnsEmptyList()
        {
            // Arrange
            var searchItemName = "NonExistent";
            var searchCuisineName = "Unknown";
            var searchMealTypeName = "Unknown";

            _restaurantManagerServiceMock
                .Setup(s => s.SearchMenuItems(searchItemName, searchCuisineName, searchMealTypeName))
                .ReturnsAsync(new List<CreateMenuItemResponse>());

            // Act
            var actionResult = await _controller.SearchMenuItems(searchItemName, searchCuisineName, searchMealTypeName);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<OkObjectResult>(okResult);

            var menuItems = okResult.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.IsNotNull(menuItems);
            Assert.AreEqual(0, menuItems.Count());
        }

        [Test]
        public async Task FilterMenuItems_WhenValidParameters_ReturnsOkResult()
        {
            // Arrange
            decimal? minPrice = 5.00m;
            decimal? maxPrice = 15.00m;
            bool? isAvailable = true;
            string? cuisineName = "Italian";
            string? mealTypeName = "Main Course";
            string? sortBy = "price";
            string? sortOrder = "asc";

            var mockFilteredItems = new List<CreateMenuItemResponse>
        {
        new CreateMenuItemResponse
        {
            Id = 1,
            ItemName = "Veg Pizza",
            Description = "Delicious Veg Pizza",
            CookingTime = TimeSpan.FromMinutes(18),
            Price = 10.00m,
            IsAvailable = true,
            Cuisine = "Italian",
            MealType = "Main Course",
            RestaurantId = 1,
            RestaurantName = "Pizza Hut",
            tasteInfo = "Cheese, Tomato, Veggies"
        }
        };

            _restaurantManagerServiceMock
                .Setup(s => s.FilterMenuItems(minPrice, maxPrice, isAvailable, cuisineName, mealTypeName, sortBy, sortOrder))
                .ReturnsAsync(mockFilteredItems);

            // Act
            var actionResult = await _controller.FilterMenuItems(minPrice, maxPrice, isAvailable, cuisineName, mealTypeName, sortBy, sortOrder);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<OkObjectResult>(okResult);

            var items = okResult.Value as IEnumerable<CreateMenuItemResponse>;
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count());
            Assert.AreEqual("Veg Pizza", items.First().ItemName);
        }

        [Test]
        public async Task DeleteMenuItem_WhenItemExists_ReturnsNoContent()
        {
            // Arrange
            int menuItemId = 1;

            _restaurantManagerServiceMock
                .Setup(s => s.DeleteMenuItem(menuItemId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteMenuItem(menuItemId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteMenuItem_WhenItemDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int menuItemId = 99;

            _restaurantManagerServiceMock
                .Setup(s => s.DeleteMenuItem(menuItemId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteMenuItem(menuItemId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"MenuItem with ID {menuItemId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateMenuItem_WhenValid_ReturnsOkResult()
        {
            // Arrange
            int menuItemId = 1;
            var updateDto = new MenuItemUpdate
            {
                Name = "Updated Pizza",
                Description = "Spicy Italian Pizza",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 11.99m,
                ImagePath = "pizza.jpg",
                AvailabilityTime = "11:00-23:00",
                IsAvailable = true,
                CuisineId = 1,
                MealTypeId = 2,
                Calories = 300,
                Proteins = 15,
                Fats = 10,
                Carbohydrates = 35,
                TasteInfo = "Spicy, Cheesy"
            };

            var updatedResponse = new CreateMenuItemResponse
            {
                Id = menuItemId,
                ItemName = updateDto.Name,
                Description = updateDto.Description,
                CookingTime = updateDto.CookingTime,
                Price = updateDto.Price,
                IsAvailable = updateDto.IsAvailable,
                tasteInfo = updateDto.TasteInfo
            };

            _restaurantManagerServiceMock
                .Setup(s => s.UpdateMenuItem(menuItemId, updateDto))
                .ReturnsAsync(updatedResponse);

            // Act
            var result = await _controller.UpdateMenuItem(menuItemId, updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var item = okResult.Value as CreateMenuItemResponse;
            Assert.IsNotNull(item);
            Assert.AreEqual("Updated Pizza", item.ItemName);
        }

        [Test]
        public async Task UpdateMenuItem_WhenItemNotFound_ReturnsNotFound()
        {
            // Arrange
            int menuItemId = 99;
            var updateDto = new MenuItemUpdate
            {
                Name = "Nonexistent Item",
                Price = 10.00m
                
            };

            _restaurantManagerServiceMock
                .Setup(s => s.UpdateMenuItem(menuItemId, updateDto))
                .ReturnsAsync((CreateMenuItemResponse?)null);

            // Act
            var result = await _controller.UpdateMenuItem(menuItemId, updateDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Menu item with ID {menuItemId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateOrderStatus_WhenServiceSucceeds_ReturnsNoContent()
        {
            // Arrange
            int managerId = 1;
            int orderId = 1001;
            int statusId = 2;

            // Mock the service to simulate the behavior of updating the order status
            _orderServiceMock
                .Setup(service => service.UpdateOrderStatusByRestaurant(orderId, statusId, managerId))
                .Returns(Task.CompletedTask);  // Mock Task completion (success)

            // Act
            var result = await _controller.UpdateOrderStatus(managerId, orderId, statusId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);  // Ensure the result is NoContent (HTTP 204)
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);  // Ensure it's not null
            Assert.AreEqual(204, noContentResult.StatusCode);  // Ensure the status code is 204 No Content
        }

        [Test]
        public async Task GetOrdersByManager_WhenCalled_ReturnsOkWithOrders()
        {
            // Arrange
            int managerId = 1;
            var mockOrders = new List<Order>
        {
        new Order
        {
            OrderId = 1,
            CustomerId = 101,
            OrderDate = DateTime.Now,
            TotalAmount = 299.99m,
            CustomerAddressId = 1,
            OrderStatusId = 1,
            PaymentMethodId = 1,
            PaymentStatusId = 2,
            DeliveryPartnerId = 5,
            OrderItems = new List<OrderItem>()
        }
        };

            _orderServiceMock
                .Setup(service => service.GetOrdersByRestaurantManager(managerId))
                .ReturnsAsync(mockOrders);

            // Act
            var result = await _controller.GetOrdersByManager(managerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var orders = okResult.Value as List<Order>;
            Assert.IsNotNull(orders);
            Assert.AreEqual(mockOrders.Count, orders.Count);
            Assert.AreEqual(mockOrders[0].OrderId, orders[0].OrderId);
        }

        [Test]
        public async Task GetOrdersByManager_WhenNoOrdersFound_ReturnsOkWithEmptyList()
        {
            // Arrange
            int managerId = 2;
            var emptyOrderList = new List<Order>();

            _orderServiceMock
                .Setup(service => service.GetOrdersByRestaurantManager(managerId))
                .ReturnsAsync(emptyOrderList);

            // Act
            var result = await _controller.GetOrdersByManager(managerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var orders = okResult.Value as List<Order>;
            Assert.IsNotNull(orders);
            Assert.IsEmpty(orders);
        }

































    }
}
