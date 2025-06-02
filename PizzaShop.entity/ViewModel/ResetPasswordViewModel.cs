using System.ComponentModel.DataAnnotations;

namespace Pizzashop.entity.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}