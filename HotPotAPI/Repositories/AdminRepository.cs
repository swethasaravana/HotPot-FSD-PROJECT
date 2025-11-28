using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class AdminRepository : Repository<int, Admin>
    {
        public AdminRepository(HotPotDbContext context) : base(context)
        {
        }

        public override async Task<Admin> GetById(int key)
        {
            return await _context.Admins.FindAsync(key);
        }

        public override async Task<IEnumerable<Admin>> GetAll()
        {
            var admins = await _context.Admins.ToListAsync();
            if (!admins.Any())
                throw new Exception("No admins found");
            return admins;
        }
    }
}
