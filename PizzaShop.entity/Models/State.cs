using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class State
{
    public int StateId { get; set; }

    public int? CountryId { get; set; }

    public string StateName { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country? Country { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
