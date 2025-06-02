using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Invoice
{
    public decimal? TotalAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? FinalAmount { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int InvoiceId { get; set; }

    public int? OrderId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order? Order { get; set; }
}
