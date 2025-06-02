
using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddSectionViewModel
{

    public int SectionId { get; set; } 

    [Required]
    public string SectionName { get; set; } = null!;

    public string? SectionDescription { get; set; }


}