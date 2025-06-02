namespace Pizzashop.entity.ViewModels;
using System.ComponentModel.DataAnnotations;

public class AddEventVM
{
    public int EventId { get; set; }
    public int EventTypeId { get; set; }
    public int No_of_Person { get; set; }
    public int OrderType { get; set; }
    public bool AC_Non_AC { get; set; }
  
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Status { get; set; } = "Active";
}