
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Gets all categories from the repository.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AddCategoryViewModel>> GetAllCategory()
        {
            try
            {
                var categories = await _repository.GetAllCategory();
                var categoryViewModels = categories.Select(c => new AddCategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description
                }).ToList();
                return categoryViewModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
        public List<Category> GetAllCategoryList()
        {
            try
            {
                var categories = _repository.GetAllCategoryLIst();

                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Gets all categories for select Item .
        /// </summary>
        /// <returns></returns>

        public List<AddCategoryViewModel> GetAllCategoriesForItem()
        {
            try
            {
                return _repository.GetAllCategoriesForItem();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public List<Unit> GetAllUnit()
        {
            try
            {
                return _repository.UnitGet();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public List<ModifierGroup> GetAllModifierGroup()
        {
            try
            {
                return _repository.GroupModifer();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }



        public bool IsExistCategory(string categoryName)
        {
            try
            {
                var result = _repository.CheckCategoryName(categoryName);
                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Adds a new category to the repository.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 


        public Category AddCategoryAsync(AddCategoryViewModel model)
        {
            try
            {
                var userModel = new Category
                {

                    CategoryId = model.CategoryId,
                    CategoryName = model.CategoryName,
                    Description = model.Description

                };
                var result = _repository.AddCategory(userModel);

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null!;
            }

        }

        /// <summary>
        /// Edits an existing category get data in the repository.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public AddCategoryViewModel EditCategory(int id)
        {
            try
            {
                if (id == null)
                {
                    return null!;
                }

                var category = _repository.GetOneByIdAsync(id);

                AddCategoryViewModel getcategory = new AddCategoryViewModel();

                getcategory.CategoryId = category.CategoryId;
                getcategory.CategoryName = category.CategoryName;
                getcategory.Description = category.Description;

                return getcategory;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool CheckCategoryName(string categoryName, int id)
        {
            try
            {
                var result = _repository.CheckCategoryName(categoryName, id);
                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Edits an existing category Edit data in the repository.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AddCategoryViewModel EditCategory(AddCategoryViewModel model)
        {
            try
            {
                var id = model.CategoryId;
                if (id == null)
                {
                    return null;
                }
                var category = _repository.GetOneByIdAsync(id);


                if (category != null)
                {
                    category.CategoryId = model.CategoryId;
                    category.CategoryName = model.CategoryName;
                    category.Description = model.Description;
                    _repository.EditCategory(category);
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Deletes an existing category get data in the repository.
        /// </summary>
        /// <param name="id"></param>
        public bool DeleteCategory(int id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }
                var result = _repository.DeleteAsync(id);
                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }


        #region Menu Item 


        /// <summary>
        /// Gets all items from the repository.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<AddPaginationViewmodel<AddItemViewModel>> GetAllItem(int categoryId, int page, int pageSize, string search)
        {
            var items = await _repository.GetAllItem(categoryId);
            int tableCount;

            var itemViewModels = items.Select(c => new AddItemViewModel
            {
                CategoryId = c.CategoryId,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                IsAvailable = c.IsAvailable,
                FoodType = c.FoodType,
                shortcode = c.Shortcode,
                Image = c.Image,
                Descriptionitem = c.Description,
                Quantity = c.Quantity,
                Price = c.Price,
                IsDefault = c.IsDefault
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.ItemName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            tableCount = itemViewModels.Count;

            itemViewModels = itemViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new AddPaginationViewmodel<AddItemViewModel>
            {
                Items = itemViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
            };
        }


        public List<AddItemViewModel> GetMenuItem(int categoryid)
        {
            var items = _repository.GetMenuItem(categoryid);
            var itemViewModels = items.Select(c => new AddItemViewModel
            {
                ItemId = c.ItemId,
                ItemName = c.ItemName,
            }).ToList();

            return itemViewModels;
        }


        /// <summary>
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 

        public bool AddItem(AddItemViewModel model)
        {
            try
            {
                var menuItem = new MenuItem();
                menuItem.ItemName = model.ItemName;
                menuItem.FoodType = model.FoodType;
                menuItem.IsAvailable = model.IsAvailable;
                menuItem.Price = model.Price;
                menuItem.Quantity = model.Quantity;
                menuItem.Shortcode = model.shortcode;
                menuItem.Description = model.Descriptionitem;
                menuItem.CategoryId = model.CategoryId;
                menuItem.Image = model.Image;
                menuItem.UnitId = model.UintId;
                menuItem.IsDefault = model.IsDefault;

                if (model.Tax == null)
                {
                    menuItem.Tax = 0;
                }
                else
                {
                    menuItem.Tax = model.Tax;
                }

                var item = _repository.AddItem(menuItem);

                if (model.ModifierGroups != null)
                {
                    foreach (var modifier in model.ModifierGroups)
                    {
                        var newMapping = new MappingItemModifierGroup
                        {
                            ItemId = item.ItemId,
                            ModifierGroupId = modifier.Id,
                            MinValue = modifier.Min,
                            MaxValue = modifier.Max
                        };
                        _repository.AddItemMapping(newMapping);
                    }

                }
                if (item == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch
            {
                return false;
            }



        }

        public async Task<AddPaginationViewmodel<AddModifierViewModel>> GetAllModifier(int modifierGroupId, int page, int pageSize, string search)
        {
            var items = await _repository.GetAllModifier(modifierGroupId);
            var ModifiergroupName = await _repository.GetModifierNameAsync(modifierGroupId);

            int tableCount;

            var itemViewModels = items.Select(c => new AddModifierViewModel
            {
                ModifierId = c.ModifierId,
                ModifierName = c.ModifierName,
                Rate = c.Rate,
                UnitName = c.Unit.UnitName,
                ModifierGroupName = ModifiergroupName,
                Quantity = c.Quantity,
                MappingId = c.MappingModifierModifiergroups.Select(m => m.MappingId).FirstOrDefault()
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.ModifierName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            tableCount = itemViewModels.Count;

            itemViewModels = itemViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new AddPaginationViewmodel<AddModifierViewModel>
            {
                Items = itemViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
            };
        }



        public async Task<List<AddModifierViewModel>> GetAllModifier(int modifierGroupId)
        {
            try
            {
                var items = await _repository.GetAllModifier(modifierGroupId);
                var ModifiergroupName = await _repository.GetModifierNameAsync(modifierGroupId);
                var modifier = items.Select(c => new AddModifierViewModel
                {
                    ModifierId = c.ModifierId,
                    ModifierName = c.ModifierName,
                    Rate = c.Rate,
                    UnitName = c.Unit.UnitName,
                    ModifierGroupName = ModifiergroupName,
                    Quantity = c.Quantity
                }).ToList();
                return modifier;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }


        /// <summary>
        /// Get item for Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AddItemViewModel GetEditItem(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null!;
                }

                var item = _repository.GetItem(id);

                if (item == null)
                {
                    return null;
                }


                List<DataItem> modifierGroups = new List<DataItem>();
                if (item.MappingItemModifierGroups != null)
                {
                    var modifier = item.MappingItemModifierGroups.Select(c => new DataItem
                    {
                        Id = c.ModifierGroup.ModifierGroupId,
                        Min = c.MinValue ?? 0,
                        Max = c.MaxValue ?? 0,
                        Name = c.ModifierGroup.GroupName
                    }).ToList();
                    modifierGroups = modifier;
                }

                return new AddItemViewModel
                {
                    ItemId = item.ItemId,
                    CategoryId = item.CategoryId,
                    FoodType = item.FoodType,
                    ItemName = item.ItemName,
                    IsAvailable = item.IsAvailable,
                    Descriptionitem = item.Description,
                    IsDefault = item.IsDefault,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Tax = item.Tax,
                    shortcode = item.Shortcode,
                    UintId = item.UnitId,
                    Image = item.Image,
                    ModifierGroups = modifierGroups
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} , StackTrace: {ex.StackTrace}");
                return null;
            }
        }



        /// <summary>
        /// Edit item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns
        public AddItemViewModel EditItem(AddItemViewModel model)
        {
            try
            {
                var id = model.ItemId;
                if (id == 0)
                {
                    return null;
                }

                var item = _repository.GetOneItemByID(id);

                if (item != null)
                {

                    item.CategoryId = model.CategoryId;
                    item.FoodType = model.FoodType;
                    item.ItemName = model.ItemName;
                    item.IsAvailable = model.IsAvailable;
                    item.IsDefault = model.IsDefault;
                    item.Price = model.Price;
                    item.Tax = model.Tax;
                    item.Quantity = model.Quantity;
                    if (model.Image == null)
                    {
                        item.Image = item.Image;
                    }
                    else
                    {
                        item.Image = model.Image;
                    }
                    item.Shortcode = model.shortcode;
                    item.UnitId = model.UintId;
                    item.Description = model.Descriptionitem;


                    if (model.ModifierGroups != null)
                    {

                        var existingMappings = _repository.GetMappingByItemId(item.ItemId);
                        if (existingMappings != null)
                        {
                            _repository.DeleteMappings(existingMappings);
                        }
                        foreach (var modifier in model.ModifierGroups)
                        {
                            var newMapping = new MappingItemModifierGroup
                            {
                                ItemId = item.ItemId,
                                ModifierGroupId = modifier.Id,
                                MinValue = modifier.Min,
                                MaxValue = modifier.Max
                            };
                            _repository.AddMapping(newMapping);
                        }
                    }


                    _repository.EditItem(item);
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} , StackTrace: {ex.StackTrace}");
                return null!;
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id"></param>
        public void DeleteItem(int id)
        {
            try
            {
                if (id == null)
                {
                    return;
                }
                _repository.DeleteItemAsync(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }

        /// <summary>
        /// Delete many item
        /// </summary>
        /// <param name="Ids"></param>
        public void DeleteManyItem(List<int> Ids)
        {
            try
            {
                if (Ids == null)
                {
                    return;
                }
                _repository.DeletemanyItem(Ids);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }



        #endregion

        #region  Modifier



        // public bool CheckModifierName(int ModifierName, List<int> SelectedModifierIds)
        // {
        //     foreach(var modifierGroupId in SelectedModifierIds){
        //         var result = _repository.CheckModifierName(ModifierName, modifierGroupId); 
        //     }
        //    // var result = _repository.CheckModifierName(ModifierName, SelectedModifierIds);
        //     return result;
        // }
        /// <summary>
        /// Adds a new modifier to the repository.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public bool AddModifieritem(AddModifierViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return false;
                }
                var modifierModel = new ModifiersItem
                {
                    ModifierName = model.ModifierName,
                    Rate = model.Rate,
                    Quantity = model.Quantity,
                    Description = model.Description,
                    UnitId = model.UnitId
                };
                var modifier = _repository.AddModifieritem(modifierModel);
                foreach (var itemId in model.SelectedModifierIds)
                {
                    var itemModifierGroup = new MappingModifierModifiergroup
                    {
                        ModifierId = modifier.ModifierId,
                        ModifierGroupId = itemId
                    };
                    _repository.AddModifierMapping(itemModifierGroup);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }



        /// <summary>
        /// Gets all modifier from the repository.
        /// </summary>
        /// <returns></returns>
        public List<ModifierGroup> GetAllModifierGroupList()
        {
            try
            {
                var modifierGroups = _repository.GetAllModifierGroupLIst();

                return modifierGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        /// <summary>
        /// Gets all modifier for select Item .
        /// </summary>
        /// <returns></returns>
        /// 
        public List<AddModifierGroupViewModel> GetAllMOdifierForItem()
        {
            try
            {
                return _repository.GetAllModifierForItem();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null!;
            }
        }

        /// <summary>
        /// Gets all modifier from the repository.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AddModifierGroupViewModel>> GetAllModifierGroupsidebar()
        {
            try
            {
                var modifiergroups = await _repository.GetAllModofierGroup();
                var categoryViewModels = modifiergroups.Select(c => new AddModifierGroupViewModel
                {
                    ModifierGroupId = c.ModifierGroupId,
                    ModifierName = c.GroupName,
                    Descriptionmodifier = c.Description
                }).ToList();
                return categoryViewModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
        public async Task AddModifierGroupAsync(AddModifierGroupViewModel model)
        {
            var modifierGroup = new ModifierGroup
            {
                GroupName = model.ModifierName,
                Description = model.Descriptionmodifier
            };

            _repository.AddModifierGroup(modifierGroup);
            foreach (var ModifierID in model.SelectedModifierIds)
            {
                var itemModifierGroup = new MappingModifierModifiergroup
                {
                    ModifierId = ModifierID,
                    ModifierGroupId = modifierGroup.ModifierGroupId
                };
                _repository.AddModifierMapping(itemModifierGroup);

            }
        }

        public AddModifierGroupViewModel EditModifierGroup(int id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                var modifierGroup = _repository.GetOneModifierGroup(id);

                List<ItemSelected> modifierGroups123 = new List<ItemSelected>();
                if (modifierGroup.MappingModifierModifiergroups != null)
                {
                    var modifier = modifierGroup.MappingModifierModifiergroups.Where(c => !c.IsDeleted).Select(c => new ItemSelected
                    {
                        Id = c.Modifier.ModifierId,
                        Name = c.Modifier.ModifierName
                    }).ToList();
                    modifierGroups123 = modifier;
                }

                AddModifierGroupViewModel getModifierGroup = new AddModifierGroupViewModel();

                getModifierGroup.ModifierGroupId = modifierGroup.ModifierGroupId;
                getModifierGroup.ModifierName = modifierGroup.GroupName;
                getModifierGroup.Descriptionmodifier = modifierGroup.Description;
                getModifierGroup.ModifierItems = modifierGroups123;


                return getModifierGroup;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }



        public async Task EditModifierGroup(AddModifierGroupViewModel model)
        {
            var id = model.ModifierGroupId;
            var OldModifierGroup = _repository.GetOneModifierGroup(id);



            if (OldModifierGroup != null)
            {

                OldModifierGroup.ModifierGroupId = model.ModifierGroupId;
                OldModifierGroup.Description = model.Descriptionmodifier;
                OldModifierGroup.GroupName = model.ModifierName;



                if (model.ModifierItems != null)
                {
                    var existingMappings = _repository.GetMappingByGroupId(model.ModifierGroupId);
                    if (existingMappings != null)
                    {
                        _repository.DeleteMappingItrm(existingMappings);
                    }

                    foreach (var modifier in model.ModifierItems)
                    {
                        var newMapping = new MappingModifierModifiergroup
                        {
                            ModifierGroupId = model.ModifierGroupId,
                            ModifierId = modifier.Id
                        };
                        _repository.AddItemMapping(newMapping);
                    }
                }


                _repository.EditModifierGroup(OldModifierGroup);
            }

        }

        public void DeleteModifierGroup(int id)
        {
            try
            {
                if (id == null)
                {
                    return;
                }
                _repository.DeleteModifierGroup(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }




        public AddModifierViewModel GetEditModifierItem(int id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                var item = _repository.GetModifierItem(id);



                AddModifierViewModel getItem = new AddModifierViewModel
                {
                    ModifierId = item.ModifierId,
                    ModifierName = item.ModifierName,
                    Rate = item.Rate,
                    UnitId = item.UnitId,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    SelectedModifierIds = item.MappingModifierModifiergroups.Select(m => m.ModifierGroupId).ToList(),

                };



                return getItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool EditModifierItem(AddModifierViewModel model)
        {
            try
            {
                var id = model.ModifierId;
                if (id == null)
                {
                    return false;
                }
                var item = _repository.GetModifierItem(id);




                if (item != null)
                {
                    item.ModifierId = model.ModifierId;

                    item.ModifierName = model.ModifierName;
                    item.Rate = model.Rate;
                    item.Quantity = model.Quantity;
                    item.UnitId = model.UnitId;
                    item.Description = model.Description;

                    if (model.SelectedModifierIds != null)
                    {
                        var existingMappings = _repository.GetMappingByModifierId(item.ModifierId);

                        if (existingMappings != null)
                        {
                            _repository.DeleteMappingItrm(existingMappings);
                        }
                        foreach (var modifier in model.SelectedModifierIds)
                        {
                            var newMapping = new MappingModifierModifiergroup
                            {
                                ModifierId = item.ModifierId,
                                ModifierGroupId = modifier
                            };
                            _repository.AddItemMapping(newMapping);
                        }

                    }

                    _repository.EditModifierItem(item);
                }
                // foreach (var itemid in item.MappingModifierModifiergroups)
                // {
                //     if (model.SelectedModifierIds.Contains(itemid.MappingId))
                //     {
                //         var existingMappings = item.MappingModifierModifiergroups.FirstOrDefault(m => m.MappingId == itemid.MappingId);
                //         if (existingMappings != null)
                //         {
                //             _repository.DeleteModifierItemAsync(existingMappings.MappingId);
                //         }
                //     }
                //     else
                //     {
                //         var itemModifierGroup = new MappingModifierModifiergroup
                //         {
                //             ModifierId = item.ModifierId,
                //             ModifierGroupId = itemid.MappingId
                //         };
                //         _repository.AddModifierMapping(itemModifierGroup);

                //     }
                // }
                // foreach (var itemId in model.SelectedModifierIds)
                // {
                //     var existingMappings = item.MappingModifierModifiergroups.Where(m => m.ModifierGroupId == itemId).ToList();
                //     if (existingMappings != null)
                //     {
                //         _repository.DeleteMappingItrm(existingMappings);
                //     }

                //     var itemModifierGroup = new MappingModifierModifiergroup
                //     {
                //         ModifierId = item.ModifierId,
                //         ModifierGroupId = itemId
                //     };
                //     _repository.AddModifierMapping(itemModifierGroup);
                // }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public void DeleteModifierItem(int id)
        {
            try
            {
                if (id == null)
                {
                    return;
                }
                _repository.DeleteModifierItemAsync(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }
        public bool DeleteManyModifierItem(List<int> Ids)
        {
            try
            {
                if (Ids == null)
                {
                    return false;
                }
                var result = _repository.DeletemanyModifierItem(Ids);
                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;

            }
        }



        public List<AddModifierViewModel> GetAllExistingModifier()
        {
            try
            {
                return _repository.GetAllExistingModifier();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        #endregion

    }
}