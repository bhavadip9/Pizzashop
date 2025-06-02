
using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class TaxRepository : ITaxRepository
    {
        private readonly NewPizzashopContext _context;

        public TaxRepository(NewPizzashopContext context)
        {
            _context = context;
        }

        public async Task<List<TaxesAndFee>> GetAllTax()
        {
            try
            {

                return await _context.TaxesAndFees
                       .Where(c => !c.IsDelete).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;

            }
        }
        public List<TaxesAndFee> GetAllTaxList()
        {
            try
            {

                return _context.TaxesAndFees
                       .Where(c => !c.IsDelete).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;

            }
        }

        public bool AddTax(AddTaxViewModel table)
        {
            try
            {
                var taxdata = new TaxesAndFee();

                taxdata.TaxName = table.TaxName;
                taxdata.TaxType = table.TaxType;

                taxdata.IsEnabled = table.IsEnable;
                taxdata.IsDefault = table.IsDefault;
                taxdata.TaxValue = table.TaxAmount;
                _context.TaxesAndFees.Add(taxdata);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public TaxesAndFee GetTax(int id)
        {
            return _context.TaxesAndFees.FirstOrDefault(m => m.TaxId == id)!;
        }

        public bool UpdateTax(TaxesAndFee user)
        {
            try
            {
                var result = _context.TaxesAndFees.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public bool DeleteTax(int id)
        {
            var tax = _context.TaxesAndFees.Find(id);

            if (tax != null)
            {
                tax.IsDelete = true;
                _context.TaxesAndFees.Remove(tax);
            }
            _context.SaveChanges();
            return true;
        }

    }
}