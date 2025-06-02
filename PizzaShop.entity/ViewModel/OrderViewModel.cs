
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class OrderViewModel
{

    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public string PaymentMethod { get; set; }

    public int? Rating { get; set; }

    public float? TotalAmount { get; set; }
    public string CustomerName { get; set; }

    public string? OrderStatus { get; set; }

    public DateTime OrderDate { get; set; }

    public string Date { get; set; }
}