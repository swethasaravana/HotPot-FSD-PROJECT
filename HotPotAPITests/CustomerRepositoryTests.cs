using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPotAPI.Tests
{
    public class CustomerRepositoryTests
    {
        private HotPotDbContext _context;
        private CustomerRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Simulated password & hash
            var password1 = System.Text.Encoding.UTF8.GetBytes("pass123");
            var hash1 = System.Text.Encoding.UTF8.GetBytes("salt123");

            var password2 = System.Text.Encoding.UTF8.GetBytes("word456");
            var hash2 = System.Text.Encoding.UTF8.GetBytes("salt456");

            var user1 = new User
            {
                Username = "john123",
                Password = password1,
                HashKey = hash1,
                Role = "Customer"
            };

            var user2 = new User
            {
                Username = "jane456",
                Password = password2,
                HashKey = hash2,
                Role = "Customer"
            };

            var customer1 = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Gender = "Male",
                Email = "john@example.com",
                Phone = "1234567890",
                User = user1
            };

            var customer2 = new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Gender = "Female",
                Email = "jane@example.com",
                Phone = "9876543210",
                User = user2
            };

            _context.Customers.AddRange(customer1, customer2);
            _context.SaveChanges();

            _repository = new CustomerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ValidId_ReturnsCustomer()
        {
            var result = await _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [Test]
        public void GetById_InvalidId_ThrowsException()
        {
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _repository.GetById(999);
            });
        }

        [Test]
        public async Task GetAll_WithCustomers_ReturnsAllCustomers()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_NoCustomers_ThrowsException()
        {
            // Clear customers and save
            _context.Customers.RemoveRange(_context.Customers);
            _context.SaveChanges();

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _repository.GetAll();
            });
        }
    }
}
