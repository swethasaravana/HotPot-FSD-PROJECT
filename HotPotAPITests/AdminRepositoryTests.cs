using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HotPotAPI.Tests
{
    public class AdminRepositoryTests
    {
        private HotPotDbContext _context;
        private AdminRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            // Seed the database
            _context.Admins.AddRange(
                new Admin { Id = 1, Name = "Admin One", Email = "admin1@test.com" },
                new Admin { Id = 2, Name = "Admin Two", Email = "admin2@test.com"}
            );
            _context.SaveChanges();

            _repository = new AdminRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ExistingId_ReturnsAdmin()
        {
            // Act
            var result = await _repository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Admin One", result.Name);
        }

        [Test]
        public async Task GetById_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_WhenAdminsExist_ReturnsAllAdmins()
        {
            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_WhenNoAdminsExist_ThrowsException()
        {
            // Arrange
            _context.Admins.RemoveRange(_context.Admins);
            _context.SaveChanges();

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll(), "No admins found");
        }
    }
}
