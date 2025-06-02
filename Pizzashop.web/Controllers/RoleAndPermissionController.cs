namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using PizzaShop.service.Implementation;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.ViewModel;

/// <summary>
/// Hendle Full Admin Accesee 
/// </summary>

[CustomAuthorize("Admin")]
public class RoleAndPermissionController : Controller
{
    private readonly IRoleService _roleService;
    private readonly IHomeService _homeService;

    public RoleAndPermissionController(IHomeService homeService, IRoleService roleService)
    {

        _homeService = homeService;
        _roleService = roleService;

    }



    #region RoleAndPermission page



    [HttpGet]
    public async Task<IActionResult> RoleAndPermission()
    {
        return View("RoleAndPermission");
    }

    #endregion



    #region Permission

    [HttpGet]
    public async Task<IActionResult> Permission(int roleId)
    {

        var permissions = await _homeService.GetPermissionsForRoleAsynC(roleId);
        var rolename = _roleService.GetRoleName(roleId);
        ViewBag.RoleName = rolename;
        var viewModel = new RoleAndPermissionViewModel
        {
            RoleId = roleId,
            Permissions = permissions.Select(p => new PermissionViewModel
            {
                RolePermissionId = p.RolePermissionId,
                PermissionName = p.PermissionName,
                CanView = p.CanView,
                CanAddEdit = p.CanAddEdit,
                CanDelete = p.CanDelete,

            }).ToList()
        };
        return View("Permission", viewModel);

    }


    [HttpPost]
    public async Task<IActionResult> SavePermissions(RoleAndPermissionViewModel model)
    {
        await _homeService.SavePermissionsAsync(model);
        TempData["success"] = "Successfully,Give Permission";
        return RedirectToAction("RoleAndPermission");
    }


    #endregion

}


