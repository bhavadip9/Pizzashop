
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class DashboardViewModel
{
    public float TotalSale { get; set; }
    public int TotalOrder { get; set; }
    public string Role { get; set; }
    public int TotalWaitingUser { get; set; }

    public float AvgTotalOrder{ get; set; }
    public float AvgWaitingTime { get; set; }

    public int NewCustomerCount { get; set; }
    public int WaitingListCount { get; set; }

    public double AverageWaitingTime{get; set;}

    public List<SellingItem> TopSellingItems { get; set; }
    public List<SellingItem> LeastSellingItems { get; set; }

    // public List<RevenueGraph> RevenueGraph { get; set; }

}


public class SellingItem
{
    public int ItemId { get; set; }
    public int TotalQuantity { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }

}
// public class RevenueGraph
// {
//     public DateTime Date { get; set; }
//     public float TotalRevenue { get; set; }

// }

public class GraphDataVM
{
    public decimal MaxRevenue { get; set; }
    public int MaxCustomerGrowth { get; set; }
    public List<string>? Labels { get; set; }
    public List<decimal> RevenueData { get; set; }
    public List<int>? CustomerGrowthData { get; set; }
}