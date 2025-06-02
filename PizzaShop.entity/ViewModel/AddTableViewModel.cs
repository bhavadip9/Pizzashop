
using System.ComponentModel.DataAnnotations;


namespace Pizzashop.entity.ViewModels;

public partial class AddTableViewModel
{
 [Required]
    public int SectionId { get; set; }

    public int TableId { get; set; }

    [Required]
    public string TableName { get; set; } = null!;
    public string SectionName { get; set; } = null!;

    [Required]
    [Range(1, 999999, ErrorMessage = "Capacity must be greater than 1.")]
    public int Capacity { get; set; }
    [Required]
    public string? TableDescription { get; set; }

    public string Status { get; set; }





}