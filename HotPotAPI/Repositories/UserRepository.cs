using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(HotPotDbContext context) : base(context) { }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.Include(u => u.Customer).ToListAsync();
        }

        public override async Task<User> GetById(string username)
        {
            return await _context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Username == username);
        }

    }

}
