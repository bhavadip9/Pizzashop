using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class OrderedItemModifier
{
    public int OrderedItemModifierId { get; set; }

    public int? Quantity { get; set; }

    public int? ModifierId { get; set; }

    public int? Orderitemid { get; set; }

    public virtual ModifiersItem? Modifier { get; set; }

    public virtual OrderDetail? Orderitem { get; set; }
}
