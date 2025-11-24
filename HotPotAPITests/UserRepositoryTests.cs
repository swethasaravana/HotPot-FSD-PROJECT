using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private HotPotDbContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Seed user and customer with addresses
            var customer1 = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Gender = "Male",
                Email = "john@example.com",
                Phone = "9999999999",
                Addresses = new List<CustomerAddress>
                {
                    new CustomerAddress
                    {
                        AddressId = 1,
                        Label = "Home",
                        Street = "123 Main St",
                        City = "Chennai",
                        Pincode = "600001"
                    }
                }
            };

            var user1 = new User
            {
                Username = "john_doe",
                Password = Encoding.UTF8.GetBytes("password"),
                HashKey = Encoding.UTF8.GetBytes("hash"),
                Role = "Customer",
                Customer = customer1
            };

            var user2 = new User
            {
                Username = "jane_doe",
                Password = Encoding.UTF8.GetBytes("password2"),
                HashKey = Encoding.UTF8.GetBytes("hash2"),
                Role = "Admin"
            };

            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();

            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ShouldReturnAllUsersWithCustomers()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            var customerUser = result.FirstOrDefault(u => u.Username == "john_doe");
            Assert.IsNotNull(customerUser?.Customer);
            Assert.AreEqual("John Doe", customerUser.Customer.Name);
            Assert.IsNotNull(customerUser.Customer.Addresses);
            Assert.AreEqual(1, customerUser.Customer.Addresses.Count);
        }

        [Test]
        public async Task GetById_ExistingUsername_ShouldReturnUserWithCustomer()
        {
            var result = await _repository.GetById("john_doe");

            Assert.IsNotNull(result);
            Assert.AreEqual("john_doe", result.Username);
            Assert.IsNotNull(result.Customer);
            Assert.AreEqual("John Doe", result.Customer.Name);
        }

        [Test]
        public async Task GetById_NonExistingUsername_ShouldReturnNull()
        {
            var result = await _repository.GetById("non_existent");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_WhenNoUsersExist_ShouldReturnEmptyList()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

       
    }
}
