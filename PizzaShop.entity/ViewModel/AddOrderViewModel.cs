
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddOrderViewModel
{



    public int Id { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }

 
    public List<AddModifierItem> Modifiers { get; set; }

}


public class AddModifierItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Price { get; set; }

}



