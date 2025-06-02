
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddItemViewModel
{

        public int ModifierGroupId { get; set; }
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Itemname is required.")]
        public string ItemName { get; set; }


        [Required]
        public string FoodType { get; set; }

        public string? Descriptionitem { get; set; }
        [Required]
        [Range(1, 999999, ErrorMessage = "Price must be greater than 1.")]
        public float Price { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsFav { get; set; }


        public bool IsDefault { get; set; }
        [Required]
        public float Tax { get; set; }
        [Required(ErrorMessage = "Please select a unit.")]
        public int UintId { get; set; }
        [Required]
        [Range(1, 999999, ErrorMessage = "Quantity must be greater than 1.")]
        public int Quantity { get; set; }
        public int TotalQuantity { get; set; }

        public string shortcode { get; set; }

        public string? Image { get; set; }

        public IFormFile? FormFile { get; set; }

        // public List<AddModifierGroupViewModel>? ModifierList { get; set; }

        public List<int>? ModifierIds { get; set; }

        public Dictionary<int, int>? ModifierMinValues { get; set; }
        public Dictionary<int, int>? ModifierMaxValues { get; set; }


        public string? SelectedModifierGroupJson { get; set; }
        public string? SelectedModifierGroupJsonEdit { get; set; }

        public string? SelectedModifierGroupIds { get; set; }

        public List<DataItem> ModifierGroups { get; set; } = new List<DataItem>();

}

public class DataItem
{
        public int Id { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public string Name { get; set; }

        public List<AddModifierViewModel> ModifierItem { get; set; }


}


