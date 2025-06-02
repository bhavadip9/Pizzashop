using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class OrderTaxMapping
{
    public int OrderTaxId { get; set; }

    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public float? TaxAmount { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual TaxesAndFee Tax { get; set; } = null!;
}
