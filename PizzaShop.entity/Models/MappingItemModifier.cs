using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class MappingItemModifier
{
    public int ModifierGroupId { get; set; }

    public int ModifierId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int MappingId { get; set; }
}
