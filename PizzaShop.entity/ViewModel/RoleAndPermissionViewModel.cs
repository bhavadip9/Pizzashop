using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.ViewModel;


namespace Pizzashop.entity.ViewModels;

    public class RoleAndPermissionViewModel
    {

     public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<PermissionViewModel> Permissions { get; set; }
    }
