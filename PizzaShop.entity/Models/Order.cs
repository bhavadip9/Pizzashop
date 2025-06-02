using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

/// <summary>
/// Change Name Order
/// </summary>
public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public float Amount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public int TotalPerson { get; set; }

    public bool IsDelete { get; set; }

    public string? OrderComment { get; set; }

    public float? SubTotal { get; set; }

    public float? OtherTax { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderTableMapping> OrderTableMappings { get; set; } = new List<OrderTableMapping>();

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
