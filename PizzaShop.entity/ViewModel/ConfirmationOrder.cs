using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public class ConfirmationOrder
{



    public int orderid { get; set; }
    public int customerid { get; set; }
    public float totalbill { get; set; }
    public float subtotal { get; set; }
    public float othertax { get; set; }
    public List<TaxList> selectedTaxes { get; set; }
    public string selectedTaxesstring { get; set; }

    public List<OrderTaxMapping> orderTaxMappings { get; set; }
  

}

public class TaxList
{
    public int taxid { get; set; }

    public int  orderid { get; set; }

    public float taxAmount { get; set; }

}
