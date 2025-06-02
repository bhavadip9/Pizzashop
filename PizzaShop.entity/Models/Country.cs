using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
