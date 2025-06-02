
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class RoleService: IRoleService
    {

        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }


        public Role GetOneByIdAsync(int id)
        {

            return _repository.GetRole(id);
        }
        public string GetRoleName(int id)
        {
            return _repository.GetRoleName(id);
        }

    }
}