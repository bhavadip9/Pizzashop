using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int? CustomerId { get; set; }

    public int? OrderId { get; set; }

    public int? FoodRating { get; set; }

    public int? ServiceRating { get; set; }

    public int? AmbienceRating { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order? Order { get; set; }
}
