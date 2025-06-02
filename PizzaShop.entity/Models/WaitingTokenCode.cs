using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class WaitingTokenCode
{
    public int TokenId { get; set; }

    public int TotalPerson { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int SectionId { get; set; }

    public string PhoneNo { get; set; } = null!;

    public bool IsDelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Section Section { get; set; } = null!;
}
