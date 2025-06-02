using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class City
{
    public int CityId { get; set; }

    public int? StateId { get; set; }

    public string CityName { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual State? State { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
