using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;

namespace PizzaShop.entity.Models;

public partial class NewPizzashopContext : DbContext
{
    public NewPizzashopContext()
    {
    }

    public NewPizzashopContext(DbContextOptions<NewPizzashopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MappingItemModifierGroup> MappingItemModifierGroups { get; set; }

    public virtual DbSet<MappingModifierModifiergroup> MappingModifierModifiergroups { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }

    public virtual DbSet<ModifiersItem> ModifiersItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderTableMapping> OrderTableMappings { get; set; }

    public virtual DbSet<OrderTaxMapping> OrderTaxMappings { get; set; }

    public virtual DbSet<OrderedItemModifier> OrderedItemModifiers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PermissionTable> PermissionTables { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermissionTable> RolePermissionTables { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TaxesAndFee> TaxesAndFees { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WaitingTokenCode> WaitingTokenCodes { get; set; }

    public virtual DbSet<WaitingUser> WaitingUsers { get; set; }

    public DbSet<FlatOrderDto> FlatOrderDtos { get; set; }

    public DbSet<OrderDetailsVM> OrderDetailsVMs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=New_pizzashop;Username=postgres;Password=Tatva@123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlatOrderDto>().HasNoKey();
        modelBuilder.Entity<OrderDetailsVM>().HasNoKey();

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("category_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CategoryModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("category_modified_by_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(255)
                .HasColumnName("city_name");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("cities_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(255)
                .HasColumnName("country_name");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "customers_email_key").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Visits).HasColumnName("visits");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("customers_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CustomerModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("customers_modified_by_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("Order_id");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FeedbackCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("feedback_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("feedback_Customer_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.FeedbackModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("feedback_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("feedback_Order_id_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.FinalAmount).HasColumnName("final_amount");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("issued_at");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvoiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("invoice_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvoiceModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("invoice_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("invoice_order_id_fkey");
        });

        modelBuilder.Entity<MappingItemModifierGroup>(entity =>
        {
            entity.HasKey(e => e.Mappingid).HasName("mapping_item_modifierGroup_pkey");

            entity.ToTable("mapping_item_modifierGroup");

            entity.Property(e => e.Mappingid).HasColumnName("mappingid");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MaxValue).HasColumnName("maxValue");
            entity.Property(e => e.MinValue).HasColumnName("minValue");
            entity.Property(e => e.ModifierGroupId).HasColumnName("Modifier_group_id");

            entity.HasOne(d => d.Item).WithMany(p => p.MappingItemModifierGroups)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("mapping_item_modifierGroup_item_id_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.MappingItemModifierGroups)
                .HasForeignKey(d => d.ModifierGroupId)
                .HasConstraintName("mapping_item_modifierGroup_Modifier_group_id_fkey");
        });

        modelBuilder.Entity<MappingModifierModifiergroup>(entity =>
        {
            entity.HasKey(e => e.MappingId).HasName("mapping_item_modifier_pkey");

            entity.ToTable("mapping_modifier_modifiergroup", tb => tb.HasComment("Change name Modifier And ModifierGroup"));

            entity.Property(e => e.MappingId)
                .HasDefaultValueSql("nextval('\"mapping_item_modifierGroup_mappingid_seq\"'::regclass)")
                .HasColumnName("mapping_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MappingModifierModifiergroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("mapping_item_modifier_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MappingModifierModifiergroupModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("mapping_item_modifier_modified_by_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.MappingModifierModifiergroups)
                .HasForeignKey(d => d.ModifierGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mapping_item_modifier_modifier_group_id_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.MappingModifierModifiergroups)
                .HasForeignKey(d => d.ModifierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mapping_item_modifier_modifier_id_fkey");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("menu_item_pkey");

            entity.ToTable("menu_item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FoodType)
                .HasDefaultValueSql("'Vegetarian'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("food_type");
            entity.Property(e => e.Image).HasColumnType("character varying");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(true)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsFav)
                .HasDefaultValue(false)
                .HasColumnName("is_fav");
            entity.Property(e => e.ItemName)
                .HasColumnType("character varying")
                .HasColumnName("item_name");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Shortcode)
                .HasColumnType("character varying")
                .HasColumnName("shortcode");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_item_category_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MenuItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("menu_item_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MenuItemModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("menu_item_modified_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_item_unit_id_fkey");
        });

        modelBuilder.Entity<ModifierGroup>(entity =>
        {
            entity.HasKey(e => e.ModifierGroupId).HasName("modifier_group_pkey");

            entity.ToTable("modifier_group");

            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GroupName)
                .HasMaxLength(255)
                .HasColumnName("group_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(true)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modifier_group_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModifierGroupModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modifier_group_modified_by_fkey");
        });

        modelBuilder.Entity<ModifiersItem>(entity =>
        {
            entity.HasKey(e => e.ModifierId).HasName("modifiers_item_pkey");

            entity.ToTable("modifiers_item");

            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierName)
                .HasMaxLength(255)
                .HasColumnName("modifier_name");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifiersItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modifiers_item_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModifiersItemModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modifiers_item_modified_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.ModifiersItems)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_item_unit_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("ordered_pkey");

            entity.ToTable("order", tb => tb.HasComment("Change Name Order"));

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("nextval('ordered_order_id_seq'::regclass)")
                .HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderComment).HasColumnType("character varying");
            entity.Property(e => e.OrderStatus).HasColumnType("character varying");
            entity.Property(e => e.OrderType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Dine-In'::character varying")
                .HasColumnName("order_type");
            entity.Property(e => e.TotalPerson).HasColumnName("Total_Person");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("ordered_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_customer_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrderModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("ordered_modified_by_fkey");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("OrderDetail_pkey");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.ItemComment)
                .HasColumnType("character varying")
                .HasColumnName("Item_Comment");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetail_ItemId_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetail_order_id_fkey");
        });

        modelBuilder.Entity<OrderTableMapping>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("order_table_mapping_pkey");

            entity.ToTable("order_table_mapping");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTableMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_table_mapping_order_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderTableMappings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_table_mapping_table_id_fkey");
        });

        modelBuilder.Entity<OrderTaxMapping>(entity =>
        {
            entity.HasKey(e => e.OrderTaxId).HasName("order_tax_mapping_pkey");

            entity.ToTable("order_tax_mapping");

            entity.Property(e => e.OrderTaxId).HasColumnName("order_tax_id");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
            entity.Property(e => e.TaxId).HasColumnName("tax_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_order_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.TaxId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_tax_id_fkey");
        });

        modelBuilder.Entity<OrderedItemModifier>(entity =>
        {
            entity.HasKey(e => e.OrderedItemModifierId).HasName("ordered_item_modifier_pkey");

            entity.ToTable("ordered_item_modifier");

            entity.Property(e => e.OrderedItemModifierId).HasColumnName("ordered_item_modifier_id");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.Orderitemid).HasColumnName("orderitemid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderedItemModifiers)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("ordered_item_modifier_modifier_id_fkey");

            entity.HasOne(d => d.Orderitem).WithMany(p => p.OrderedItemModifiers)
                .HasForeignKey(d => d.Orderitemid)
                .HasConstraintName("ordered_item_modifier_orderitemid_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("character varying")
                .HasColumnName("payment_method");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("payment_order_id_fkey");
        });

        modelBuilder.Entity<PermissionTable>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permission_table_pkey");

            entity.ToTable("permission_table");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(255)
                .HasColumnName("permission_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PermissionTableCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("permission_table_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PermissionTableModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("permission_table_modified_by_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<RolePermissionTable>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("role_permission_table_pkey");

            entity.ToTable("role_permission_table");

            entity.Property(e => e.RolePermissionId).HasColumnName("role_permission_id");
            entity.Property(e => e.CanAddEdit).HasColumnName("can_add_edit");
            entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            entity.Property(e => e.CanView).HasColumnName("can_view");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.PermissionName).HasColumnType("character varying");
            entity.Property(e => e.Roles).HasColumnName("roles");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RolePermissionTables)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("role_permission_table_created_by_fkey");

            entity.HasOne(d => d.RolesNavigation).WithMany(p => p.RolePermissionTables)
                .HasForeignKey(d => d.Roles)
                .HasConstraintName("role_permission_table_roles_fkey");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("section_pkey");

            entity.ToTable("section");

            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SectionName)
                .HasMaxLength(255)
                .HasColumnName("section_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SectionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("section_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SectionModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("section_modified_by_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.StateName)
                .HasMaxLength(255)
                .HasColumnName("state_name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("states_country_id_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValueSql("'Available'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TableName)
                .HasMaxLength(255)
                .HasColumnName("table_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tables_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TableModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tables_modified_by_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_section_id_fkey");
        });

        modelBuilder.Entity<TaxesAndFee>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("taxes_and_fees_pkey");

            entity.ToTable("taxes_and_fees");

            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(false)
                .HasColumnName("is_enabled");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.TaxName)
                .HasMaxLength(255)
                .HasColumnName("tax_name");
            entity.Property(e => e.TaxType).HasDefaultValue(false);
            entity.Property(e => e.TaxValue)
                .HasPrecision(5, 2)
                .HasColumnName("tax_value");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaxesAndFeeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("taxes_and_fees_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TaxesAndFeeModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("taxes_and_fees_modified_by_fkey");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("unit_pkey");

            entity.ToTable("unit");

            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UnitName)
                .HasMaxLength(50)
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnType("character varying");
            entity.Property(e => e.CityName).HasColumnName("city_name");
            entity.Property(e => e.CountryName).HasColumnName("country_name");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.Image)
                .HasColumnType("character varying")
                .HasColumnName("image");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");
            entity.Property(e => e.ResetToken).HasColumnType("character varying");
            entity.Property(e => e.Roles).HasColumnName("roles");
            entity.Property(e => e.StateName).HasColumnName("state_name");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");
            entity.Property(e => e.ZipCode).HasColumnName("zip_code");

            entity.HasOne(d => d.CityNameNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_city_name_fkey");

            entity.HasOne(d => d.CountryNameNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_country_name_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("users_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("users_modified_by_fkey");

            entity.HasOne(d => d.RolesNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roles_fkey");

            entity.HasOne(d => d.StateNameNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.StateName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_state_name_fkey");
        });

        modelBuilder.Entity<WaitingTokenCode>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("waiting_token_codes_pkey");

            entity.ToTable("waiting_token_codes");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email).HasColumnType("character varying");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("isDelete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PhoneNo)
                .HasColumnType("character varying")
                .HasColumnName("Phone_No");
            entity.Property(e => e.TotalPerson).HasColumnName("total_person");
            entity.Property(e => e.UserName)
                .HasColumnType("character varying")
                .HasColumnName("user_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WaitingTokenCodeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("waiting_token_codes_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.WaitingTokenCodeModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("waiting_token_codes_modified_by_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingTokenCodes)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_token_codes_SectionId_fkey");
        });

        modelBuilder.Entity<WaitingUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("waiting_user");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email).HasColumnType("character varying");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PhoneNo)
                .HasColumnType("character varying")
                .HasColumnName("Phone_No");
            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.TotalPerson).HasColumnName("total_person");
            entity.Property(e => e.UserName)
                .HasColumnType("character varying")
                .HasColumnName("user_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
