


using System.ComponentModel.DataAnnotations;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public partial class WaitingViewModel
{
    public int? TableId { get; set; }

    public List<SectionList>? SectionList { get; set; } = new List<SectionList>();
}


public class SectionList
{


    public int? SectionId { get; set; }

    public string? SectionName { get; set; }

    public List<TableList>? TableList { get; set; } = new List<TableList>();
}
public class TableList
{

    public int? OrderId { get; set; }
    public int? TableId { get; set; }

    public string? TableName { get; set; }

    public string? TableStatus { get; set; }

    public DateTime Time { get; set; }
    public DateTime Assigntime { get; set; }

    public int? Capacity { get; set; }

    public float? orderAmount { get; set; }
}



public class WaitingUserDetails
{
    [Required]
    [Range(1, 999999, ErrorMessage = "No of Person must be greater than 1.")]
    public int No_of_Person { get; set; }
    public int WaitingUserId { get; set; }

    [Required]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string Phone { get; set; }

    [Required]
    public string Email { get; set; }
    public int SectionId { get; set; }
    public DateTime CreateTime { get; set; }

    public TimeSpan WaitingTime { get; set; }

    public int? TableId { get; set; }
    public List<int>? SelectedTable { get; set; } = new List<int>();
}


public class AddWaitingAssignViewModel
{
    public List<WaitingUserDetails>? WaitingUserList { get; set; } = new List<WaitingUserDetails>();

    public List<int>? SelectedTable { get; set; } = new List<int>();

    public List<SectionList>? SectionList { get; set; } = new List<SectionList>();

    public int? SectionId { get; set; }


}

public class TableAndSectionViewModel
{
    public List<SectionList>? SectionList { get; set; } = new List<SectionList>();

    public List<TableList>? TableList { get; set; } = new List<TableList>();

    [Required]
    public List<int>? SelectedTablelist { get; set; }

    [Required]
    public int sectionId { get; set; }
    public int? WaitingUserId { get; set; }

}