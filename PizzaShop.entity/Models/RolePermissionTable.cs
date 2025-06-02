using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PizzaShop.entity.Models;

public partial class RolePermissionTable
{
    public int RolePermissionId { get; set; }

    public int? Roles { get; set; }

    public bool CanView { get; set; }

    public bool CanAddEdit { get; set; }

    public bool CanDelete { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public string? PermissionName { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
    [JsonIgnore]
    public virtual Role? RolesNavigation { get; set; }
}
