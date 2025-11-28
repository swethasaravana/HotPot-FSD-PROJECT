using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class CartRepository : Repository<int, Cart>
    {
        private readonly HotPotDbContext _dbContext;

        public CartRepository(HotPotDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Cart>> GetAll()
        {
            var carts = await _dbContext.Carts.ToListAsync();
            foreach (var cart in carts)
            {
                _dbContext.Entry(cart).Collection(c => c.CartItems).Load();

                foreach (var item in cart.CartItems)
                {
                    _dbContext.Entry(item).Reference(ci => ci.MenuItem).Load();
                }
            }
            return carts;
        }

        public override async Task<Cart> GetById(int id)
        {
            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.CartId == id);
            if (cart != null)
            {
                await _dbContext.Entry(cart).Collection(c => c.CartItems).LoadAsync();

                foreach (var item in cart.CartItems)
                {
                    await _dbContext.Entry(item).Reference(ci => ci.MenuItem).LoadAsync();
                }
            }
            return cart;
        }

        public async Task<Cart> GetCartByUserId(int customerid)
        {
            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerid);
            if (cart != null)
            {
                await _dbContext.Entry(cart).Collection(c => c.CartItems).LoadAsync();

                foreach (var item in cart.CartItems)
                {
                    await _dbContext.Entry(item).Reference(ci => ci.MenuItem).LoadAsync();
                }
            }
            return cart;
        }
    }

}
