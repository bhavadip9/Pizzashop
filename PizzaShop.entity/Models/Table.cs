using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Table
{
    public int TableId { get; set; }

    public int SectionId { get; set; }

    public string TableName { get; set; } = null!;

    public int Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderTableMapping> OrderTableMappings { get; set; } = new List<OrderTableMapping>();

    public virtual Section Section { get; set; } = null!;
}
