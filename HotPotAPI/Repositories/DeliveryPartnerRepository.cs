using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;
using HotPotAPI.Interfaces;

namespace HotPotAPI.Repositories
{
    public class DeliveryPartnerRepository : IRepository<int, DeliveryPartner>
    {
        private readonly HotPotDbContext _context;

        public DeliveryPartnerRepository(HotPotDbContext context)
        {
            _context = context;
        }

        // Get DeliveryPartner by ID
        public async Task<DeliveryPartner> GetById(int id)
        {
            var partner = await _context.DeliveryPartners
                .Include(dp => dp.User)  // Include related user information if necessary
                .SingleOrDefaultAsync(dp => dp.DeliveryPartnerId == id);

            if (partner == null)
                throw new Exception($"Delivery Partner with ID {id} not found");

            return partner;
        }

        // Get all DeliveryPartners
        public async Task<IEnumerable<DeliveryPartner>> GetAll()
        {
            var partners = await _context.DeliveryPartners
                .Include(dp => dp.User)  // Include related user information if necessary
                .ToListAsync();

            if (partners.Count == 0)
                throw new Exception("No delivery partners found");

            return partners;
        }

        // Add a new DeliveryPartner
        public async Task<DeliveryPartner> Add(DeliveryPartner entity)
        {
            await _context.DeliveryPartners.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // Update an existing DeliveryPartner
        public async Task<DeliveryPartner> Update(int id, DeliveryPartner entity)
        {
            var existingPartner = await _context.DeliveryPartners
                .SingleOrDefaultAsync(dp => dp.DeliveryPartnerId == id);

            if (existingPartner == null)
                return null;

            // Update the existing partner's fields
            existingPartner.FullName = entity.FullName;
            existingPartner.Email = entity.Email;
            existingPartner.Phone = entity.Phone;
            existingPartner.VehicleNumber = entity.VehicleNumber;

            _context.DeliveryPartners.Update(existingPartner);
            await _context.SaveChangesAsync();
            return existingPartner;
        }

        // Delete a DeliveryPartner
        public async Task<DeliveryPartner> Delete(int id)
        {
            var partner = await _context.DeliveryPartners
                .SingleOrDefaultAsync(dp => dp.DeliveryPartnerId == id);

            if (partner == null)
                return null;

            _context.DeliveryPartners.Remove(partner);
            await _context.SaveChangesAsync();
            return partner;
        }
    }
}
