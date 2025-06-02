using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Section
{
    public int SectionId { get; set; }

    public string SectionName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<WaitingTokenCode> WaitingTokenCodes { get; set; } = new List<WaitingTokenCode>();
}
