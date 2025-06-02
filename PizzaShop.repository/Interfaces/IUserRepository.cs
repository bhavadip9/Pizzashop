
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface IUserRepository
    {
        public IQueryable<User> GetAll();

        public Task<List<User>> GetAllUser();
        public User GetOneByIdAsync(int id);
        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(int id);
        Task<User?> GetUser(String email, string password);

        Task<User> GetUserWithRoleAsync(int userId);

        public User GetOneByEmail(string email);
    }
}