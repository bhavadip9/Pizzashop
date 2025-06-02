using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int ItemId { get; set; }

    public int OrderId { get; set; }

    public int Quntity { get; set; }

    public int Prepared { get; set; }

    public string? ItemComment { get; set; }

    public bool IsDeleted { get; set; }

    public virtual MenuItem Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderedItemModifier> OrderedItemModifiers { get; set; } = new List<OrderedItemModifier>();
}
