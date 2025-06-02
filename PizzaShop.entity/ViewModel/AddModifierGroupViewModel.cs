
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddModifierGroupViewModel
{

  public int ModifierGroupId { get; set; }

  public string ModifierName { get; set; } = null!;

  public string? Descriptionmodifier { get; set; }


  //  public List<AddModifierViewModel> Modifier { get; set; }

  public List<int> SelectedModifierIds { get; set; }
  public List<string> SelectedModifierName { get; set; }
  public List<int> editSelectedModifierIds { get; set; }
  public List<string> editSelectedModifierName { get; set; }

  public List<ItemSelected> ModifierItems { get; set; } = new List<ItemSelected>();

  public string? SelectedModifierJsonEdit { get; set; }
}



public class ItemSelected
{
  public int Id { get; set; }
  public string Name { get; set; }
}
