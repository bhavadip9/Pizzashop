



using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;

namespace Pizzashop.entity.ViewModels;

public class ProfileViewModel
{
  public int UserId { get; set; }

  [Required(ErrorMessage = "Email is required.")]
  public string? Email { get; set; }

  // [Required(ErrorMessage = "Password is required.")]
  // [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
  // [DataType(DataType.Password)]
  // public string? Password { get; set; }
  [Required(ErrorMessage = "Password is required.")]
  [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
  [DataType(DataType.Password)]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
         ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
  public string? Password { get; set; }
  [Required(ErrorMessage = "First Name is required.")]
  public string? FirstName { get; set; }

  [Required(ErrorMessage = "Last Name is required.")]
  public string? LastName { get; set; }

  [Required(ErrorMessage = "Username is required.")]
  public string? UserName { get; set; }

  public string? RoleName { get; set; }

  [Required(ErrorMessage = "Role selection is required.")]
  public int RoleId { get; set; }


  public int CountryId { get; set; }


  public int StateId { get; set; }


  public int CityId { get; set; }

  [Required(ErrorMessage = "Zip Code is required.")]
  [Range(100000, 999999, ErrorMessage = "Zip code must be exactly 6 digits.")]
  public int? ZipCode { get; set; }

  [Required(ErrorMessage = "Phone number is required.")]
  [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
  public string? Phone { get; set; }

  [Required(ErrorMessage = "Address is required.")]
  [MinLength(5, ErrorMessage = "Address must be at least 5 characters long.")]
  public string? Address { get; set; }

  public string? SelectedRole { get; set; }

  public List<Country> Countries { get; set; } = new();
  public List<State> States { get; set; } = new();
  public List<City> Cities { get; set; } = new();
  public List<Role> Roles { get; set; } = new();

  public int? SelectedCountryId { get; set; }
  public int? SelectedStateId { get; set; }
  public int? SelectedCityId { get; set; }
  public string? CountryName { get; set; }
  public string? StateName { get; set; }
  public string? CityName { get; set; }

  public string? Image { get; set; }
  public bool Status { get; set; }
  public string? Role { get; set; }
  public string? ErrorMessage { get; set; }


  public IFormFile FormFile { get; set; }
}