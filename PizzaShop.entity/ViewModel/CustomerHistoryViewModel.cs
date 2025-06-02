
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class CustomerHistoryViewModel
{

    public int CustomerId { get; set; }

    public string? CustomerPhone { get; set; }

    public int OrderAmount { get; set; }
    public int Maxbill { get; set; }

    public float? Avgbill { get; set; }

    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }

    public int Visits { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<OrderDetailVM> orderDetailViewModels = new List<OrderDetailVM>();


}


public class OrderDetailVM
{
    public DateTime OrderDate { get; set; }

    public string OrderType { get; set; }

    public string PaymentMethod { get; set; }

    public int TotalItem { get; set; }

    public decimal? OrderAmount { get; set; }

}