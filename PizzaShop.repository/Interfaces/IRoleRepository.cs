
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll();
        public List<Role> GetAllRole();
        Role? GetRole(int id);
        public Role? GetRoleName(string role);

        public string GetRoleName(int id);
    }
}