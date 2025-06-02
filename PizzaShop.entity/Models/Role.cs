using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PizzaShop.entity.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<RolePermissionTable> RolePermissionTables { get; set; } = new List<RolePermissionTable>();
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
