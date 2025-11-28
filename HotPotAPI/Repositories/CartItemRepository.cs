using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class CartItemRepository : Repository<int, CartItem>
    {
        private readonly HotPotDbContext _dbContext;

        public CartItemRepository(HotPotDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<CartItem>> GetAll()
        {
            var items = await _dbContext.CartItems.ToListAsync();

            foreach (var item in items)
            {
                await _dbContext.Entry(item).Reference(ci => ci.MenuItem).LoadAsync();
            }

            return items;
        }

        public override async Task<CartItem> GetById(int id)
        {
            var item = await _dbContext.CartItems.SingleOrDefaultAsync(ci => ci.CartItemId == id);

            if (item != null)
            {
                await _dbContext.Entry(item).Reference(ci => ci.MenuItem).LoadAsync();
            }

            return item;
        }
    }
}
