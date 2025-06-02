
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface IloginRepository
    {
        IEnumerable<User> GetAll();
        User GetOneByEmail(string Email);
        public Task UpdatePassword(User user);


    }
}