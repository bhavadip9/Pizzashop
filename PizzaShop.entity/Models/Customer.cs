using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Visits { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
