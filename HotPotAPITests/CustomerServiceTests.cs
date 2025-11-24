using Castle.Core.Resource;
using HotPotAPI.Contexts;
using HotPotAPI.Exceptions;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private CustomerService _service;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<int, Customer>> _customerRepoMock;
        private Mock<IRepository<int, Restaurant>> _restaurantRepoMock;
        private Mock<IRepository<int, MenuItem>> _menuItemRepoMock;
        private Mock<IRepository<int, Cart>> _cartRepoMock;
        private Mock<IRepository<int, CartItem>> _cartItemRepoMock;
        private Mock<HotPotDbContext> _context;

        [SetUp]
        public void Setup()
        {
            // Use real in-memory DB context
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Mock<HotPotDbContext>(options);

            _userRepoMock = new Mock<IRepository<string, User>>();
            _customerRepoMock = new Mock<IRepository<int, Customer>>();
            _restaurantRepoMock = new Mock<IRepository<int, Restaurant>>();
            _menuItemRepoMock = new Mock<IRepository<int, MenuItem>>();
            _cartRepoMock = new Mock<IRepository<int, Cart>>();
            _cartItemRepoMock = new Mock<IRepository<int, CartItem>>();

            _service = new CustomerService(
                _context.Object,
                _userRepoMock.Object,
                _customerRepoMock.Object,
                _restaurantRepoMock.Object,
                _menuItemRepoMock.Object,
                _cartRepoMock.Object,
                _cartItemRepoMock.Object
            );


        }
        [Test]
        public async Task AddCustomer_Success_ReturnsCreateCustomerResponse()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "John Doe",
                Gender = "Male",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Addresses = new List<CreateCustomerAddress>
            {
                new CreateCustomerAddress { Label = "Home", Street = "123 Street", City = "City", Pincode = "12345" }
            },
                Password = "password123"
            };

            // Mock the user repository to return a non-null user
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Username = request.Email });

            // Mock the customer repository to return a non-null customer
            _customerRepoMock.Setup(r => r.Add(It.IsAny<Customer>())).ReturnsAsync(new Customer { Id = 1 });

            // Act
            var result = await _service.AddCustomer(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id); // Assert that the customer ID is returned correctly
        }

        [Test]
        public void AddCustomer_FailsToCreateUser_ThrowsException()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "John Doe",
                Gender = "Male",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Addresses = new List<CreateCustomerAddress>
            {
                new CreateCustomerAddress { Label = "Home", Street = "123 Street", City = "City", Pincode = "12345" }
            },
                Password = "password123"
            };

            // Mock the user repository to return null (simulating failure)
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddCustomer(request));
            Assert.AreEqual("Failed to create user", ex.Message); // Assert the correct exception message
        }

        [Test]
        public async Task AddCustomer_FailsToCreateUser_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "John Doe",
                Gender = "Male",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Addresses = new List<CreateCustomerAddress>
            {
            new CreateCustomerAddress { Label = "Home", Street = "123 Street", City = "City", Pincode = "12345" }
            },
                Password = "password123"
            };

            // Mock the user repository to throw a duplicate email exception
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ThrowsAsync(new EmailAlreadyExistsException(request.Email));

            // Act & Assert
            var ex = Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _service.AddCustomer(request));
            Assert.AreEqual($"The email '{request.Email}' is already registered.", ex.Message); // Assert the custom exception message
        }

        [Test]
        public void AddCustomer_FailsToCreateCustomer_ThrowsException()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "John Doe",
                Gender = "Male",
                Email = "johndoe@example.com",
                Phone = "1234567890",
                Addresses = new List<CreateCustomerAddress>
            {
                new CreateCustomerAddress { Label = "Home", Street = "123 Street", City = "City", Pincode = "12345" }
            },
                Password = "password123"
            };

            // Mock the user repository to return a valid user
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Username = request.Email });

            // Mock the customer repository to return null (simulating failure)
            _customerRepoMock.Setup(r => r.Add(It.IsAny<Customer>())).ReturnsAsync((Customer)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddCustomer(request));
            Assert.AreEqual("Failed to create customer", ex.Message); 
        }

        [Test]
        public async Task GetAllRestaurants_Success_ReturnsListOfRestaurants()
        {
            // Arrange: Set up mock data for restaurants
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" },
                new Restaurant { RestaurantId = 2, RestaurantName = "Burger King", Location = "Los Angeles", ContactNumber = "0987654321", Email = "burgerking@domain.com", Restaurantlogo = "logo2.png" }
            };

            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);

            // Act: Call the service method to get all restaurants
            var result = await _service.GetAllRestaurants();

            // Assert: Verify that the result is a list of restaurants and matches the mock data
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual("Pizza Hut", result[0].RestaurantName); 
        }

        [Test]
        public async Task GetAllRestaurants_EmptyList_ReturnsEmptyList()
        {
            // Arrange: Set up an empty list of restaurants
            var mockRestaurants = new List<Restaurant>();

            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);

            // Act: Call the service method to get all restaurants
            var result = await _service.GetAllRestaurants();

            // Assert: Verify that the result is an empty list
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count); 
        }

        [Test]
        public async Task GetRestaurantByName_Success_ReturnsRestaurant()
        {
            // Arrange: Set up mock data for restaurants
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" },
                new Restaurant { RestaurantId = 2, RestaurantName = "Burger King", Location = "Los Angeles", ContactNumber = "0987654321", Email = "burgerking@domain.com", Restaurantlogo = "logo2.png" }
            };

            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);

            // Act: Call the service method to get a restaurant by name
            var result = await _service.GetRestaurantByName("Pizza Hut");

            // Assert: Verify that the correct restaurant is returned
            Assert.IsNotNull(result);
            Assert.AreEqual("Pizza Hut", result.RestaurantName); // Check that the returned restaurant name matches
        }

        [Test]
        public async Task GetRestaurantByName_RestaurantNotFound_ReturnsNull()
        {
            // Arrange: Set up mock data for restaurants
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" },
                new Restaurant { RestaurantId = 2, RestaurantName = "Burger King", Location = "Los Angeles", ContactNumber = "0987654321", Email = "burgerking@domain.com", Restaurantlogo = "logo2.png" }
            };

            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);

            // Act: Call the service method to get a restaurant by a name that does not exist
            var result = await _service.GetRestaurantByName("McDonald's");

            // Assert: Verify that null is returned when the restaurant is not found
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetMenuByRestaurantName_RestaurantFound_ReturnsMenuItems()
        {
            // Arrange: Mock data for restaurants and menu items with the new fields
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" },
                new Restaurant { RestaurantId = 2, RestaurantName = "Burger King", Location = "Los Angeles", ContactNumber = "0987654321", Email = "burgerking@domain.com", Restaurantlogo = "logo2.png" }
            };

            var mockMenuItems = new List<MenuItem>
            {
            new MenuItem
            {
            Id = 1,
            Name = "Pizza",
            Description = "Delicious cheese pizza",
            CookingTime = TimeSpan.FromMinutes(15),
            Price = 10m,
            ImagePath = "pizza.png",
            AvailabilityTime = "10:00 AM - 11:00 PM",
            IsAvailable = true,
            CuisineId = 1,
            MealTypeId = 1,
            RestaurantId = 1,
            Calories = 250,
            Proteins = 10.5f,
            Fats = 9.2f,
            Carbohydrates = 30.0f,
            TasteInfo = "Cheesy and crispy"
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
            CuisineId = 1,
            MealTypeId = 1,
            RestaurantId = 1,
            Calories = 350,
            Proteins = 15.0f,
            Fats = 12.0f,
            Carbohydrates = 45.0f,
            TasteInfo = "Creamy and rich"
            },
            new MenuItem
            {
            Id = 3,
            Name = "Burger",
            Description = "Juicy beef burger",
            CookingTime = TimeSpan.FromMinutes(10),
            Price = 8m,
            ImagePath = "burger.png",
            AvailabilityTime = "10:00 AM - 11:00 PM",
            IsAvailable = true,
            CuisineId = 2,
            MealTypeId = 2,
            RestaurantId = 2,
            Calories = 450,
            Proteins = 20.0f,
            Fats = 25.0f,
            Carbohydrates = 50.0f,
            TasteInfo = "Juicy and savory"
            }
            };

            // Mock the repository methods
            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);
            _menuItemRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockMenuItems);

            // Act: Call the service method to get menu items for "Pizza Hut"
            var result = await _service.GetMenuByRestaurantName("Pizza Hut");

            // Assert: Verify that the correct menu items are returned
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual("Pizza", result[0].Name); 
            Assert.AreEqual("Pasta", result[1].Name);

            // Assert that the nutritional info is correctly mapped
            Assert.AreEqual(250, result[0].Calories);
            Assert.AreEqual(10.5f, result[0].Proteins);
            Assert.AreEqual(9.2f, result[0].Fats);
            Assert.AreEqual(30.0f, result[0].Carbohydrates);

            // Assert availability time and other fields
            Assert.AreEqual("10:00 AM - 11:00 PM", result[0].AvailabilityTime);
            Assert.AreEqual("Cheesy and crispy", result[0].TasteInfo);
        }

        [Test]
        public void GetMenuByRestaurantName_RestaurantNotFound_ThrowsException()
        {
            // Arrange: Mock data for restaurants and menu items
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" }
            };
            var mockMenuItems = new List<MenuItem>
            {
            new MenuItem
            {
                Id = 1,Name = "Pizza",Description = "Delicious cheese pizza",CookingTime = TimeSpan.FromMinutes(15),Price = 10m,
                ImagePath = "pizza.png",AvailabilityTime = "10:00 AM - 11:00 PM",IsAvailable = true,CuisineId = 1,
                MealTypeId = 1,RestaurantId = 1,Calories = 250,Proteins = 10.5f,Fats = 9.2f,Carbohydrates = 30.0f,TasteInfo = "Cheesy and crispy"
            }
            };
            // Mock the repository methods
            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);
            _menuItemRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockMenuItems);
            // Act & Assert: Verify that an exception is thrown when restaurant is not found
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetMenuByRestaurantName("Burger King"));
            Assert.AreEqual("Restaurant not found.", ex.Message); // Assert the exception message
        }

        [Test]
        public async Task GetMenuByRestaurantName_RestaurantFound_NoMenuItems_ReturnsEmptyList()
        {
            // Arrange: Mock data for restaurants and menu items
            var mockRestaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut", Location = "New York", ContactNumber = "1234567890", Email = "pizzahut@domain.com", Restaurantlogo = "logo1.png" }
            };
            var mockMenuItems = new List<MenuItem>(); 

            // Mock the repository methods
            _restaurantRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockRestaurants);
            _menuItemRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockMenuItems);

            // Act: Call the service method to get menu items for "Pizza Hut"
            var result = await _service.GetMenuByRestaurantName("Pizza Hut");

            // Assert: Verify that the result is an empty list
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count); 
        }

        [Test]
        public async Task GetAvailableMenuItems_ReturnsOnlyAvailableItems_WithMappedFields()
        {
            // Arrange
            var mockMenuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Id = 1,Name = "Pizza",Description = "Cheesy pizza",CookingTime = TimeSpan.FromMinutes(15),Price = 9.99m,IsAvailable = true,
                ImagePath = "pizza.jpg",Cuisine = new Cuisine { Name = "Italian" },MealType = new MealType { Name = "Dinner" },
                RestaurantId = 1,Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Pizza Hut" }
            },
            new MenuItem
            {
                Id = 2,Name = "Burger",Description = "Beef burger",CookingTime = TimeSpan.FromMinutes(10),Price = 5.49m,
                IsAvailable = false,ImagePath = "burger.jpg",Cuisine = new Cuisine { Name = "American" },MealType = new MealType { Name = "Lunch" },
                RestaurantId = 2,Restaurant = new Restaurant { RestaurantId = 2, RestaurantName = "Burger King" }
            }
        };

            _menuItemRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockMenuItems);

            // Act
            var result = await _service.GetAvailableMenuItems();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            var item = result.First();
            Assert.AreEqual(1, item.Id);
            Assert.AreEqual("Pizza", item.ItemName);
            Assert.AreEqual("Cheesy pizza", item.Description);
            Assert.AreEqual(TimeSpan.FromMinutes(15), item.CookingTime);
            Assert.AreEqual(9.99m, item.Price);
            Assert.AreEqual(true, item.IsAvailable);
            Assert.AreEqual("pizza.jpg", item.ImagePath);
            Assert.AreEqual("Italian", item.Cuisine);
            Assert.AreEqual("Dinner", item.MealType);
            Assert.AreEqual(1, item.RestaurantId);
            Assert.AreEqual("Pizza Hut", item.RestaurantName);
        }

        [Test]
        public async Task GetAvailableMenuItems_NoAvailableItems_ReturnsEmptyList()
        {
            // Arrange
            var mockMenuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Id = 1,Name = "Pizza",Description = "Delicious cheese pizza",CookingTime = TimeSpan.FromMinutes(15),Price = 10m,
                ImagePath = "pizza.png",AvailabilityTime = "10:00 AM - 11:00 PM",IsAvailable = false,CuisineId = 1,
                MealTypeId = 1,RestaurantId = 1,Calories = 250,Proteins = 10.5f,Fats = 9.2f,Carbohydrates = 30.0f,TasteInfo = "Cheesy and crispy"
            }
        };

            _menuItemRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockMenuItems);

            // Act
            var result = await _service.GetAvailableMenuItems();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetMenuByRestaurantId_ValidId_ReturnsMenuItems()
        {
            // Arrange
            var restaurantId = 1;
            var restaurant = new Restaurant
            {
                RestaurantId = restaurantId,
                RestaurantName = "Foodie Hub",
                Location = "Chennai",
                ContactNumber = "1234567890",
                Email = "contact@foodie.com",
                Restaurantlogo = "logo.png"
            };

            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = 1, Name = "Pizza", RestaurantId = restaurantId },
                new MenuItem { Id = 2, Name = "Burger", RestaurantId = restaurantId },
                new MenuItem { Id = 3, Name = "Pasta", RestaurantId = 99 } // Should be excluded
            };

            _restaurantRepoMock.Setup(r => r.GetById(restaurantId)).ReturnsAsync(restaurant);
            _menuItemRepoMock.Setup(m => m.GetAll()).ReturnsAsync(menuItems);

            // Act
            var result = await _service.GetMenuByRestaurantId(restaurantId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(m => m.RestaurantId == restaurantId));
        }

        [Test]
        public void GetMenuByRestaurantId_InvalidId_ThrowsException()
        {
            // Arrange
            int invalidId = 999;
            _restaurantRepoMock.Setup(r => r.GetById(invalidId)).ReturnsAsync((Restaurant)null!);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetMenuByRestaurantId(invalidId));
            Assert.AreEqual("Restaurant not found.", ex.Message);
        }

        [Test]
        public async Task GetMenuItemsByName_ExactMatch_ReturnsMatchingMenuItems()
        {
            // Arrange
            var menuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Id = 1,
                Name = "Chicken Biryani",
                Description = "Spicy rice with chicken",
                CookingTime = TimeSpan.FromMinutes(20),
                Price = 180.00m,
                IsAvailable = true,
                Cuisine = new Cuisine { Name = "Indian" },
                MealType = new MealType { Name = "Lunch" },
                RestaurantId = 1,
                Restaurant = new Restaurant
                {
                    RestaurantId = 1,
                    RestaurantName = "Biryani House",
                    Location = "Chennai",
                    ContactNumber = "9876543210",
                    Email = "biryani@house.com",
                    Restaurantlogo = "biryani_logo.png"
                }
            },
            new MenuItem { Id = 2, Name = "Paneer Tikka", RestaurantId = 2 }
        };

            _menuItemRepoMock.Setup(m => m.GetAll()).ReturnsAsync(menuItems);

            // Act
            var result = await _service.GetMenuItemsByName("biryani");

            // Assert
            Assert.AreEqual(1, result.Count);
            var item = result.First();
            Assert.AreEqual("Chicken Biryani", item.ItemName);
            Assert.AreEqual("Indian", item.Cuisine);
            Assert.AreEqual("Lunch", item.MealType);
            Assert.AreEqual("Biryani House", item.RestaurantName);
        }

        [Test]
        public async Task GetMenuItemsByName_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            var menuItems = new List<MenuItem>
        {
            new MenuItem { Id = 1, Name = "Dosa" },
            new MenuItem { Id = 2, Name = "Idli" }
        };

            _menuItemRepoMock.Setup(m => m.GetAll()).ReturnsAsync(menuItems);

            // Act
            var result = await _service.GetMenuItemsByName("burger");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetMenuItemsByCuisine_ValidCuisine_ReturnsFilteredItems()
        {
            // Arrange
            var menuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Id = 1,
                Name = "Tandoori Chicken",
                Cuisine = new Cuisine { Name = "Indian" },
                MealType = new MealType { Name = "Dinner" },
                Description = "Grilled chicken",
                CookingTime = TimeSpan.FromMinutes(25),
                Price = 250,
                IsAvailable = true,
                RestaurantId = 1,
                Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Spice Hut" }
            },
            new MenuItem
            {
                Id = 2,
                Name = "Chow Mein",
                Cuisine = new Cuisine { Name = "Chinese" },
                RestaurantId = 2
            }
        };

            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(menuItems);

            // Act
            var result = await _service.GetMenuItemsByCuisine("indian");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Tandoori Chicken", result[0].ItemName);
            Assert.AreEqual("Indian", result[0].Cuisine);
        }

        [Test]
        public async Task GetMenuItemsByCuisine_NoMatch_ReturnsEmptyList()
        {
            var menuItems = new List<MenuItem>
        {
            new MenuItem { Id = 1, Name = "Pizza", Cuisine = new Cuisine { Name = "Italian" } }
        };

            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(menuItems);

            var result = await _service.GetMenuItemsByCuisine("mexican");

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetMenuItemsByMealType_ValidMealType_ReturnsItems()
        {
            // Arrange
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Name = "Idli",
                Description = "Soft idlis",
                CookingTime = TimeSpan.FromMinutes(10),
                Price = 30,
                IsAvailable = true,
                MealType = new MealType { Name = "Breakfast" },
                Cuisine = new Cuisine { Name = "South Indian" },
                RestaurantId = 1,
                Restaurant = new Restaurant { RestaurantId = 1, RestaurantName = "Saravana Bhavan" }
            },
            new MenuItem
            {
                Name = "Pizza",
                MealType = new MealType { Name = "Dinner" },
                RestaurantId = 2
            }
        };

            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(items);

            // Act
            var result = await _service.GetMenuItemsByMealType("breakfast");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Idli", result[0].ItemName);
            Assert.AreEqual("Breakfast", result[0].MealType);
        }

        [Test]
        public async Task GetMenuItemsByMealType_NoMatch_ReturnsEmpty()
        {
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Name = "Burger",
                MealType = new MealType { Name = "Lunch" }
            }
        };

            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(items);

            var result = await _service.GetMenuItemsByMealType("dinner");

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetMenuItemsByMealType_MealTypeIsNull_DoesNotThrow()
        {
            var items = new List<MenuItem>
        {
            new MenuItem { Name = "Salad", MealType = null }
        };

            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(items);

            var result = await _service.GetMenuItemsByMealType("Lunch");

            Assert.IsEmpty(result);
        }

        private List<MenuItem> GetSampleMenuItems()
        {
            return new List<MenuItem>
        {
            new MenuItem
            {
                Name = "Idli",Description = "Steamed rice cakes",CookingTime = TimeSpan.FromMinutes(10),Price = 40,IsAvailable = true,
                ImagePath = "idli.jpg",Cuisine = new Cuisine { Name = "South Indian" },MealType = new MealType { Name = "Breakfast" },
                RestaurantId = 1,Restaurant = new Restaurant { RestaurantName = "Saravana Bhavan" }
        },
            new MenuItem
            {
                Name = "Pizza",Description = "Cheese Pizza",CookingTime = TimeSpan.FromMinutes(15),Price = 150,IsAvailable = true,
                ImagePath = "pizza.jpg",Cuisine = new Cuisine { Name = "Italian" },MealType = new MealType { Name = "Dinner" },
                RestaurantId = 2,Restaurant = new Restaurant { RestaurantName = "Domino's" }
        },
        new MenuItem
            {
                Name = "Burger",Description = "Grilled chicken burger",CookingTime = TimeSpan.FromMinutes(12),Price = 90,IsAvailable = false,
                ImagePath = "burger.jpg",Cuisine = null,MealType = null,RestaurantId = 3,Restaurant = new Restaurant { RestaurantName = "Burger King" }
            }
        };
    }

        [Test]
        public async Task SearchMenuItems_AllFiltersMatch_ReturnsMatchingItem()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.SearchMenuItems("Idli", "South Indian", "Breakfast");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Idli", result[0].ItemName);
        }

        [Test]
        public async Task SearchMenuItems_PartialFilter_ItemNameOnly()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.SearchMenuItems("Pizza", null, null);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Pizza", result[0].ItemName);
        }

        [Test]
        public async Task SearchMenuItems_NoFilters_ReturnsAllItems()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.SearchMenuItems(null, null, null);

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public async Task SearchMenuItems_NoMatch_ReturnsEmpty()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.SearchMenuItems("Biryani", "North Indian", "Lunch");

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task SearchMenuItems_CuisineOrMealTypeNull_DoesNotThrow()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.SearchMenuItems("Burger", "AnyCuisine", "AnyMeal");

            Assert.IsEmpty(result); 
        }

        [Test]
        public async Task FilterMenuItems_MinAndMaxPrice_ReturnsWithinRange()
        {
            // Arrange
            var menuItems = GetSampleMenuItems();
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(menuItems);

            // Act
            var result = (await _service.FilterMenuItems(
                minPrice: 80, maxPrice: 160,
                isAvailable: null,
                cuisineName: null,
                mealTypeName: null,
                sortBy: "price", sortOrder: "asc")).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Burger", result[0].ItemName);
            Assert.AreEqual("Pizza", result[1].ItemName);
        }


        [Test]
        public async Task FilterMenuItems_IsAvailableTrue_ReturnsOnlyAvailable()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, true, null, null, null, null);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(r => r.IsAvailable));
        }

        [Test]
        public async Task FilterMenuItems_ByCuisineAndMealType_ReturnsMatch()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, null, "South Indian", "Breakfast", null, null);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Idli", result.First().ItemName);
        }

        [Test]
        public async Task FilterMenuItems_SortByPriceDescending_ReturnsSorted()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, null, null, null, "price", "desc");

            var resultList = result.ToList();
            Assert.AreEqual("Pizza", resultList[0].ItemName);
            Assert.AreEqual("Burger", resultList[1].ItemName);
            Assert.AreEqual("Idli", resultList[2].ItemName);
        }

        [Test]
        public async Task FilterMenuItems_SortByNameAscending_DefaultSort()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, null, null, null, "name", "asc");

            var names = result.Select(r => r.ItemName).ToList();
            CollectionAssert.AreEqual(new[] { "Burger", "Idli", "Pizza" }, names);
        }

        [Test]
        public async Task FilterMenuItems_InvalidSortField_DefaultToName()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, null, null, null, "invalid", "asc");

            var names = result.Select(r => r.ItemName).ToList();
            CollectionAssert.AreEqual(new[] { "Burger", "Idli", "Pizza" }, names);
        }

        [Test]
        public async Task FilterMenuItems_AllNullParams_ReturnsAllSortedByName()
        {
            _menuItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(GetSampleMenuItems());

            var result = await _service.FilterMenuItems(null, null, null, null, null, null, null);

            var names = result.Select(r => r.ItemName).ToList();
            CollectionAssert.AreEqual(new[] { "Burger", "Idli", "Pizza" }, names);
        }

        [Test]
        public async Task GetCartByCustomerId_ExistingCartWithItems_ReturnsCartWithItems()
        {
            // Arrange
            int customerId = 1;
            var existingCart = new Cart
            {
                CartId = 101,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    CartItemId = 201,
                    CartId = 101,
                    MenuItemId = 301,
                    Quantity = 2,
                    PriceAtPurchase = 120
                }
            };

            var menuItem = new MenuItem
            {
                Id = 301,
                Name = "Pizza"
            };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { existingCart });
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(cartItems);
            _menuItemRepoMock.Setup(r => r.GetById(301)).ReturnsAsync(menuItem);

            // Act
            var result = await _service.GetCartByCustomerId(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.CartId);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual("Pizza", result.Items.First().MenuItemName);
            Assert.AreEqual(2, result.Items.First().Quantity);
        }

        [Test]
        public async Task GetCartByCustomerId_CartDoesNotExist_CreatesAndReturnsEmptyCart()
        {
            // Arrange
            int customerId = 2;
            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart>());
            _cartRepoMock.Setup(r => r.Add(It.IsAny<Cart>()))
                .ReturnsAsync((Cart c) =>
                {
                    c.CartId = 999; // Simulate DB-generated ID
                    return c;
                });

            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<CartItem>());

            // Act
            var result = await _service.GetCartByCustomerId(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customerId, result.CustomerId);
            Assert.AreEqual(999, result.CartId);
            Assert.AreEqual(0, result.Items.Count);
        }

        [Test]
        public async Task ClearCart_CartExists_RemovesAllItemsAndReturnsTrue()
        {
            // Arrange
            int customerId = 1;
            var cart = new Cart { CartId = 10, CustomerId = customerId };
            var cartItems = new List<CartItem>
        {
            new CartItem { CartItemId = 101, CartId = 10 },
            new CartItem { CartItemId = 102, CartId = 10 }
        };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { cart });
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(cartItems);

            _cartItemRepoMock.Setup(x => x.Delete(It.IsAny<int>())).Verifiable();

            // Act
            var result = await _service.ClearCart(customerId);

            // Assert
            Assert.IsTrue(result);
            _cartItemRepoMock.Verify(r => r.Delete(101), Times.Once);
            _cartItemRepoMock.Verify(r => r.Delete(102), Times.Once);
        }

        [Test]
        public async Task ClearCart_CartDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int customerId = 2;
            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart>());

            // Act
            var result = await _service.ClearCart(customerId);

            // Assert
            Assert.IsFalse(result);
            _cartItemRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task AddCartItem_CartIsEmpty_AddsNewItem()
        {
            // Arrange
            int customerId = 1;
            var request = new CreateCartItemRequest { MenuItemId = 101, Quantity = 2 };
            var menuItem = new MenuItem { Id = 101, Name = "Pizza", Price = 150, RestaurantId = 1 };
            var cart = new Cart { CartId = 1, CustomerId = customerId };
            var addedCartItem = new CartItem
            {
                CartItemId = 201,
                MenuItemId = 101,
                Quantity = 2,
                PriceAtPurchase = 150
            };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { cart });
            _menuItemRepoMock.Setup(r => r.GetById(101)).ReturnsAsync(menuItem);
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<CartItem>());
            _cartItemRepoMock.Setup(r => r.Add(It.IsAny<CartItem>())).ReturnsAsync(addedCartItem);

            // Act
            var result = await _service.AddCartItem(customerId, request);

            // Assert
            Assert.AreEqual(201, result.CartItemId);
            Assert.AreEqual(101, result.MenuItemId);
            Assert.AreEqual("Pizza", result.MenuItemName);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(150, result.PriceAtPurchase);
        }

        [Test]
        public async Task AddCartItem_ItemExistsInCart_IncrementsQuantity()
        {
            // Arrange
            int customerId = 1;
            var request = new CreateCartItemRequest { MenuItemId = 101, Quantity = 2 };
            var menuItem = new MenuItem { Id = 101, Name = "Pizza", Price = 150, RestaurantId = 1 };
            var cart = new Cart { CartId = 1, CustomerId = customerId };
            var existingCartItem = new CartItem { CartItemId = 201, CartId = 1, MenuItemId = 101, Quantity = 1, PriceAtPurchase = 150 };
            var updatedCartItem = new CartItem { CartItemId = 201, MenuItemId = 101, Quantity = 3, PriceAtPurchase = 150 };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { cart });
            _menuItemRepoMock.Setup(r => r.GetById(101)).ReturnsAsync(menuItem);
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<CartItem> { existingCartItem });
            _cartItemRepoMock.Setup(r => r.Update(201, It.IsAny<CartItem>())).ReturnsAsync(updatedCartItem);

            // Act
            var result = await _service.AddCartItem(customerId, request);

            // Assert
            Assert.AreEqual(201, result.CartItemId);
            Assert.AreEqual(3, result.Quantity);
        }

        [Test]
        public void AddCartItem_MenuItemNotFound_ThrowsException()
        {
            // Arrange
            int customerId = 1;
            var request = new CreateCartItemRequest { MenuItemId = 999, Quantity = 1 };
            var cart = new Cart { CartId = 1, CustomerId = customerId };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { cart });
            _menuItemRepoMock.Setup(r => r.GetById(999)).ReturnsAsync((MenuItem)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddCartItem(customerId, request));
            Assert.AreEqual("MenuItem not found", ex.Message);
        }

        [Test]
        public void AddCartItem_DifferentRestaurant_ThrowsException()
        {
            // Arrange
            int customerId = 1;
            var request = new CreateCartItemRequest { MenuItemId = 102, Quantity = 1 };
            var cart = new Cart { CartId = 1, CustomerId = customerId };

            var existingMenuItem = new MenuItem { Id = 101, RestaurantId = 1 };
            var newMenuItem = new MenuItem { Id = 102, RestaurantId = 2 };

            var existingCartItem = new CartItem { MenuItemId = 101, CartId = 1 };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart> { cart });
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<CartItem> { existingCartItem });
            _menuItemRepoMock.Setup(r => r.GetById(101)).ReturnsAsync(existingMenuItem);
            _menuItemRepoMock.Setup(r => r.GetById(102)).ReturnsAsync(newMenuItem);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddCartItem(customerId, request));
            Assert.AreEqual("You can only add items from only one restaurant at a time.", ex.Message);
        }

        [Test]
        public async Task AddCartItem_NoCartExists_CreatesCartAndAddsItem()
        {
            // Arrange
            int customerId = 10;
            var request = new CreateCartItemRequest { MenuItemId = 201, Quantity = 1 };
            var newCart = new Cart { CartId = 999, CustomerId = customerId };
            var menuItem = new MenuItem { Id = 201, Name = "Dosa", Price = 100, RestaurantId = 5 };
            var addedItem = new CartItem { CartItemId = 777, MenuItemId = 201, Quantity = 1, PriceAtPurchase = 100 };

            _cartRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Cart>());
            _cartRepoMock.Setup(r => r.Add(It.IsAny<Cart>())).ReturnsAsync(newCart);
            _menuItemRepoMock.Setup(r => r.GetById(201)).ReturnsAsync(menuItem);
            _cartItemRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<CartItem>());
            _cartItemRepoMock.Setup(r => r.Add(It.IsAny<CartItem>())).ReturnsAsync(addedItem);

            // Act
            var result = await _service.AddCartItem(customerId, request);

            // Assert
            Assert.AreEqual(777, result.CartItemId);
        }

        [Test]
        public async Task UpdateCartItem_ValidInput_UpdatesAndReturnsResponse()
        {
            // Arrange
            int customerId = 1;
            int cartItemId = 10;
            var cartItemUpdate = new CartItemUpdate { Quantity = 5 };

            var cartItem = new CartItem
            {
                CartItemId = cartItemId,
                CartId = 100,
                MenuItemId = 200,
                Quantity = 2,
                PriceAtPurchase = 50
            };

            var cart = new Cart
            {
                CartId = 100,
                CustomerId = customerId
            };

            var menuItem = new MenuItem
            {
                Id = 200,
                Name = "Pizza",
                Price = 50
            };

            _cartItemRepoMock.Setup(r => r.GetById(cartItemId)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(r => r.GetById(cartItem.CartId)).ReturnsAsync(cart);
            _cartItemRepoMock.Setup(r => r.Update(cartItem.CartItemId, It.IsAny<CartItem>()))
                .ReturnsAsync((int id, CartItem ci) => ci);
            _menuItemRepoMock.Setup(r => r.GetById(menuItem.Id)).ReturnsAsync(menuItem);

            // Act
            var result = await _service.UpdateCartItem(customerId, cartItemId, cartItemUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(cartItemId, result.CartItemId);
            Assert.AreEqual(menuItem.Id, result.MenuItemId);
            Assert.AreEqual("Pizza", result.MenuItemName);
            Assert.AreEqual(5, result.Quantity); // updated quantity
        }

        [Test]
        public void UpdateCartItem_CartItemNotFound_ThrowsException()
        {
            // Arrange
            _cartItemRepoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((CartItem)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateCartItem(1, 10, new CartItemUpdate { Quantity = 2 }));

            Assert.That(ex.Message, Is.EqualTo("Cart item not found"));
        }

        [Test]
        public void UpdateCartItem_CartDoesNotBelongToCustomer_ThrowsException()
        {
            // Arrange
            var cartItem = new CartItem { CartItemId = 1, CartId = 5, MenuItemId = 3 };
            var cart = new Cart { CartId = 5, CustomerId = 999 }; // wrong customer

            _cartItemRepoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(r => r.GetById(cartItem.CartId)).ReturnsAsync(cart);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateCartItem(1, 1, new CartItemUpdate { Quantity = 2 }));

            Assert.That(ex.Message, Is.EqualTo("Cart does not belong to customer"));
        }

        [Test]
        public async Task RemoveCartItem_ValidCustomerAndCartItem_ReturnsTrue()
        {
            // Arrange
            int customerId = 1, cartItemId = 10;
            var cartItem = new CartItem { CartItemId = cartItemId, CartId = 5 };
            var cart = new Cart { CartId = 5, CustomerId = customerId };

            _cartItemRepoMock.Setup(repo => repo.GetById(cartItemId)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(repo => repo.GetById(cartItem.CartId)).ReturnsAsync(cart);
            _cartItemRepoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Verifiable();

            // Act
            var result = await _service.RemoveCartItem(customerId, cartItemId);

            // Assert
            Assert.IsTrue(result);
            _cartItemRepoMock.Verify(r => r.Delete(cartItemId), Times.Once);
        }

        [Test]
        public async Task RemoveCartItem_CartItemNotFound_ReturnsFalse()
        {
            // Arrange
            _cartItemRepoMock.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync((CartItem)null);

            // Act
            var result = await _service.RemoveCartItem(1, 999);

            // Assert
            Assert.IsFalse(result);
            _cartItemRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task RemoveCartItem_CartNotFound_ReturnsFalse()
        {
            // Arrange
            var cartItem = new CartItem { CartItemId = 10, CartId = 100 };
            _cartItemRepoMock.Setup(repo => repo.GetById(10)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(repo => repo.GetById(It.IsAny<int>())).Verifiable();

            // Act
            var result = await _service.RemoveCartItem(1, 10);

            // Assert
            Assert.IsFalse(result);
            _cartItemRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task RemoveCartItem_CartDoesNotBelongToCustomer_ReturnsFalse()
        {
            // Arrange
            var cartItem = new CartItem { CartItemId = 10, CartId = 200 };
            var cart = new Cart { CartId = 200, CustomerId = 99 }; // Different customer

            _cartItemRepoMock.Setup(repo => repo.GetById(10)).ReturnsAsync(cartItem);
            _cartRepoMock.Setup(repo => repo.GetById(200)).ReturnsAsync(cart);

            // Act
            var result = await _service.RemoveCartItem(1, 10);

            // Assert
            Assert.IsFalse(result);
            _cartItemRepoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetTotalCustomers_ShouldReturnCorrectCount()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new HotPotDbContext(options))
            {
                var customer1 = new Customer
                {
                    Name = "John Doe",
                    Gender = "Male",
                    Email = "john@example.com",
                    Phone = "1234567890",
                    Addresses = new List<CustomerAddress>
            {
                new CustomerAddress
                {
                    Label = "Home",
                    Street = "123 Main St",
                    City = "CityA",
                    Pincode = "11111"
                }
            }
                };

                var customer2 = new Customer
                {
                    Name = "Jane Doe",
                    Gender = "Female",
                    Email = "jane@example.com",
                    Phone = "0987654321",
                    Addresses = new List<CustomerAddress>
            {
                new CustomerAddress
                {
                    Label = "Work",
                    Street = "456 Office Rd",
                    City = "CityB",
                    Pincode = "22222"
                }
            }
                };

                context.Customers.AddRange(customer1, customer2);
                await context.SaveChangesAsync();

                var service = new CustomerService(
                    context,
                    new Mock<IRepository<string, User>>().Object,
                    new Mock<IRepository<int, Customer>>().Object,
                    new Mock<IRepository<int, Restaurant>>().Object,
                    new Mock<IRepository<int, MenuItem>>().Object,
                    new Mock<IRepository<int, Cart>>().Object,
                    new Mock<IRepository<int, CartItem>>().Object
                );

                var result = await service.GetTotalCustomers();

                Assert.AreEqual(2, result);
            }
        }

        [Test]
        public async Task GetAllCuisines_ShouldReturnAllCuisines()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new HotPotDbContext(options))
            {
                context.Cuisines.AddRange(
                    new Cuisine { Name = "Indian" },
                    new Cuisine { Name = "Chinese" }
                );
                await context.SaveChangesAsync();

                var service = new CustomerService(
                    context,
                    new Mock<IRepository<string, User>>().Object,
                    new Mock<IRepository<int, Customer>>().Object,
                    new Mock<IRepository<int, Restaurant>>().Object,
                    new Mock<IRepository<int, MenuItem>>().Object,
                    new Mock<IRepository<int, Cart>>().Object,
                    new Mock<IRepository<int, CartItem>>().Object
                );

                var result = await service.GetAllCuisines();

                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.Any(c => c.Name == "Indian"));
            }
        }

        [Test]
        public async Task GetAllMealTypes_ShouldReturnAllMealTypes()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new HotPotDbContext(options))
            {
                context.MealTypes.AddRange(
                    new MealType { Name = "Breakfast" },
                    new MealType { Name = "Dinner" }
                );
                await context.SaveChangesAsync();

                var service = new CustomerService(
                    context,
                    new Mock<IRepository<string, User>>().Object,
                    new Mock<IRepository<int, Customer>>().Object,
                    new Mock<IRepository<int, Restaurant>>().Object,
                    new Mock<IRepository<int, MenuItem>>().Object,
                    new Mock<IRepository<int, Cart>>().Object,
                    new Mock<IRepository<int, CartItem>>().Object
                );

                var result = await service.GetAllMealTypes();

                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.Any(m => m.Name == "Breakfast"));
            }
        }









    }


}


