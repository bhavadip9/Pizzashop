using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NewPizzashopContext _context;

        public CategoryRepository(NewPizzashopContext context)
        {
            _context = context;
        }

        #region  Category
        public async Task<List<Category>> GetAllCategory()
        {
            try
            {
                return await _context.Categories
                       .Where(c => !c.IsDeleted).OrderByDescending(o => o.CategoryId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        public List<Category> GetAllCategoryLIst()
        {
            try
            {
                return _context.Categories
                       .Where(c => !c.IsDeleted).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        public List<AddCategoryViewModel> GetAllCategoriesForItem()
        {
            try
            {
                return _context.Categories.Where(c => !c.IsDeleted)
                    .Select(c => new AddCategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public bool CheckCategoryName(string name)
        {
            try
            {
                var category = _context.Categories.Any(c => c.CategoryName.ToLower() == name.ToLower() && !c.IsDeleted);
                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool CheckCategoryName(string name, int id)
        {
            try
            {
                var category = _context.Categories.Any(c => c.CategoryName.ToLower() == name.ToLower() && c.CategoryId != id && !c.IsDeleted);
                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        public Category AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }

        }
        public Category EditCategory(Category cat)
        {
            try
            {
                _context.Categories.Update(cat);
                _context.SaveChanges();
                return cat;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public Category GetOneByIdAsync(int id)
        {
            try
            {
                return _context.Categories.FirstOrDefault(m => m.CategoryId == id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        // public void DeleteAsync(int id)
        // {
        //     try
        //     {
        //         var category = _context.Categories.Find(id);
        //         category.IsDeleted = true;
        //         _context.Categories.Update(category);

        //         //  _context.Categories.Remove(category);
        //         _context.SaveChanges();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //         return;
        //     }

        // }

        public bool DeleteAsync(int id)
        {
            try
            {
                var category = _context.Categories
                    .Include(c => c.MenuItems).ThenInclude(c => c.MappingItemModifierGroups)
                    .FirstOrDefault(c => c.CategoryId == id);

                if (category == null)
                    return false;

                foreach (var item in category.MenuItems)
                {
                    _context.MappingItemModifierGroups.RemoveRange(item.MappingItemModifierGroups);
                }
                _context.MenuItems.RemoveRange(category.MenuItems);

                category.IsDeleted = true;
                _context.Categories.Update(category);

                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion


        #region  Unit


        public List<Unit> UnitGet()
        {
            try
            {
                return _context.Units.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        #endregion

        #region ModifierGroup
        public List<ModifierGroup> GroupModifer()
        {
            try
            {
                return _context.ModifierGroups.Where(c => !c.IsDeleted).ToList();
                //return _context.ModifierGroups.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        public async Task<List<ModifiersItem>> GetAllModifier(int modifierGroupId)
        {
            try
            {
                var modifiers = _context.MappingModifierModifiergroups.Where(c => !c.IsDeleted).Where(c => c.ModifierGroupId == modifierGroupId).Select(c => c.ModifierId).ToList();
                var result = await _context.ModifiersItems.Where(c => !c.IsDeleted).Where(c => modifiers.Contains(c.ModifierId)).Include(c => c.MappingModifierModifiergroups).Include(c => c.Unit).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<string> GetModifierNameAsync(int modifierGroupId)
        {
            try
            {
                var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(p => p.ModifierGroupId == modifierGroupId);
                return modifierGroup?.GroupName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public ModifierGroup EditModifierGroup(ModifierGroup item)
        {
            try
            {
                _context.ModifierGroups.Update(item);
                _context.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        #endregion
        #region Item

        public MenuItem GetOneItemByID(int id)
        {
            try
            {
                return _context.MenuItems.FirstOrDefault(m => m.ItemId == id)!;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<MappingModifierModifiergroup> GetMappingByGroupId(int modifierGroupId)
        {
            return _context.MappingModifierModifiergroups.Where(m => m.ModifierGroupId == modifierGroupId).ToList();
        }
        public List<MappingModifierModifiergroup> GetMappingByModifierId(int modifierGroupId)
        {
            return _context.MappingModifierModifiergroups.Where(m => m.ModifierId == modifierGroupId).ToList();
        }
        public void DeleteMappingItrm(List<MappingModifierModifiergroup> mappings)
        {
            _context.MappingModifierModifiergroups.RemoveRange(mappings);
            _context.SaveChanges();
        }

        public void AddItemMapping(MappingModifierModifiergroup mapping)
        {
            _context.MappingModifierModifiergroups.Add(mapping);
            _context.SaveChanges();
        }



        public List<MappingItemModifierGroup> GetMappingByItemId(int itemId)
        {
            return _context.MappingItemModifierGroups.Where(m => m.ItemId == itemId).ToList();
        }

        public void DeleteMappings(List<MappingItemModifierGroup> mappings)
        {
            _context.MappingItemModifierGroups.RemoveRange(mappings);
            _context.SaveChanges();
        }

        public void AddMapping(MappingItemModifierGroup mapping)
        {
            _context.MappingItemModifierGroups.Add(mapping);
            _context.SaveChanges();
        }

        public List<MenuItem> GetMenuItem(int categoryid)
        {
            return _context.MenuItems.Where(c => !c.IsDeleted).Where(c => c.CategoryId == categoryid).ToList();
        }

        public async Task<List<MenuItem>> GetAllItem(int categoryId)
        {
            try
            {
                var result = await _context.MenuItems.Where(c => c.CategoryId == categoryId).Where(c => !c.IsDeleted).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new();
            }

        }

        public MenuItem AddItem(MenuItem menuItem)
        {
            try
            {
                _context.MenuItems.Add(menuItem);
                _context.SaveChanges();
                return menuItem;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }
        }

        public MappingItemModifierGroup AddItemMapping(MappingItemModifierGroup modifier)
        {
            try
            {
                _context.MappingItemModifierGroups.Add(modifier);
                _context.SaveChanges();
                return modifier;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }
        }

        public MenuItem EditItem(MenuItem item)
        {
            try
            {
                _context.MenuItems.Update(item);
                _context.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }



        public void DeleteItem(int id)
        {
            try
            {
                var item = _context.MenuItems.Find(id);

                if (item != null)
                {
                    item.IsDeleted = true;
                    _context.MenuItems.Update(item);
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
            }

        }



        public MenuItem GetItem(int id)
        {
            return _context.MenuItems
                .Include(m => m.MappingItemModifierGroups) // Include mapping table
                .ThenInclude(m => m.ModifierGroup) // Include ModifierGroup details
                .FirstOrDefault(m => m.ItemId == id)!;
        }

        public List<int?> GetModifierGroupIds(int itemId)
        {
            return _context.MappingItemModifierGroups
                .Where(m => m.ItemId == itemId)
                .Select(m => m.ModifierGroupId)
                .ToList();
        }



        public void DeleteItemAsync(int id)
        {
            var item = _context.MenuItems.Find(id);
            if (item == null)
            {
                return;
            }
            item.IsDeleted = true;
            _context.MenuItems.Update(item);
            //  _context.Categories.Remove(category);
            _context.SaveChanges();
        }


        public bool DeletemanyItem(List<int> Ids)
        {
            try
            {
                var itemsToDelete = _context.MenuItems.Where(item => Ids.Contains(item.ItemId)).ToList();

                foreach (var item in itemsToDelete)
                {
                    item.IsDeleted = true;
                }
                _context.MenuItems.UpdateRange(itemsToDelete);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return false;
            }

        }



        #endregion

        #region Modifier

        public ModifierGroup GetOneModifierGroup(int id)
        {
            var result = _context.ModifierGroups.Include(c => c.MappingModifierModifiergroups).ThenInclude(m => m.Modifier).FirstOrDefault(m => m.ModifierGroupId == id);
            return result!;
        }
        public List<int> GetManySelectModifier(int id)
        {
            return _context.MappingModifierModifiergroups.Where(s => s.ModifierGroupId.Equals(id)).Select(m => m.ModifierId).ToList();

        }
        public List<string> GetModifierNamesByIds(List<int> modifierIds)
        {
            try
            {
                return _context.ModifiersItems
                                .Where(m => modifierIds.Contains(m.ModifierId))
                                .Select(m => m.ModifierName)
                                .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<List<ModifierGroup>> GetAllModofierGroup()
        {
            try
            {
                return await _context.ModifierGroups
                       .Where(c => !c.IsDeleted).ToListAsync();

                // var result = await _context.ModifierGroups.ToListAsync();
                // return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }


        public ModifierGroup AddModifierGroup(ModifierGroup modifier)
        {
            try
            {
                _context.ModifierGroups.Add(modifier);
                _context.SaveChanges();
                return modifier;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public MappingModifierModifiergroup AddModifierMapping(MappingModifierModifiergroup modifier)
        {
            try
            {
                _context.MappingModifierModifiergroups.Add(modifier);
                _context.SaveChanges();
                return modifier;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }
        }

        public void DeleteModifierGroup(int id)
        {

            try
            {
                var modifier = _context.ModifierGroups.Include(c => c.MappingModifierModifiergroups).FirstOrDefault(c => c.ModifierGroupId == id); ;
                if (modifier == null)
                {
                    return;
                }
                foreach (var item in modifier.MappingModifierModifiergroups)
                {
                    item.IsDeleted = true;
                    _context.MappingModifierModifiergroups.Update(item);
                }
                modifier.IsDeleted = true;
                _context.ModifierGroups.Update(modifier);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
            }

        }



        #endregion

        #region ModifierItem

        public ModifiersItem AddModifieritem(ModifiersItem model)
        {
            try
            {
                _context.ModifiersItems.Add(model);
                _context.SaveChanges();
                return _context.ModifiersItems.Where(c => !c.IsDeleted).FirstOrDefault(c => c.ModifierName == model.ModifierName)!;

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null;
            }
        }

        public List<ModifierGroup> GetAllModifierGroupLIst()
        {
            try
            {
                return _context.ModifierGroups
                       .Where(c => !c.IsDeleted).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
        public List<AddModifierGroupViewModel> GetAllModifierForItem()
        {
            try
            {
                return _context.ModifierGroups.Where(c => !c.IsDeleted)
                    .Select(c => new AddModifierGroupViewModel
                    {
                        ModifierGroupId = c.ModifierGroupId,
                        ModifierName = c.GroupName
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public ModifiersItem GetModifierItem(int id)
        {
            return _context.ModifiersItems.Include(c => c.MappingModifierModifiergroups).ThenInclude(m => m.ModifierGroup)
                .FirstOrDefault(m => m.ModifierId == id);
        }

        public ModifiersItem EditModifierItem(ModifiersItem item)
        {
            _context.ModifiersItems.Update(item);
            _context.SaveChanges();
            return item;
        }

        public void DeleteModifierItemAsync(int id)
        {
            var item = _context.MappingModifierModifiergroups.Find(id);
            if (item == null)
            {
                return;
            }
            item.IsDeleted = true;
            _context.MappingModifierModifiergroups.Update(item);
            _context.SaveChanges();
        }


        public bool DeletemanyModifierItem(List<int> Ids)
        {
            try
            {
                foreach (var item in Ids)
                {
                    var Finditem = _context.ModifiersItems.Include(c => c.MappingModifierModifiergroups).FirstOrDefault(c => c.ModifierId == item);
                    if (Finditem == null)
                    {
                        return false;
                    }
                    foreach (var mod in Finditem.MappingModifierModifiergroups)
                    {
                        Finditem.MappingModifierModifiergroups.Remove(mod);
                    }
                    Finditem.IsDeleted = true;
                    _context.ModifiersItems.Update(Finditem);
                }

                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return false;
            }

        }

        public List<AddModifierViewModel> GetAllExistingModifier()
        {
            try
            {
                return _context.ModifiersItems.Where(c => !c.IsDeleted).Include(c => c.MappingModifierModifiergroups).ThenInclude(c => c.ModifierGroup)
                    .Select(c => new AddModifierViewModel
                    {
                        ModifierId = c.ModifierId,
                        ModifierName = c.ModifierName,
                        Rate = c.Rate,
                        Quantity = c.Quantity,
                        Description = c.Description,
                        UnitId = c.UnitId
                    })

                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

    }
}