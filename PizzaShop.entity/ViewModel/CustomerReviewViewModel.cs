
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class CustomerReviewViewModel
{

    public int CustomerId { get; set; }

    public int orderid { get; set; }
    public int FoodRating { get; set; }
    public int ServiceRating { get; set; }
    public int AmbienceRating { get; set; }
    public string OrderComment { get; set; }


}