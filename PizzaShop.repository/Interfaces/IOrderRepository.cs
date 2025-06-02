
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
  public interface IOrderRepository
  {

    public Task<List<Order>> GetAllOrder();

    public IQueryable<Order> GetAllOrderExport();

    public Order GetOrderDetails(int id);
  
    public List<AddTaxViewModel> TaxFind(int id);
    public List<AddTableViewModel> TableMappping(int id);
     public string GetModifierName(int OrderModifierId);
    public int GetModifierPrice(int OrderModifierId);
    List<TaxesAndFee> AllTaxForOrder();
    public List<AddTableViewModel> TableMapppingAfterorder(int id);
    public List<Order> OrderDetailsForDashboard(DateTime fromdate);

     public List<OrderDetailsVM> GetOrderDetailsVM(int id);
  }
}