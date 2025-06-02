

using Pizzashop.entity.ViewModels;

namespace PizzaShop.service.Interfaces
{
    public interface ILoginService
    {

        Task Updatepass(ResetPasswordViewModel user);

        public bool PasswordForget(String email);
        public ResetPasswordViewModel resetPassword(string token, string email);
    }
}