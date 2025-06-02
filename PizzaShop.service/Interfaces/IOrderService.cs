using Pizzashop.entity.ViewModels;


namespace PizzaShop.service.Interfaces
{
    public interface IOrderService
    {


        public Task<AddPaginationViewmodel<OrderViewModel>> GetAllOrder(int page, int pageSize, string search, string date, string status, DateTime fromDate, DateTime toDate, string orderbyorderid, string orderbyorderdate);
        public ExportOrdersResult GetExportOrders(string search, string status, int time, DateTime fromDate, DateTime toDate);


        public OrderDetailViewModel GetOrderDetails(int id);
    }
}