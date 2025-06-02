// using System;
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;
// using Pizzashop.entity.ViewModels;

// namespace PizzaShop.entity.Models;

// public partial class NewContext : DbContext
// {
//     public NewContext()
//     {
//     }

//     public NewContext(DbContextOptions<NewContext> options)
//         : base(options)
//     {
//     }


//     // public DbSet<FlatOrderDto> FlatOrderDtos { get; set; }

//     public DbSet<FlatOrderDto> FlatOrderDtos { get; set; }

//     public DbSet<OrderDetailsVM> OrderDetailsVMs { get; set; }



//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=localhost;Database=New_pizzashop;Username=postgres;Password=Tatva@123;");

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {

//         modelBuilder.Entity<FlatOrderDto>().HasNoKey();
//         modelBuilder.Entity<OrderDetailsVM>().HasNoKey();
       
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// }
