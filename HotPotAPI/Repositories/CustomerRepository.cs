using HotPotAPI.Contexts;
using HotPotAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Repositories
{
    public class CustomerRepository : Repository<int, Customer>
    {
        public CustomerRepository(HotPotDbContext context) : base(context) 
        {
        }
        public override async Task<Customer> GetById(int id)
        {
            var customer = await _context.Customers
                .SingleOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                throw new Exception($"Customer with ID {id} not found");

            return customer;
        }
        public async override Task<IEnumerable<Customer>> GetAll()
        {
            var customers = _context.Customers.Include(c => c.User);
            if (customers.Count() == 0)
                throw new Exception("No customers found");
            return customers;
        }
    }
}
