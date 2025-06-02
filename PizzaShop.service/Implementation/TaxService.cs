
using PizzaShop.service.Interfaces;
using Pizzashop.entity.ViewModels;
using PizzaShop.repository.Interfaces;


namespace PizzaShop.service.Implementation
{
    public class TaxService : ITaxService
    {
        private readonly ITaxRepository _repository;

        public TaxService(ITaxRepository repository)
        {
            _repository = repository;
        }

        public List<AddTaxViewModel> GetAllTaxList()
        {
            var taxes = _repository.GetAllTaxList();
            var taxViewModels = taxes.Select(c => new AddTaxViewModel
            {

                TaxId = c.TaxId,
                TaxName = c.TaxName,
                TaxType = c.TaxType,
                TaxAmount = c.TaxValue,
                IsEnable = c.IsEnabled,
                IsDefault = c.IsDefault,

            }).ToList();

            return taxViewModels;

        }
        public bool AddTax(AddTaxViewModel tax)
        {
            try
            {
                if (tax == null)
                {
                    return false;
                }

                var result = _repository.AddTax(tax);
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

        public async Task<List<AddTaxViewModel>> GetAllTax(string search)
        {
            try
            {
                var taxes = await _repository.GetAllTax();
                var taxViewModels = taxes.Select(c => new AddTaxViewModel
                {

                    TaxId = c.TaxId,
                    TaxName = c.TaxName,
                    TaxType = c.TaxType,
                    TaxAmount = c.TaxValue,
                    IsEnable = c.IsEnabled,
                    IsDefault = c.IsDefault,

                }).ToList();
                //var search = string.Empty;
                if (!string.IsNullOrEmpty(search))
                {
                    taxViewModels = taxViewModels.Where(u => u.TaxName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return taxViewModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public AddTaxViewModel GetEditTax(int id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }

                var tax = _repository.GetTax(id);

                AddTaxViewModel gettax = new AddTaxViewModel();
                gettax.TaxName = tax.TaxName;
                gettax.TaxType = tax.TaxType;
                gettax.TaxAmount = tax.TaxValue;
                gettax.IsEnable = tax.IsEnabled;
                gettax.IsDefault = tax.IsDefault;
                gettax.TaxId = tax.TaxId;

                return gettax;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool EditTax(AddTaxViewModel model)
        {
            try
            {
                var id = model.TaxId;
                if (id == null && id == 0)
                {
                    return false;
                }
                var oldtax = _repository.GetTax(id);


                if (oldtax != null)
                {
                    oldtax.TaxName = model.TaxName;
                    oldtax.TaxType = model.TaxType;
                    oldtax.TaxValue = model.TaxAmount;
                    if (oldtax.IsEnabled != model.IsEnable)
                    {
                        oldtax.IsEnabled = model.IsEnable;
                    }

                    oldtax.IsDefault = model.IsDefault;

                    _repository.UpdateTax(oldtax);

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public bool DeleteTax(int id)
        {
            try
            {

                var result = _repository.DeleteTax(id);
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



    }
}