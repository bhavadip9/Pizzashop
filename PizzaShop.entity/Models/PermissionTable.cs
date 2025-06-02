using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class PermissionTable
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }
}
