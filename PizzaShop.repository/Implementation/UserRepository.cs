
using Microsoft.EntityFrameworkCore;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly NewPizzashopContext _context;

        public UserRepository(NewPizzashopContext context)
        {
            _context = context;
        }


        public IQueryable<User> GetAll()
        {
            return _context.Users.Where(c => !c.IsDelete).Include(u => u.RolesNavigation).AsQueryable();
        }




        public async Task<List<User>> GetAllUser()
        {
            try
            {
                var result = _context.Users.Include(c => c.RolesNavigation).Where(c => !c.IsDelete)
                                               .ToList();
                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }

        }
        public async Task<User> GetUserWithRoleAsync(int userId)
        {
            try
            {
                return await _context.Users.Where(c => !c.IsDelete)
                              .Include(u => u.RolesNavigation)
                              .FirstOrDefaultAsync(u => u.UserId == userId);

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }

        }

        public User? GetOneByIdAsync(int id)
        {
            var user = _context.Users.Where(c => !c.IsDelete).FirstOrDefault(m => m.UserId == id);
            return user;
        }
        public User GetOneByEmail(string email)
        {
            return _context.Users.Where(c => !c.IsDelete).FirstOrDefault(m => m.Email.ToLower() == email.ToLower())!;
        }


        public async Task<User?> GetUser(String email, string password)
        {

            var user = _context.Users.Where(c => !c.IsDelete).FirstOrDefault(x => x.Email == email && x.Password == password);
            return user;
        }

        public async Task AddAsync(User user)
        {
            _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                user.IsDelete = true;
                _context.Users.Update(user);
            }
            _context.SaveChanges();
        }

    }
}