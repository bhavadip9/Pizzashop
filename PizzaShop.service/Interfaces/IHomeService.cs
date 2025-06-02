using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.entity.ViewModel;


namespace PizzaShop.service.Interfaces
{
  public interface IHomeService
  {

    public ProfileViewModel ProfileGet();
    public ProfileViewModel ProfileUpdate(ProfileViewModel profile);
    public ProfileViewModel UpdateImage(ProfileViewModel profile);

    public bool ChangePass(ChangepasswordViewModel changePassword);


    public User GetOneByEmail(string email);

    Task<List<PermissionViewModel>> GetPermissionsForRoleAsynC(int roleId);
    Task SavePermissionsAsync(RoleAndPermissionViewModel model);


    public DashboardViewModel GetDashboard(int dateRange);

    public GraphDataVM GetDashboardGraph(int dateRange);
  }
}