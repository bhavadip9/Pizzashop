using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual ICollection<ModifiersItem> ModifiersItems { get; set; } = new List<ModifiersItem>();
}
