
using AuthenticationDemo.Utils;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace Pizzashop.Services
{
    public class AuthService : IAuthService
    {



        private readonly IUserRepository _repository;
        private readonly IRoleRepository _roleRepository;


        public AuthService(IUserRepository repository, IRoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }


        /// <summary>
        /// Get AuthenticateUser
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var userData = _repository.GetOneByEmail(email);
            if (userData != null)
            {
                var VerifyPassword = PasswordUtills.VerifyPassword(password, userData.Password);
                if (VerifyPassword)
                {
                    return userData;
                }
                else
                {
                    userData.Password = string.Empty;
                    return userData;
                }
            }
            return null;

        }

        /// <summary>
        /// CheckRole 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role?> CheckRole(string role)
        {
            var roleName = _roleRepository.GetRoleName(role);
            return roleName;
        }


    }
}









