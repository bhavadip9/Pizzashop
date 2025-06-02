using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class TableCodeMapping
{
    public int MappingId { get; set; }

    public int? TableId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Table? Table { get; set; }
}
