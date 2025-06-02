using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class OrderTableMapping
{
    public int OrderDetailId { get; set; }

    public int TableId { get; set; }

    public int OrderId { get; set; }

    public bool IsDelete { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
