using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class ModifierGroup
{
    public int ModifierGroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingItemModifierGroup> MappingItemModifierGroups { get; set; } = new List<MappingItemModifierGroup>();

    public virtual ICollection<MappingModifierModifiergroup> MappingModifierModifiergroups { get; set; } = new List<MappingModifierModifiergroup>();

    public virtual User? ModifiedByNavigation { get; set; }
}
