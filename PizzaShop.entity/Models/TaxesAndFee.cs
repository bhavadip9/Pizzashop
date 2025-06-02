using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class TaxesAndFee
{
    public int TaxId { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxValue { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDelete { get; set; }

    public bool IsDefault { get; set; }

    public bool TaxType { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();
}
