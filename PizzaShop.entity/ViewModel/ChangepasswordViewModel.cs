using System.ComponentModel.DataAnnotations;

namespace Pizzashop.entity.ViewModels;

    public class ChangepasswordViewModel
    {

       
     
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    
       [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "Confirmation Password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

       
    }
