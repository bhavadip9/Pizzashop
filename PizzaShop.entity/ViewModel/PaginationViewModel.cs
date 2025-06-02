



using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class PaginationViewmodel
{
  public List<User> Users { get; set; } = null!;

  public string RoleSortOrder { get; set; } = null;

  public string? NameSortOrder { get; set; }

  public String? RoleName { get; set; }

  public int TotalCount { get; set; }
  public int CurrentPage { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }

  public string Page { get; set; }

  public string? Orderby { get; set; }

  public string Image { get; set; }


}