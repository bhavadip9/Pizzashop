
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface ITaxRepository
    {
        public Task<List<TaxesAndFee>> GetAllTax();
        public bool AddTax(AddTaxViewModel table);
        public TaxesAndFee GetTax(int id);
        public bool UpdateTax(TaxesAndFee user);
        public bool DeleteTax(int id);
        public List<TaxesAndFee> GetAllTaxList();

    }
}