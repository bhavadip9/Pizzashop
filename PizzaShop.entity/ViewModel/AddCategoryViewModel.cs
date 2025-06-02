
using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddCategoryViewModel
{
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set;}

        public string? Description { get; set; }

        public bool IsDeleted { get; set; }
        
}