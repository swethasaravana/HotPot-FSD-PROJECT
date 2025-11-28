using HotPotAPI.Contexts;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HotPotAPI.Services
{
    public class DeliveryPartnerService : IDeliveryPartnerService
    {
        private readonly HotPotDbContext _context;
        private readonly IRepository<int, DeliveryPartner> _partnerRepository;
        private readonly IRepository<string, User> _userRepository;

        public DeliveryPartnerService(HotPotDbContext context, IRepository<int, DeliveryPartner> partnerRepository, IRepository<string, User> userRepository)
        {
            _context = context;
            _partnerRepository = partnerRepository;
            _userRepository = userRepository;
        }

        public async Task<CreateDeliveryPartnerResponse> AddDeliveryPartner(CreateDeliveryPartnerRequest request)
        {
            using var hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            var user = MapToUser(request, passwordHash, hmac.Key);
            var userResult = await _userRepository.Add(user);

            if (userResult == null)
                throw new Exception("Failed to create user");

            var partner = MapToDeliveryPartner(request);
            partner.Username = user.Username;

            var partnerResult = await _partnerRepository.Add(partner);
            if (partnerResult == null)
                throw new Exception("Failed to create delivery partner");

            return new CreateDeliveryPartnerResponse { PartnerId = partnerResult.DeliveryPartnerId };
        }

        private DeliveryPartner MapToDeliveryPartner(CreateDeliveryPartnerRequest request)
        {
            return new DeliveryPartner
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                VehicleNumber = request.VehicleNumber
            };
        }

        private User MapToUser(CreateDeliveryPartnerRequest request, byte[] hash, byte[] key)
        {
            return new User
            {
                Username = request.Email,
                Password = hash,
                HashKey = key,
                Role = "DeliveryPartner"
            };
        }

        public async Task<DeliveryPartner?> UpdateDeliveryPartner(int partnerId, DeliveryPartnerUpdate updatedData)
        {
            var existing = await _partnerRepository.GetById(partnerId);
            if (existing == null) return null;

            existing.FullName = updatedData.FullName;
            existing.Email = updatedData.Email;
            existing.Phone = updatedData.Phone;
            existing.VehicleNumber = updatedData.VehicleNumber;
            existing.IsAvailable = updatedData.IsAvailable;

            return await _partnerRepository.Update(partnerId, existing);
        }

        public async Task<bool> DeleteDeliveryPartner(int partnerId)
        {
            var existing = await _partnerRepository.GetById(partnerId);
            if (existing == null) return false;

            if (!existing.IsAvailable)
                throw new InvalidOperationException("Cannot delete. Delivery Partner is currently assigned to orders.");

            // Nullify DeliveryPartnerId in orders before delete (FK must allow nulls)
            var ordersWithPartner = await _context.Orders
                .Where(o => o.DeliveryPartnerId == partnerId)
                .ToListAsync();

            foreach (var order in ordersWithPartner)
            {
                order.DeliveryPartnerId = null;
            }

            await _context.SaveChangesAsync();

            // Now delete the partner
            await _partnerRepository.Delete(partnerId);
            return true;
        }

        public async Task<DeliveryPartner> GetById(int id)
        {
            return await _partnerRepository.GetById(id);
        }

        public async Task<DeliveryPartner> GetDeliveryPartnerById(int partnerId)
        {
            var partner = await _partnerRepository.GetById(partnerId);
            if (partner == null)
                throw new Exception("Delivery partner not found");

            return partner;
        }

        public async Task<DeliveryPartner> Update(int id, DeliveryPartner entity)
        {
            return await _partnerRepository.Update(id, entity);
        }

        public async Task<List<DeliveryPartner>> GetAllDeliveryPartners()
        {
            var partners = await _partnerRepository.GetAll();
            return (List<DeliveryPartner>)partners;
        }

        public async Task<int> GetTotalDeliveryPartners()
        {
            return await _context.DeliveryPartners.CountAsync();
        }
    }
}
