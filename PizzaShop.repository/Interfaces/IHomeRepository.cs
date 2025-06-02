
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface IHomeRepository
    {
        string? GetUserEmail();
        User? GetOneByEmail(string email);

        User? UpdateUser(User user);

        User? UpdatePass(User user, string NewPassword);

        Task<User> AddUser(ProfileViewModel user);

        public User? GetUser(int id);
        public RolePermissionTable? UpdatePermisson(RolePermissionTable permission);
    }
}