
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
  public interface ICategoryRepository
  {
    Task<List<Category>> GetAllCategory();
    public Category AddCategory(Category category);
    public bool CheckCategoryName(string name);

    public Category EditCategory(Category cat);
    public bool CheckCategoryName(string name, int id);
    public List<AddCategoryViewModel> GetAllCategoriesForItem();
    public Category GetOneByIdAsync(int id);
    public bool DeleteAsync(int id);

    Task<List<MenuItem>> GetAllItem(int categoryId);

    public MenuItem AddItem(MenuItem menuItem);
    public MappingItemModifierGroup AddItemMapping(MappingItemModifierGroup modifier);

    public MenuItem EditItem(MenuItem item);

    public MenuItem GetOneItemByID(int id);

    public void DeleteItem(int id);
    public List<Category> GetAllCategoryLIst();

    public MenuItem GetItem(int id);
    public List<Unit> UnitGet();
    public List<ModifierGroup> GroupModifer();

    public Task<List<ModifiersItem>> GetAllModifier(int modifierId);

    public void DeleteItemAsync(int id);

    public bool DeletemanyItem(List<int> Ids);



    public List<MenuItem> GetMenuItem(int categoryid);

    public Task<string> GetModifierNameAsync(int modifierGroupId);

    public ModifierGroup AddModifierGroup(ModifierGroup modifier);

    public Task<List<ModifierGroup>> GetAllModofierGroup();

    public List<AddModifierGroupViewModel> GetAllModifierForItem();

    public List<ModifierGroup> GetAllModifierGroupLIst();
    public MappingModifierModifiergroup AddModifierMapping(MappingModifierModifiergroup modifier);

    public ModifiersItem AddModifieritem(ModifiersItem model);
    public ModifiersItem GetModifierItem(int id);

    public ModifiersItem EditModifierItem(ModifiersItem item);

    public void DeleteModifierItemAsync(int id);

    public List<MappingModifierModifiergroup> GetMappingByModifierId(int modifierGroupId);
    public bool DeletemanyModifierItem(List<int> Ids);

    public List<AddModifierViewModel> GetAllExistingModifier();

    public ModifierGroup GetOneModifierGroup(int id);

    public List<int> GetManySelectModifier(int id);

    public List<string> GetModifierNamesByIds(List<int> modifierIds);

    public void DeleteModifierGroup(int id);

    public List<int?> GetModifierGroupIds(int itemId);
    public List<MappingItemModifierGroup> GetMappingByItemId(int itemId);

    public void DeleteMappings(List<MappingItemModifierGroup> mappings);

    public void AddMapping(MappingItemModifierGroup mapping);

    public List<MappingModifierModifiergroup> GetMappingByGroupId(int modifierGroupId);
    public void DeleteMappingItrm(List<MappingModifierModifiergroup> mappings);
    public void AddItemMapping(MappingModifierModifiergroup mapping);

    public ModifierGroup EditModifierGroup(ModifierGroup item);
  }
}