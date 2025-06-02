
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzashop.entity.ViewModels
{
  public class MenuListViewModel
  {
   
    public List<AddCategoryViewModel>? Category { get; set; }
    public List<AddItemViewModel>? CategoryItem { get; set; }
    public List<AddModifierGroupViewModel>? ModifierGroup { get; set; }

    public List<AddModifierViewModel>? Modifier { get; set; }


  }
}