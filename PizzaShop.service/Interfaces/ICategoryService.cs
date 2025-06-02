


using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;


namespace PizzaShop.service.Interfaces
{
  public interface ICategoryService
  {

    //Category Interface
    Task<List<AddCategoryViewModel>> GetAllCategory();

    public List<AddCategoryViewModel> GetAllCategoriesForItem();
    public Category AddCategoryAsync(AddCategoryViewModel model);
    public bool IsExistCategory(string categoryName);
    public AddCategoryViewModel EditCategory(int id);
    public bool CheckCategoryName(string categoryName, int id);
    public AddCategoryViewModel EditCategory(AddCategoryViewModel model);
    public bool DeleteCategory(int id);


    //Item Interface
    public bool AddItem(AddItemViewModel model);
    public List<AddItemViewModel> GetMenuItem(int categoryid);
    public Task<AddPaginationViewmodel<AddItemViewModel>> GetAllItem(int categoryId, int page, int pageSize, string search);

    public List<Category> GetAllCategoryList();
    public List<Unit> GetAllUnit();
    public AddItemViewModel GetEditItem(int id);
    public List<ModifierGroup> GetAllModifierGroup();

    public Task<List<AddModifierViewModel>> GetAllModifier(int modifierGroupId);

    public AddItemViewModel EditItem(AddItemViewModel model);

    public void DeleteItem(int id);

    public void DeleteManyItem(List<int> Ids);


    //Modifier Group
    public Task<AddPaginationViewmodel<AddModifierViewModel>> GetAllModifier(int modifierGroupId, int page, int pageSize, string search);

    public Task<List<AddModifierGroupViewModel>> GetAllModifierGroupsidebar();

    public List<AddModifierGroupViewModel> GetAllMOdifierForItem();

    public List<ModifierGroup> GetAllModifierGroupList();

    public bool AddModifieritem(AddModifierViewModel model);

    public AddModifierViewModel GetEditModifierItem(int id);


    public bool EditModifierItem(AddModifierViewModel model);
    public void DeleteModifierItem(int id);


    public bool DeleteManyModifierItem(List<int> Ids);

    public List<AddModifierViewModel> GetAllExistingModifier();

    public Task AddModifierGroupAsync(AddModifierGroupViewModel model);

    public AddModifierGroupViewModel EditModifierGroup(int id);

    public void DeleteModifierGroup(int id);

    public Task EditModifierGroup(AddModifierGroupViewModel model);
  }
}