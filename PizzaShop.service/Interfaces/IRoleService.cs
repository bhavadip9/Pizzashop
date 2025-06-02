using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
    public interface IRoleService
    {

      
        public Role GetOneByIdAsync(int id);


         public string GetRoleName(int id);
    }
}