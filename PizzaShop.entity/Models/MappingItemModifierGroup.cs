using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class MappingItemModifierGroup
{
    public int? ItemId { get; set; }

    public int? ModifierGroupId { get; set; }

    public int Mappingid { get; set; }

    public int? MinValue { get; set; }

    public int? MaxValue { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual ModifierGroup? ModifierGroup { get; set; }
}
