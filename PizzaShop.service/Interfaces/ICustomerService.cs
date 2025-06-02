using Pizzashop.entity.ViewModels;



namespace PizzaShop.service.Interfaces
{
    public interface ICustomerService
    {
      public Task<AddPaginationViewmodel<CustomerDetailViewModel>> GetAllCutomer(int page, int pageSize, string search, string time, string SortbyName, string SortbyDate);

       public ExportOrdersResult GetExportCustomer(string search, string time);

        public CustomerHistoryViewModel GetCustomer(int id);
    }
}