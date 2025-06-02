using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.entity.ViewModel
{
    public class PermissionViewModel
    {
        public int RolePermissionId { get; set; }
        public string? PermissionName { get; set; }
        public bool IsChecked { get; set; } // Whether the role has this permission
        public bool CanView { get; set; } // Permission to view
        public bool CanAddEdit { get; set; } // Permission to add or edit
        public bool CanDelete { get; set; } // Permission to delete

        public bool PreviousIsChecked { get; set; }
        public bool PreviousCanView { get; set; }
        public bool PreviousCanAddEdit { get; set; }
        public bool PreviousCanDelete { get; set; }
    }
}