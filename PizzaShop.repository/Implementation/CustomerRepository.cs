
using Microsoft.EntityFrameworkCore;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly NewPizzashopContext _context;

        public CustomerRepository(NewPizzashopContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> GetAllCustomer()
        {
            try
            {
                var result = _context.Customers.Include(o => o.Orders).ToList();
                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }

        }

        public IQueryable<Customer> GetAllCustomerExport()
        {
            try

            {
                var query = _context.Customers.Include(o => o.Orders).AsQueryable();
                return query;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }
        }


        public Customer GetCustomer(int id)
        {
            try
            {
                var customer = _context.Customers
                    .Include(o => o.Feedbacks)

                    .Include(c => c.Orders)
                    .ThenInclude(o => o.Payments)
                    .Include(o => o.Orders)
                    .ThenInclude(o => o.OrderDetails)
                    // .ThenInclude(o => o.OrderTableMappings)
                    .FirstOrDefault(c => c.CustomerId == id);

                return customer!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }

        }

        public List<Customer> GetCustomersWithCompletedOrderByDate(DateTime fromdate)
        {
            try
            {
                List<Customer> customers = _context.Customers.Where(o => o.CreatedAt >= fromdate).Distinct().ToList();
                return customers;
            }
            catch (Exception ex)
            {
                return new List<Customer>();
            }
        }
    }

}