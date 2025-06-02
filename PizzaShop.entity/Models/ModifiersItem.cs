using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class ModifiersItem
{
    public int ModifierId { get; set; }

    public string ModifierName { get; set; } = null!;

    public decimal Rate { get; set; }

    public string? Description { get; set; }

    public int UnitId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int Quantity { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingModifierModifiergroup> MappingModifierModifiergroups { get; set; } = new List<MappingModifierModifiergroup>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderedItemModifier> OrderedItemModifiers { get; set; } = new List<OrderedItemModifier>();

    public virtual Unit Unit { get; set; } = null!;
}
