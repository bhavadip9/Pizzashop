using System;
using System.Collections.Generic;

namespace PizzaShop.entity.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string UserName { get; set; } = null!;

    public string? Image { get; set; }

    public int Roles { get; set; }

    public int CountryName { get; set; }

    public int StateName { get; set; }

    public int CityName { get; set; }

    public int ZipCode { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? RememberMe { get; set; }

    public string? ResetToken { get; set; }

    public string Address { get; set; } = null!;

    public bool Status { get; set; }

    public string Phone { get; set; } = null!;

    public bool IsDelete { get; set; }

    public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; } = new List<Category>();

    public virtual ICollection<Category> CategoryModifiedByNavigations { get; set; } = new List<Category>();

    public virtual City CityNameNavigation { get; set; } = null!;

    public virtual Country CountryNameNavigation { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerModifiedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Feedback> FeedbackCreatedByNavigations { get; set; } = new List<Feedback>();

    public virtual ICollection<Feedback> FeedbackModifiedByNavigations { get; set; } = new List<Feedback>();

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseModifiedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Invoice> InvoiceCreatedByNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<Invoice> InvoiceModifiedByNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<MappingModifierModifiergroup> MappingModifierModifiergroupCreatedByNavigations { get; set; } = new List<MappingModifierModifiergroup>();

    public virtual ICollection<MappingModifierModifiergroup> MappingModifierModifiergroupModifiedByNavigations { get; set; } = new List<MappingModifierModifiergroup>();

    public virtual ICollection<MenuItem> MenuItemCreatedByNavigations { get; set; } = new List<MenuItem>();

    public virtual ICollection<MenuItem> MenuItemModifiedByNavigations { get; set; } = new List<MenuItem>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<ModifierGroup> ModifierGroupCreatedByNavigations { get; set; } = new List<ModifierGroup>();

    public virtual ICollection<ModifierGroup> ModifierGroupModifiedByNavigations { get; set; } = new List<ModifierGroup>();

    public virtual ICollection<ModifiersItem> ModifiersItemCreatedByNavigations { get; set; } = new List<ModifiersItem>();

    public virtual ICollection<ModifiersItem> ModifiersItemModifiedByNavigations { get; set; } = new List<ModifiersItem>();

    public virtual ICollection<Order> OrderCreatedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderModifiedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<PermissionTable> PermissionTableCreatedByNavigations { get; set; } = new List<PermissionTable>();

    public virtual ICollection<PermissionTable> PermissionTableModifiedByNavigations { get; set; } = new List<PermissionTable>();

    public virtual ICollection<RolePermissionTable> RolePermissionTables { get; set; } = new List<RolePermissionTable>();

    public virtual Role RolesNavigation { get; set; } = null!;

    public virtual ICollection<Section> SectionCreatedByNavigations { get; set; } = new List<Section>();

    public virtual ICollection<Section> SectionModifiedByNavigations { get; set; } = new List<Section>();

    public virtual State StateNameNavigation { get; set; } = null!;

    public virtual ICollection<Table> TableCreatedByNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Table> TableModifiedByNavigations { get; set; } = new List<Table>();

    public virtual ICollection<TaxesAndFee> TaxesAndFeeCreatedByNavigations { get; set; } = new List<TaxesAndFee>();

    public virtual ICollection<TaxesAndFee> TaxesAndFeeModifiedByNavigations { get; set; } = new List<TaxesAndFee>();

    public virtual ICollection<WaitingTokenCode> WaitingTokenCodeCreatedByNavigations { get; set; } = new List<WaitingTokenCode>();

    public virtual ICollection<WaitingTokenCode> WaitingTokenCodeModifiedByNavigations { get; set; } = new List<WaitingTokenCode>();
}
