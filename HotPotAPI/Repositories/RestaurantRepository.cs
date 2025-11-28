using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class RestaurantRepository : Repository<int, Restaurant>
    {
        public RestaurantRepository(HotPotDbContext context) : base(context) { }

        public override async Task<IEnumerable<Restaurant>> GetAll()
        {
            return await _context.Restaurants.ToListAsync();
        }

        public async override Task<Restaurant> GetById(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                throw new Exception("Restaurant not found");
            return restaurant;
        }

    }
}
