
using Microsoft.EntityFrameworkCore;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class RoleAndPermissionRepo : IRoleAndPermissionRepo
    {
        private readonly NewPizzashopContext _context;

        public RoleAndPermissionRepo(NewPizzashopContext context)
        {
            _context = context;

        }

        public List<RolePermissionTable> getAllRole()
        {
            return _context.RolePermissionTables.ToList();
        }

        public RolePermissionTable? getAllPermisstion(int id)
        {

            var user = _context.RolePermissionTables.Find(id);
            return user;
        }

        public async Task<List<RolePermissionTable>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var result = await _context.RolePermissionTables.Where(rp => rp.Roles == roleId).ToListAsync();
            return result ?? new List<RolePermissionTable>();
        }

        public async Task UpdatePermissionsAsync(List<RolePermissionTable> updatedPermissions)
        {
            _context.RolePermissionTables.UpdateRange(updatedPermissions); // Bulk update
            await _context.SaveChangesAsync(); // Save changes to DB
        }
        public List<RolePermissionTable> GetPermissionByroleId(int id)
        {
            var rolePermissions = _context.RolePermissionTables.Where(c => c.RolesNavigation.RoleId == id).ToList();

            return rolePermissions;
        }

    }
}