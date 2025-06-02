using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class WaitingUser
{
    public int? TokenId { get; set; }

    public int? TotalPerson { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public int? SectionId { get; set; }

    public string? PhoneNo { get; set; }

    public bool? IsDelete { get; set; }
}
