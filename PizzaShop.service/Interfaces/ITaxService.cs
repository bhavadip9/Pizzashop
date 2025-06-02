
using Pizzashop.entity.ViewModels;

namespace PizzaShop.service.Interfaces
{
    public interface ITaxService
    {
        public bool AddTax(AddTaxViewModel tax);
        public Task<List<AddTaxViewModel>> GetAllTax(string search);

        public AddTaxViewModel GetEditTax(int id);
        public bool EditTax(AddTaxViewModel model);
        public bool DeleteTax(int id);
        public List<AddTaxViewModel> GetAllTaxList();
    }
}