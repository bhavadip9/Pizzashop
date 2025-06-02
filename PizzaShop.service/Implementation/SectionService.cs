
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _repository;

        public SectionService(ISectionRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all section
        /// </summary>
        /// <returns></returns>
        public async Task<List<AddSectionViewModel>> GetAllSection()
        {
            try
            {
                var section = await _repository.GetAllSection();
                var sectionViewModels = section.Select(c => new AddSectionViewModel
                {

                    SectionId = c.SectionId,
                    SectionName = c.SectionName,
                    SectionDescription = c.Description
                }).ToList();
                return sectionViewModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null!;
            }
        }
        /// <summary>
        /// Get GetAllSectionList
        /// </summary>
        /// <returns></returns>
        public List<Section> GetAllSectionList()
        {
            try
            {
                return _repository.GetAllSectionList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null!;
            }
        }


        /// <summary>
        /// Add section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Section AddSectionAsync(AddSectionViewModel section)
        {
            try
            {
                if (section == null)
                {
                    return null!;
                }
                var sectiondata = new Section
                {
                    SectionName = section.SectionName,
                    Description = section.SectionDescription,
                };
               return  _repository.AddSection(sectiondata);



            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }


        public AddSectionViewModel GetEditSection(int id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                var section = _repository.getSection(id);

                AddSectionViewModel addSection = new AddSectionViewModel();

                addSection.SectionId = section.SectionId;
                addSection.SectionName = section.SectionName;
                addSection.SectionDescription = section.Description;
                return addSection;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public bool UpdateSection(AddSectionViewModel section)
        {
            try
            {
                if (section == null)
                {
                    return false;
                }
                var Section = _repository.getSection(section.SectionId);
                if (Section != null)
                {
                    Section.SectionName = section.SectionName;
                    Section.Description = section.SectionDescription;

                    _repository.UpdateSection(Section);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public void DeleteSection(int id)
        {
            try
            {
                if (id == null)
                {
                    return;
                }
                _repository.DeleteSection(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }

        #region Table


        public List<AddTableViewModel> GetTableList(int SectionId)
        {
            var items = _repository.GetAllTableList(SectionId);

            var itemViewModels = items.Select(c => new AddTableViewModel
            {
                SectionId = c.SectionId,
                TableName = c.TableName,

            }).ToList();

            return itemViewModels;

        }

        public AddTableViewModel AddTable(AddTableViewModel table)
        {
            try
            {
                if (table == null)
                {
                    return null;
                }
                return _repository.AddTable(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }


        public async Task<AddPaginationViewmodel<AddTableViewModel>> GetAllTable(int SectionId, int page, int pageSize, string search)
        {
            var items = await _repository.GetAllTable(SectionId);
            int tableCount;

            var itemViewModels = items.Select(c => new AddTableViewModel
            {
                SectionId = c.SectionId,
                TableName = c.TableName,
                Capacity = c.Capacity,
                Status = c.Status,
                TableId = c.TableId

            }).ToList();


            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.TableName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            tableCount = itemViewModels.Count;

            itemViewModels = itemViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new AddPaginationViewmodel<AddTableViewModel>
            {
                Items = itemViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
            };
        }
        public AddTableViewModel GetEditTable(int id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                var table = _repository.GetTable(id);

                AddTableViewModel newtable = new AddTableViewModel();
                newtable.TableName = table.TableName;
                newtable.SectionId = table.SectionId;
                newtable.Capacity = table.Capacity;
                newtable.Status = table.Status;
                newtable.TableId = table.TableId;


                return newtable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public AddTableViewModel EditTable(AddTableViewModel model)
        {
            try
            {
                var id = model.TableId;
                if (id == null)
                {
                    return null;
                }
                var newtable = _repository.GetTable(id);


                if (newtable != null)
                {
                    newtable.TableName = model.TableName;
                    newtable.SectionId = model.SectionId;
                    newtable.Capacity = model.Capacity;
                    newtable.Status = model.Status;

                    _repository.EditTable(newtable);
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public void DeleteTable(int id)
        {
            try
            {
                if (id == null)
                {
                    return;
                }
                _repository.DeleteTable(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }

        public void DeleteManyTable(List<int> Ids)
        {
            try
            {
                if (Ids == null)
                {
                    return;
                }
                _repository.DeletemanyTable(Ids);

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));

            }
        }


        #endregion


    }
}