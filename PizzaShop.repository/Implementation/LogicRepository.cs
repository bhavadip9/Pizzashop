
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class LogicRepository : IloginRepository
    {
        private readonly NewPizzashopContext _context;

        public LogicRepository(NewPizzashopContext context)
        {
            _context = context;
        }
        public IEnumerable<User> GetAll() => _context.Users;

        public User? GetOneByEmail(string email)
        {
            return _context.Users.SingleOrDefault(m => m.Email == email);
        }

        public async Task UpdatePassword(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        } 

    }
}