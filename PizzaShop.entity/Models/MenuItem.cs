using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class MenuItem
{
    public int ItemId { get; set; }

    public int CategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public float Price { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDefault { get; set; }

    public float Tax { get; set; }

    public int Quantity { get; set; }

    public string Shortcode { get; set; } = null!;

    public string? Image { get; set; }

    public int UnitId { get; set; }

    public string FoodType { get; set; } = null!;

    public bool IsFav { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingItemModifierGroup> MappingItemModifierGroups { get; set; } = new List<MappingItemModifierGroup>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Unit Unit { get; set; } = null!;
}
