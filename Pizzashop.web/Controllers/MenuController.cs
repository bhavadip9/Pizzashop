namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using PizzaShop.service.Implementation;
using Pizzashop.entity.ViewModels;
using Newtonsoft.Json;

[ServiceFilter(typeof(PermissionFilter))]
[CustomAuthorize("Admin", "Manager")]
public class MenuController : Controller
{

    private readonly IUploadService _uploadService;

    private readonly ICategoryService _categoryService;

    public MenuController(IUploadService uploadService, ICategoryService categoryService)
    {
        _uploadService = uploadService;
        _categoryService = categoryService;
    }





    /// <summary>
    /// Menu Page
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> MenuPage()
    {
        try
        {
            var categories = await _categoryService.GetAllCategory();
            var ModifierGroup = await _categoryService.GetAllModifierGroupsidebar();

            var viewModel = new MenuListViewModel
            {
                Category = categories.Select(p => new AddCategoryViewModel
                {
                    CategoryId = p.CategoryId,
                    CategoryName = p.CategoryName,
                    Description = p.Description
                }).ToList(),
                ModifierGroup = ModifierGroup.Select(p => new AddModifierGroupViewModel
                {
                    ModifierGroupId = p.ModifierGroupId,
                    ModifierName = p.ModifierName,
                    Descriptionmodifier = p.Descriptionmodifier

                }).ToList(),
            };

            return View("MenuPage", viewModel);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in MenuPage: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId, int page, int pageSize, string search)
    {
        try
        {
            var items = await _categoryService.GetAllItem(categoryId, page, pageSize, search);
            return PartialView("ItemSidebar", items);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in GetItemsByCategory: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }



    [HttpGet]
    public async Task<IActionResult> GetModifierItem(int modifierGroupId)
    {
        try
        {
            var items = await _categoryService.GetAllModifier(modifierGroupId);
            return Json(items);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error : {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }
    #region Menu Category

    [HttpGet]
    public IActionResult GetAllCategory()
    {
        try
        {
            var category = _categoryService.GetAllCategoryList();
            return Json(category);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error : {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }

    }

    [HttpGet]
    public IActionResult GetAllUnit()
    {
        var unit = _categoryService.GetAllUnit();
        return Json(unit);
    }

    [HttpGet]
    public IActionResult GetAllModifier()
    {
        var modifier = _categoryService.GetAllModifierGroup();
        return Json(modifier);
    }

    /// <summary>
    /// Add Category Get Method
    /// </summary>
    /// <returns></returns>

    [HttpGet]
    public IActionResult AddCategory()
    {
        return PartialView("_AddCategoryPV");
    }

    /// <summary>
    /// Add Category Post Method
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>

    [HttpPost]
    public IActionResult AddCategory(AddCategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
               // TempData["error"] = "Model is not valid !";
                return Json(new { success = false , message = "Model is not valid !" });

            }

            var isCategoryExist = _categoryService.IsExistCategory(model.CategoryName);
            if (isCategoryExist)
            {
                // TempData["error"] = "Category Already exist !";
                return Json(new { success = false, message = "Category Already exist !" });

            }

            var result = _categoryService.AddCategoryAsync(model);
            if (result != null)
            {
                TempData["success"] = "Successfully, Add Category";
                return Json(new { success = true, result, message = "Category added successfully." });
            }
            else
            {
                TempData["error"] = "Something with wrong try again!";
                return Json(new { success = false, message = "Something with wrong try again!" });

            }

        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new category.";
            return View("Error", ex);
        }
    }


    /// <summary>
    /// Edit Category
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        try
        {
            if (id == null)
            {
                TempData["error"] = "Category Is Not Found";
                return NotFound();
            }
            var categories = _categoryService.EditCategory(id);
            return RedirectToAction("MenuPage");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while editing the category.";
            return View("Error", ex);
        }
    }



    [HttpPost]
    public async Task<IActionResult> EditCategory(AddCategoryViewModel model)
    {
        try
        {
            if (model == null)
            {
                return NotFound();
            }
            var isCategoryExist = _categoryService.CheckCategoryName(model.CategoryName, model.CategoryId);
            //  var isCategoryExist = isExist.Any(c => c.CategoryName.ToLower() == model.CategoryName.ToLower() && c.CategoryId != model.CategoryId);
            if (isCategoryExist)
            {
                TempData["error"] = "Category Already exist !";
                return RedirectToAction("MenuPage");
            }

            var category = _categoryService.EditCategory(model);

            if (category == null)
            {
                return NotFound();
            }
            TempData["success"] = "Successfully, Update Category";
            return RedirectToAction("MenuPage");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the category.";
            return View("Error", ex);
        }
    }


    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        try
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid category ID.";
                return Json(new { success = false, message = "Invalid category ID." });

                // return RedirectToAction("MenuPage");

            }

            var result = _categoryService.DeleteCategory(id);
            if (result == true)
            {
                TempData["success"] = "Successfully, Delete Category";
                return Json(new { success = true, message = "Successfully, Delete Category" });

                // return RedirectToAction("Menupage");
            }
            else
            {
                TempData["error"] = "Error, Delete Category";
                return Json(new { success = false, message = "Error, Delete Category" });
                // return RedirectToAction("Menupage");
            }

        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the category.";
            return Json(new { success = true, message = "An error occurred while deleting the category" });

            // return View("Error", ex);
        }

    }

    #endregion

    #region Menu Item


    [HttpGet]
    public IActionResult AddItem()
    {
        return PartialView("_AddItemPM");
    }



    [HttpPost]
    public IActionResult AddItem(AddItemViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
              //  TempData["error"] = "Model is not valid !";
                return Json(new { success = false, message = "Model is not valid !" });

            }
            var GetAllCategory = _categoryService.GetMenuItem(model.CategoryId);
            var isItemExist = GetAllCategory.Any(c => c.ItemName.ToLower() == model.ItemName.ToLower());

            if (isItemExist)
            {
                return Json(new { success = false, message = "Item Already exist in this Category !" });
            }
            else
            {
                List<DataItem> modifierGroups = JsonConvert.DeserializeObject<List<DataItem>>(model.SelectedModifierGroupJson);
                model.ModifierGroups = modifierGroups;

                if (model.FormFile != null)
                {
                    var path = _uploadService.Upload(Image: model.FormFile, folder_name: model.ItemName);
                    model.Image = $"{Request.Scheme}://{Request.Host}/{path}";
                }
                else
                {
                    model.Image = $"{Request.Scheme}://{Request.Host}/images/dining-menu.png";
                }

                var result = _categoryService.AddItem(model);
                if (result == true)
                {
                    TempData["success"] = "New item added successfully!";
                    return Ok(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Something with wrong try again.!" });
                }
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> _EditItemPV(int id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }
            var categories = _categoryService.GetEditItem(id);
            return Json(categories);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while editing the item.";
            return View("Error", ex);
        }


    }

    [HttpPost]
    public async Task<IActionResult> _EditItemPV(AddItemViewModel model)
    {
        try
        {
            if (model == null)
            {
                return NotFound();
            }
            var isExist = _categoryService.GetMenuItem(model.CategoryId);
            var isItemExist = isExist.Any(c => c.ItemName.ToLower() == model.ItemName.ToLower() && c.ItemId != model.ItemId);
            if (isItemExist)
            {
                return Json(new { success = false, message = "Item Already exist in this Category !" });
                // TempData["error"] = "Item Already exist in this Category !";
                // return RedirectToAction("MenuPage");
            }
            if (model.SelectedModifierGroupJsonEdit != null)
            {
                List<DataItem> modifierGroups = JsonConvert.DeserializeObject<List<DataItem>>(model.SelectedModifierGroupJsonEdit);
                model.ModifierGroups = modifierGroups;
            }

            if (model.FormFile != null)
            {
                var path = _uploadService.Upload(Image: model.FormFile, folder_name: model.ItemName);
                model.Image = $"{Request.Scheme}://{Request.Host}/{path}";
            }

            var item = _categoryService.EditItem(model);

            TempData["success"] = "Successfully, Update Item";
            return RedirectToAction("MenuPage");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the item.";
            return View("Error", ex);
        }


    }


    [HttpPost]
    public IActionResult _DeleteItemPV(int id)
    {
        try
        {
            if (id == null)
            {
                TempData["error"] = "Item is not Found";
                return NotFound();
            }
            _categoryService.DeleteItem(id);
            TempData["success"] = "Successfully, Delete Category Item ";
            return RedirectToAction("Menupage");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the item.";
            return View("Error", ex);
        }

    }


    [HttpGet]
    public IActionResult DeleteManyItemGet()
    {
        try
        {
            return PartialView("_DeleteManyItem");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the item.";
            return View("Error", ex);
        }

    }
    [HttpPost]
    public IActionResult DeleteManyItem(List<int> Ids)
    {
        try
        {
            _categoryService.DeleteManyItem(Ids);
            TempData["success"] = "Successfully, Many Item";
            return RedirectToAction("Menupage");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the item.";
            return View("Error", ex);
        }

    }

    #endregion


    #region ModifierGroup

    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(AddModifierGroupViewModel model, string SelectedModifierIds, string selectedModifierName)
    {
        var modifierGroups = _categoryService.GetAllModifierGroupList();
        var isModifierGroupExist = modifierGroups.Any(c => c.GroupName.ToLower() == model.ModifierName.ToLower());


        if (isModifierGroupExist)
        {
            TempData["error"] = "ModifierGroup Already exist !";
            return RedirectToAction("MenuPage");
        }
        if (!string.IsNullOrEmpty(SelectedModifierIds))
        {
            model.SelectedModifierIds = SelectedModifierIds.Split(',').Select(int.Parse).ToList();
            model.SelectedModifierName = selectedModifierName.Split(',').ToList();
        }

        _categoryService.AddModifierGroupAsync(model);

        TempData["succses"] = "Add Modifier Group";
        return RedirectToAction("Menupage");
    }

    [HttpGet]
    public IActionResult AddExistingModifier(int page = 1, int pageSize = 2, string search = "")
    {
        try
        {
            var modifiers = _categoryService.GetAllExistingModifier();

            if (modifiers == null)
            {
                return Json(new { data = new List<object>(), totalItems = 0, page, pageSize });
            }


            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                modifiers = modifiers.Where(m => m.ModifierName.ToLower().Contains(search)).ToList();
            }

            var totalItems = modifiers.Count();

            var pagedModifiers = modifiers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new { data = pagedModifiers, totalItems, page, pageSize });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddExistingModifier: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult EditExistingModifier(int page = 1, int pageSize = 2, string search = "")
    {
        try
        {
            var modifiers = _categoryService.GetAllExistingModifier();

            if (modifiers == null)
            {
                return Json(new { data = new List<object>(), totalItems = 0, page, pageSize });
            }


            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                modifiers = modifiers.Where(m => m.ModifierName.ToLower().Contains(search)).ToList();
            }

            var totalItems = modifiers.Count();

            var pagedModifiers = modifiers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new { data = pagedModifiers, totalItems, page, pageSize });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddExistingModifier: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }


    [HttpGet]
    public async Task<IActionResult> _EditModifierGroupPV(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var modifierGroup = _categoryService.EditModifierGroup(id);
        return Json(modifierGroup);

    }



    [HttpPost]
    public async Task<IActionResult> EditModifierGroup(AddModifierGroupViewModel model)
    {
        if (model == null)
        {
            return NotFound();
        }
        var isExist = _categoryService.GetAllModifierGroupList();
        var isModifierGroupExist = isExist.Any(c => c.GroupName.ToLower() == model.ModifierName.ToLower() && c.ModifierGroupId != model.ModifierGroupId);
        if (isModifierGroupExist)
        {
            TempData["error"] = "ModifierGroup Already exist !";
            return RedirectToAction("MenuPage");
        }
        if (model.SelectedModifierJsonEdit != null)
        {
            List<ItemSelected> modifierGroups = JsonConvert.DeserializeObject<List<ItemSelected>>(model.SelectedModifierJsonEdit);
            model.ModifierItems = modifierGroups;
        }

        var modifiergroup = _categoryService.EditModifierGroup(model);

        if (modifiergroup == null)
        {
            return NotFound();
        }
        TempData["success"] = "Successfully, Update ModifierGroup";
        return RedirectToAction("MenuPage");

    }

    [HttpGet]
    public IActionResult _DeleteModifierGroup(int id)
    {
        try
        {
            var ModelModifier = new AddModifierGroupViewModel();
            ModelModifier.ModifierGroupId = id;
            return PartialView("_DeleteModifierGroupPV", ModelModifier);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the ModifierGroup.";
        }

        return Json(new { success = false });

    }

    [HttpPost]
    public IActionResult _DeleteModifierGroupPV(int id)
    {
        try
        {
            if (id == null)
            {
                TempData["error"] = "ModifierGroup Not Delete ";
            }

            _categoryService.DeleteModifierGroup(id);
            TempData["success"] = "Successfully, Delete ModifierGroup";

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the ModifierGroup.";
        }

        return Json(new { success = false });

    }




    #endregion




    #region Modifier

    [HttpGet]
    public IActionResult AddModifier()
    {
        return PartialView("_AddModifierItemPV");
    }

    [HttpPost]
    public async Task<IActionResult> AddModifier(AddModifierViewModel model)
    {
        // var IsModifierNameExist = _categoryService.CheckModifierNAme(model.ModifierName, model.SelectedModifierIds);
        if (model == null)
        {
            TempData["error"] = "Exception occurred while adding new item.";
            return RedirectToAction("Menupage");
        }
        var result = _categoryService.AddModifieritem(model);
        if (result == true)
        {
            return Json(new { success = true, message = "Add new Modifier" });
        }
        // TempData["succses"] = "Add new Modifier";
        // return RedirectToAction("MenuPage");
        return Json(new { success = false, message = "Something with wrong try again.!" });
    }


    [HttpGet]
    public async Task<IActionResult> GetItemsByModifierGroup(int modifierGroupId, int page, int pageSize, string search)
    {
        try
        {
            var items = await _categoryService.GetAllModifier(modifierGroupId, page, pageSize, search);
            return PartialView("ModifierSidebar", items);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in GetItemsByCategory: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }


    [HttpGet]
    public IActionResult GetAllModifierGroup()
    {

        var modifierGroups = _categoryService.GetAllModifierGroupList();
        return Json(modifierGroups);
    }

    [HttpGet]
    public IActionResult AddModifierItem()
    {
        return PartialView("_AddModifieritemPV");
    }

    [HttpGet]
    public async Task<IActionResult> _EditModifierItemPV(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categories = _categoryService.GetEditModifierItem(id);
        return Json(categories);
    }

    [HttpPost]
    public async Task<IActionResult> _EditModifierItemPV(AddModifierViewModel model)
    {
        if (model == null)
        {
            return NotFound();
        }

        var item = _categoryService.EditModifierItem(model);
        if (item == true)
        {
            return Json(new { success = true, message = "Successfully, Update Modifier" });
        }
        else
        {
            return Json(new { success = false, message = "Something with wrong try again.!" });
        }


        // TempData["success"] = "Successfully, Update Modifier";
        // return RedirectToAction("MenuPage");

    }

    [HttpPost]
    public IActionResult _DeleteModiferItemPV(int id)
    {
        if (id == null)
        {
            TempData["error"] = "Error In Delete Modifier";
            return RedirectToAction("MenuPage");
        }

        _categoryService.DeleteModifierItem(id);
       // return Json(new { success = true, message = "Successfully, Delete Modifier" });
        TempData["success"] = "Successfully, Delete Modifier";
        return RedirectToAction("MenuPage");
    }

    [HttpGet]
    public IActionResult DeleteManyModifierGet()
    {
        try
        {
            return PartialView("_DeleteManyModifierItem");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the item.";
            return View("Error", ex);
        }

    }

    public IActionResult DeleteManyModifierItem(List<int> Ids)
    {
        try
        {
            var result = _categoryService.DeleteManyModifierItem(Ids);
            if (result == true)
            {
                TempData["success"] = "Successfully, Many Modifier";
                return RedirectToAction("Menupage");
            }
            else
            {
                TempData["success"] = "Successfully, Many Modifier";
                return RedirectToAction("Menupage");
            }

        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the item.";
            return View("Error", ex);
        }
    }




    #endregion

}