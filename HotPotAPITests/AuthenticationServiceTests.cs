using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Moq;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<int, Customer>> _customerRepoMock;
        private Mock<IRepository<int, Admin>> _adminRepoMock;
        private Mock<IRepository<int, DeliveryPartner>> _partnerRepoMock;
        private Mock<IRepository<int, RestaurantManager>> _managerRepoMock;
        private Mock<ITokenService> _tokenServiceMock;

        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<string, User>>();
            _customerRepoMock = new Mock<IRepository<int, Customer>>();
            _adminRepoMock = new Mock<IRepository<int, Admin>>();
            _partnerRepoMock = new Mock<IRepository<int, DeliveryPartner>>();
            _managerRepoMock = new Mock<IRepository<int, RestaurantManager>>();
            _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthenticationService(
                _userRepoMock.Object,
                _customerRepoMock.Object,
                _adminRepoMock.Object,
                _partnerRepoMock.Object,
                _managerRepoMock.Object,
                _tokenServiceMock.Object);
        }

        [Test]
        public void Login_InvalidUsername_ThrowsUnauthorizedAccessException()
        {
            var loginRequest = new UserLoginRequest { Username = "invalid@example.com", Password = "123" };
            _userRepoMock.Setup(r => r.GetById(loginRequest.Username)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginRequest));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            var password = "correctpassword";
            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = "user@example.com",
                HashKey = hmac.Key,
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                Role = "Customer"
            };

            var loginRequest = new UserLoginRequest { Username = "user@example.com", Password = "wrongpassword" };

            _userRepoMock.Setup(r => r.GetById(loginRequest.Username)).ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginRequest));
        }

        [Test]
        public async Task Login_ValidCustomer_ReturnsLoginResponse()
        {
            var password = "password123";
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            var user = new User
            {
                Username = "cust@example.com",
                HashKey = hmac.Key,
                Password = passwordHash,
                Role = "Customer"
            };

            var customer = new Customer { Id = 1, Email = "cust@example.com", Name = "Test Customer" };

            _userRepoMock.Setup(r => r.GetById("cust@example.com")).ReturnsAsync(user);
            _customerRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Customer> { customer });
            _tokenServiceMock.Setup(t => t.GenerateToken(1, "Test Customer", "Customer")).ReturnsAsync("mock-token");

            var loginRequest = new UserLoginRequest { Username = "cust@example.com", Password = password };

            var result = await _authService.Login(loginRequest);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test Customer", result.Name);
            Assert.AreEqual("Customer", result.Role);
            Assert.AreEqual("mock-token", result.Token);
        }
    }
}
