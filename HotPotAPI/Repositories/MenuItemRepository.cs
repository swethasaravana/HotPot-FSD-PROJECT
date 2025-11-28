using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class MenuItemRepository : Repository<int, MenuItem>
    {
        private readonly HotPotDbContext _context;

        public MenuItemRepository(HotPotDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<MenuItem>> GetAll()
        {
            var items = await _context.MenuItems.ToListAsync();

            foreach (var item in items)
            {
                item.Cuisine = await _context.Cuisines.FindAsync(item.CuisineId);
                item.MealType = await _context.MealTypes.FindAsync(item.MealTypeId);
                item.Restaurant = await _context.Restaurants.FindAsync(item.RestaurantId);
            }

            return items;
        }

        public override async Task<MenuItem> GetById(int id)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
                throw new Exception($"MenuItem with ID {id} not found");

            // Manually load related entities
            menuItem.Cuisine = await _context.Cuisines.FindAsync(menuItem.CuisineId);
            menuItem.MealType = await _context.MealTypes.FindAsync(menuItem.MealTypeId);
            menuItem.Restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);

            return menuItem;
        }

        public async Task<IEnumerable<MenuItem>> GetByCuisineId(int cuisineId)
        {
            var items = await _context.MenuItems
            .Where(m => m.CuisineId == cuisineId)
            .ToListAsync();

            foreach (var item in items)
            {
                item.Cuisine = await _context.Cuisines.FindAsync(item.CuisineId);
                item.MealType = await _context.MealTypes.FindAsync(item.MealTypeId);
                item.Restaurant = await _context.Restaurants.FindAsync(item.RestaurantId);
            }

            return items;
        }

        public async Task<IEnumerable<MenuItem>> GetByMealTypeId(int mealTypeId)
        {
            var items = await _context.MenuItems
            .Where(m => m.MealTypeId == mealTypeId)
            .ToListAsync();

            foreach (var item in items)
            {
                item.Cuisine = await _context.Cuisines.FindAsync(item.CuisineId);
                item.MealType = await _context.MealTypes.FindAsync(item.MealTypeId);
                item.Restaurant = await _context.Restaurants.FindAsync(item.RestaurantId);
            }

            return items;
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItems()
        {
            var items = await _context.MenuItems
            .Where(m => m.IsAvailable)
            .ToListAsync();
            foreach (var item in items)
            {
                item.Cuisine = await _context.Cuisines.FindAsync(item.CuisineId);
                item.MealType = await _context.MealTypes.FindAsync(item.MealTypeId);
                item.Restaurant = await _context.Restaurants.FindAsync(item.RestaurantId);
            }

            return items;
        }
    }
}
