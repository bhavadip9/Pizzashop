
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
  public interface ISectionRepository
  {
    public Section AddSection(Section section);
    public Task<List<Section>> GetAllSection();
    public bool DeleteSection(int id);

    public Section getSection(int id);

    public bool UpdateSection(Section section);

    public List<Section> GetAllSectionList();

    public AddTableViewModel AddTable(AddTableViewModel table);

    public Task<List<Table>> GetAllTable(int sectionId);
    public Table GetTable(int id);

    public Table EditTable(Table table);

    public void DeleteTable(int id);

    public void DeletemanyTable(List<int> Ids);
    public List<Table> GetAllTableList(int sectionId);
  }
}