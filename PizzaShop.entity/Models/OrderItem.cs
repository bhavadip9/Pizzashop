using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? ItemId { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string? ItemComment { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }
}
