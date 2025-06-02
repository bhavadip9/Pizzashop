
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface IRoleAndPermissionRepo
    {
        public List<RolePermissionTable> getAllRole();
        public RolePermissionTable? getAllPermisstion(int id);
        Task<List<RolePermissionTable>> GetPermissionsByRoleIdAsync(int roleId);
        Task UpdatePermissionsAsync(List<RolePermissionTable> updatedPermissions);
        public List<RolePermissionTable> GetPermissionByroleId(int id);

    }
}