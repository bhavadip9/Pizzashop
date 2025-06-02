using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public class OrderDetailsVM
{
    public int order_id { get; set; }
    public string order_status { get; set; } = string.Empty;
    public DateTime? createdat { get; set; }
    public string? order_comment { get; set; }
    public int? orderdetail_id { get; set; }
    public bool? orderitem_isdelete { get; set; }
    public string? item_comment { get; set; }
    public int? prepared { get; set; }
    public int? quantity { get; set; }
    public int? item_id { get; set; }
    public string? item_name { get; set; }
    public int? categoryid { get; set; }
    public string? category_name { get; set; }
    public int? ordered_item_modifier_id { get; set; }
    public int? table_id { get; set; }
    public string? table_name { get; set; }
    public DateTime? table_modifiedat { get; set; }
    public int? section_id { get; set; }
    public string? section_name { get; set; }
}