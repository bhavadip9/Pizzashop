
using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class AddTaxViewModel
{

    public int TaxId { get; set; }
    [Required]
    public string TaxName { get; set; }

    [Required]
    public bool TaxType { get; set; }

    public bool IsEnable { get; set; }
    public bool IsDefault { get; set; }


    [Required]
    [Range(0.001, 999999, ErrorMessage = "Tax must be greater than 0.")]
    public decimal TaxAmount { get; set; }
    public float? TotalTax { get; set; }


}