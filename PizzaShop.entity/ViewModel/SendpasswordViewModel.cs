using System.ComponentModel.DataAnnotations;

namespace Pizzashop.entity.ViewModels
{
    public class SendpasswordViewModel
    {
        public string Email { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

       
    }
}