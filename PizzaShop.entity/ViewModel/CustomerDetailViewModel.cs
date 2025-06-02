
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class CustomerDetailViewModel
{

    public int CustomerId { get; set; }

    public string? CustomerPhone { get; set; }

    public int TotalOrder { get; set; }
    public int? TotalPerson { get; set; }

    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }

    public int Orderid{get; set;}


    public DateTime Date { get; set; }

}