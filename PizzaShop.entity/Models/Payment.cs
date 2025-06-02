using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual Order? Order { get; set; }
}
