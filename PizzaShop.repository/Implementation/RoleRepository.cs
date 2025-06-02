
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class RoleRepository : IRoleRepository
    {

        private readonly NewPizzashopContext _context;

        public RoleRepository(NewPizzashopContext context)
        {
            _context = context;
        }

        public IEnumerable<Role> GetAll() => _context.Roles;

        public List<Role> GetAllRole()
        {
            return _context.Roles.ToList();
        }

        public Role? GetRole(int id)
        {

            var RoleId = _context.Roles.FirstOrDefault(x => x.RoleId == id);
            return RoleId;
        }
        public Role? GetRoleName(string role)
        {

            var rolename = _context.Roles.FirstOrDefault(x => x.RoleName == role);
            return rolename;
        }

        public string GetRoleName(int id)
        {
            var role = _context.Roles.Find(id);

            if (role == null)
            {
                return "Role not found";
            }
            return role.RoleName;
        }
    }

}