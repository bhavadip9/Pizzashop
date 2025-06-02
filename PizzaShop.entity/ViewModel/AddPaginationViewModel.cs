



using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddPaginationViewmodel<T>
{
  public List<T> Items { get; set; }

  public int TotalCount { get; set; }
  public int categoryid { get; set; }
  public int CurrentPage { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }

  public string Page { get; set; }
  public string Email { get; set; }

  public string Category{get; set;}
  public string status{get; set;}


}
