using HotPotAPI.Contexts;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace HotPotAPI.Tests
{
    public class DeliveryPartnerRepositoryTests
    {
        private HotPotDbContext _context;
        private DeliveryPartnerRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HotPotDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HotPotDbContext(options);

            var user1 = new User
            {
                Username = "dp1",
                Password = Encoding.UTF8.GetBytes("pass1"),
                HashKey = Encoding.UTF8.GetBytes("key1"),
                Role = "DeliveryPartner"
            };

            var user2 = new User
            {
                Username = "dp2",
                Password = Encoding.UTF8.GetBytes("pass2"),
                HashKey = Encoding.UTF8.GetBytes("key2"),
                Role = "DeliveryPartner"
            };

            var partner1 = new DeliveryPartner
            {
                DeliveryPartnerId = 1,
                FullName = "Delivery One",
                Email = "dp1@example.com",
                Phone = "1111111111",
                VehicleNumber = "TN01AA1234",
                Username = "dp1",
                User = user1
            };

            var partner2 = new DeliveryPartner
            {
                DeliveryPartnerId = 2,
                FullName = "Delivery Two",
                Email = "dp2@example.com",
                Phone = "2222222222",
                VehicleNumber = "TN02BB5678",
                Username = "dp2",
                User = user2
            };

            _context.DeliveryPartners.AddRange(partner1, partner2);
            _context.SaveChanges();

            _repository = new DeliveryPartnerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectPartner()
        {
            var partner = await _repository.GetById(1);
            Assert.IsNotNull(partner);
            Assert.AreEqual("Delivery One", partner.FullName);
        }

        [Test]
        public void GetById_InvalidId_ShouldThrowException()
        {
            Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllPartners()
        {
            var partners = await _repository.GetAll();
            Assert.AreEqual(2, partners.Count());
        }

        [Test]
        public async Task Add_ShouldAddNewPartner()
        {
            var newUser = new User
            {
                Username = "dp3",
                Password = Encoding.UTF8.GetBytes("pass3"),
                HashKey = Encoding.UTF8.GetBytes("key3"),
                Role = "DeliveryPartner"
            };

            var newPartner = new DeliveryPartner
            {
                FullName = "Delivery Three",
                Email = "dp3@example.com",
                Phone = "3333333333",
                VehicleNumber = "TN03CC7890",
                Username = "dp3",
                User = newUser
            };

            var result = await _repository.Add(newPartner);
            Assert.IsNotNull(result);
            Assert.AreEqual("Delivery Three", result.FullName);

            var all = await _repository.GetAll();
            Assert.AreEqual(3, all.Count());
        }

        [Test]
        public async Task Update_ShouldModifyExistingPartner()
        {
            var updatedPartner = new DeliveryPartner
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                Phone = "0000000000",
                VehicleNumber = "TN00ZZ0000"
            };

            var result = await _repository.Update(1, updatedPartner);
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Name", result.FullName);
        }

        [Test]
        public async Task Update_InvalidId_ShouldReturnNull()
        {
            var result = await _repository.Update(99, new DeliveryPartner());
            Assert.IsNull(result);
        }

        [Test]
        public async Task Delete_ShouldRemovePartner()
        {
            var result = await _repository.Delete(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.DeliveryPartnerId);

            var all = await _repository.GetAll();
            Assert.AreEqual(1, all.Count());
        }

        [Test]
        public async Task Delete_InvalidId_ShouldReturnNull()
        {
            var result = await _repository.Delete(99);
            Assert.IsNull(result);
        }
    }
}
