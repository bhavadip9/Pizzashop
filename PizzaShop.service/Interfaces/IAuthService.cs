
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
    public interface IAuthService
    {
         Task<User?> AuthenticateUser(string email, string password);
         Task<Role?> CheckRole(string role);

         
    }
}



