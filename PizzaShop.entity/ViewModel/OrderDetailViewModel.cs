
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class OrderDetailViewModel
{

        public int OrderId { get; set; }
       
        public int InvoiceId { get; set; }
        public int DetailId { get; set; }

        public string? CustomerPhone { get; set; }

        public int TotalPerson { get; set; }
        public int CustomerId { get; set; }

        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }

        public string section { get; set; }
        public float TotalTax { get; set; }

        public string OrderStatus { get; set; }
        public string Status { get; set; }

        public string OrderComment { get; set; }
        public string ItemComment { get; set; }

        public string OrderItemQuntity { get; set; }
        public string TableName { get; set; }
        public string SectionName { get; set; }
        public string PaymentMode { get; set; }

        public float OrderItemPrice { get; set; }
        public float? Subtotal { get; set; }
        public float OderAmount { get; set; }
        public float? OtherTax { get; set; }

        public float OrderItemAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime Assigntime { get; set; }

        public DateTime EndDate { get; set; }

        public List<string> TableList = new List<string>();

        public AddTableViewModel table = new AddTableViewModel();
        public List<AddTableViewModel> ManyTableList = new List<AddTableViewModel>();
        public List<AddTaxViewModel> tax = new List<AddTaxViewModel>();

        public List<OrderManyItem> OrderItem = new List<OrderManyItem>();



}

public class UpdateOrder
{
        public int detailsid { get; set; }

        public int quantity { get; set; }
}


public class ModifierViewModel
{
        public string ModifierName { get; set; }
        public int? ModifierId { get; set; }
        public int? Quantity { get; set; }
        public float ModifierPrice { get; set; }

}

public class OrderManyItem
{
        public float OtherTax { get; set; }
        public int ItemId { get; set; }
        public int OrderDetailId { get; set; }
        public string ItemName { get; set; }

        public int categoryid { get; set; }
        public int Quantity { get; set; }
        public int Prepared { get; set; }
        public float Price { get; set; }

        public string ItemStatus { get; set; }

        public string ItemByComment { get; set; }
        public List<ModifierViewModel> modifier = new List<ModifierViewModel>();
}