
using PizzaShop.service.Interfaces;
using PizzaShop.entity.Models;
using Pizzashop.entity.ViewModels;
using PizzaShop.repository.Interfaces;
using AuthenticationDemo.Utils;

namespace PizzaShop.service.Implementation
{
    public class LoginService : ILoginService
    {
        private readonly IloginRepository _repository;

        public LoginService(IloginRepository repository)
        {
            _repository = repository;
        }



        


        /// <summary>
        ///  Password Page
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task Updatepass(ResetPasswordViewModel user)
        {
            try
            {
                if (user.Email == null)
                {
                    return;
                }
                User userdata = _repository.GetOneByEmail(user.Email);
               // userdata.Password = user.NewPassword;
                userdata.Password = PasswordUtills.HashPassword(user.NewPassword);
                await _repository.UpdatePassword(userdata);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Password Forget Page
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public bool PasswordForget(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine(email);
                return true;
            }
            var user = _repository.GetOneByEmail(email);
            if (user == null)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Reset Password Page
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public ResetPasswordViewModel resetPassword(string token, string email)
        {

            ResetPasswordViewModel change = new ResetPasswordViewModel();
            change.Token = token;
            change.Email = email;

            return change;

        }
    }
}