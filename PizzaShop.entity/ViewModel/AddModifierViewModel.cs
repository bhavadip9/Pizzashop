
using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddModifierViewModel
{
     
    public int ModifierGroupId { get; set; }
    public int MappingId { get; set; }
[Required]
    public string ModifierName { get; set; } 
    
    public string ModifierGroupName { get; set; }

    [Required(ErrorMessage = "Rate is required.")]
    public decimal Rate { get; set; }

    public string? Description { get; set; }

    public int UnitId { get; set; }
    public string UnitName { get; set; }

    public int ModifierId { get; set; }

    public bool IsDeleted { get; set; }
[Required]
    public int Quantity { get; set; }

    public List<int> SelectedModifierIds { get; set; }
    public List<string> SelectedModifierName { get; set; }
        
}