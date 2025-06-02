

using System.ComponentModel.DataAnnotations;

namespace Pizzashop.entity.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool Status { get; set; }

        public bool RememberMe { get; set; }
    }
}