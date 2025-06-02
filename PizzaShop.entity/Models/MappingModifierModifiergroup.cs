using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

/// <summary>
/// Change name Modifier And ModifierGroup
/// </summary>
public partial class MappingModifierModifiergroup
{
    public int ModifierGroupId { get; set; }

    public int ModifierId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int MappingId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ModifiersItem Modifier { get; set; } = null!;

    public virtual ModifierGroup ModifierGroup { get; set; } = null!;
}
