using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
    public interface ISectionService
    {
         public Section AddSectionAsync(AddSectionViewModel section);

        public Task<List<AddSectionViewModel>> GetAllSection();

        public void DeleteSection(int id);

        public AddSectionViewModel GetEditSection(int id);

        public bool UpdateSection(AddSectionViewModel section);

        public List<Section> GetAllSectionList();

        public AddTableViewModel AddTable(AddTableViewModel table);



        public Task<AddPaginationViewmodel<AddTableViewModel>> GetAllTable(int SectionId, int page, int pageSize, string search = "");

        public AddTableViewModel GetEditTable(int id);

        public AddTableViewModel EditTable(AddTableViewModel model);

        public void DeleteTable(int id);

        public void DeleteManyTable(List<int> Ids);
        public List<AddTableViewModel> GetTableList(int SectionId);

    }
}