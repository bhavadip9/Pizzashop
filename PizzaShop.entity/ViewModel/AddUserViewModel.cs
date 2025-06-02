using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;


namespace Pizzashop.entity.ViewModels;

public class AdduserViewModel
{

  public int? UserId { get; set; }

  public string? Email { get; set; }

  [Required]
  public string? Password { get; set; }
  [Required]
  [Display(Name = "Enter FirstName")]
  public string? FirstName { get; set; }

  [Required]
  [Display(Name = "Enter FastName")]
  public string? LastName { get; set; }

  [Required]
  [Display(Name = "Enter UserName")]
  public string? UserName { get; set; }

  // public string? Image { get; set; }

  public int? Roleid { get; set; }


  public int? Countryid { get; set; }

  public int? Stateid { get; set; }

  public int? Cityid { get; set; }

  public int? ZipCode { get; set; }

  [MaxLength]
  public int? Phone { get; set; }

  public int? RoleName { get; set; }

  public string? Address { get; set; }

  [Required]
  [Display(Name = "File")]
  public IFormFile FormFile { get; set; }

  public List<Country> Countries { get; set; } = new();
  public List<State> States { get; set; } = new();
  public List<City> Cities { get; set; } = new();
  public List<Role> Roles { get; set; } = new();



}
