
using AuthenticationDemo.Utils;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        private readonly IHomeRepository _homeRepository;

        private readonly ICountryRepository _countryRepository;

        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IHomeRepository homeRepository, ICountryRepository countryRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _homeRepository = homeRepository;
            _countryRepository = countryRepository;
            _roleRepository = roleRepository;


        }
        public async Task<AddPaginationViewmodel<UserListViewModel>> GetAllUser(int page, int pageSize, string search, string sortBy, string sortbyrole)
        {
            var userget = await _userRepository.GetAllUser();
            var email = _homeRepository.GetUserEmail();

            int tableCount;

            var userViewModels = userget.Select(c => new UserListViewModel
            {
                UserName = c.UserName,
                UserId = c.UserId,
                RoleName = c.RolesNavigation.RoleName,
                Email = c.Email,
                Status = c.Status,
                Phone = c.Phone,
                Image = c.Image,

            }).ToList();
            userViewModels = (sortBy, sortbyrole) switch
            {
                ("asc_name", "asc_name") => userViewModels.OrderBy(u => u.UserName).ThenBy(u => u.RoleName).ToList(),
                ("asc_name", "dec_name") => userViewModels.OrderBy(u => u.UserName).ThenByDescending(u => u.RoleName).ToList(),
                ("dec_name", "asc_name") => userViewModels.OrderByDescending(u => u.UserName).ThenBy(u => u.RoleName).ToList(),
                ("dec_name", "dec_name") => userViewModels.OrderByDescending(u => u.UserName).ThenByDescending(u => u.RoleName).ToList(),
                ("asc_name", _) => userViewModels.OrderBy(u => u.UserName).ToList(),
                ("dec_name", _) => userViewModels.OrderByDescending(u => u.UserName).ToList(),
                (_, "asc_name") => userViewModels.OrderBy(u => u.RoleName).ToList(),
                (_, "dec_name") => userViewModels.OrderByDescending(u => u.RoleName).ToList(),
                _ => userViewModels.OrderBy(u => u.UserName).ToList()
            };
           
            if (!string.IsNullOrEmpty(search))
            {
                userViewModels = userViewModels.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            tableCount = userViewModels.Count;

            userViewModels = userViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var user = new AddPaginationViewmodel<UserListViewModel>
            {
                Items = userViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
                Email = email
            };

            return user;
        }



        public List<Role> GetAllRole()
        {
            return _roleRepository.GetAllRole();
        }
        public List<Country> GetAllCounty()
        {
            return _countryRepository.GetAllCountry();
        }
        public List<State> GetStatesByCountry(int CountryId)
        {
            return _countryRepository.GetStatesByCountry(CountryId);
        }
        public List<City> GetCityByState(int stateId)
        {
            var city = _countryRepository.GetCityByState(stateId);
            return city;
        }


        public Task DeleteAsync(int id)
        {
            return _userRepository.DeleteAsync(id);
        }

        #region Add User

        /// <summary>
        /// Add User Service Send Password
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<SendpasswordViewModel> SendPassword(string Email, string Password)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return null;
                }

                SendpasswordViewModel data = new SendpasswordViewModel();
                data.Email = Email;
                data.Password = Password;
                await Task.CompletedTask;

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        /// <summary>
        /// Add User Service
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task AddUser(ProfileViewModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    return null;
                }

                ProfileViewModel userModel = new ProfileViewModel();

                userModel.UserName = user.UserName;
                userModel.Email = user.Email;
                // userModel.Password = user.Password;
                userModel.Password = PasswordUtills.HashPassword(user.Password);
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.RoleId = user.RoleId;
                userModel.Phone = user.Phone;
                userModel.Image = user.Image;
                userModel.ZipCode = user.ZipCode;
                userModel.CountryId = user.CountryId;
                userModel.StateId = user.StateId;
                userModel.CityId = user.CityId;
                userModel.Address = user.Address;

                return _homeRepository.AddUser(userModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }


        /// <summary>
        /// Delete User Service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(int id)
        {
            try
            {
                if (id == 0)
                {
                    return false;
                }
                var user = _userRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// Edit User Service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProfileViewModel EditUser(int id)
        {
            try
            {
                if (id == 0)
                {
                    return null;
                }
                var user = _userRepository.GetOneByIdAsync(id);
                ProfileViewModel profile = new ProfileViewModel();
                profile.UserId = user.UserId;
                profile.Email = user.Email;
                //profile.Password = user.Password;
                profile.Password = PasswordUtills.HashPassword(user.Password);
                profile.FirstName = user.FirstName;
                profile.LastName = user.LastName;
                profile.UserName = user.UserName;
                profile.Image = user.Image;
                profile.Address = user.Address;
                profile.Phone = user.Phone;
                profile.ZipCode = user.ZipCode;
                profile.Status = user.Status;
                profile.CountryId = user.CountryName;
                profile.CityId = user.CityName;
                profile.StateId = user.StateName;
                profile.RoleId = user.Roles;


                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        /// <summary>
        /// Edit User Service
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ProfileViewModel EditUser(ProfileViewModel profile)
        {
            try
            {
                // var email = profile.Email;
                // var user = _homeRepository.GetOneByEmail(email);
                var user = _homeRepository.GetUser(profile.UserId);

                user.Email = profile.Email;
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.UserName = profile.UserName;
                if (profile.Image == null)
                {
                    user.Image = user.Image;
                }
                else
                {
                    user.Image = profile.Image;
                }
                user.Address = profile.Address;
                user.Phone = profile.Phone;
                user.CountryName = profile.CountryId;
                user.StateName = profile.StateId;
                user.CityName = profile.CityId;
                user.ZipCode = profile.ZipCode ?? 0;
                user.Status = profile.Status;
                user.Roles = profile.RoleId;
                _homeRepository.UpdateUser(user);

                return new ProfileViewModel
                {
                    Email = user.Email,
                    UserId = user.UserId,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Image = user.Image,
                    Address = user.Address,
                    Phone = user.Phone,
                    CountryId = user.CountryName,
                    StateId = user.StateName,
                    CityId = user.CityName,
                    RoleId = user.Roles,
                    ZipCode = user.ZipCode,
                    Status = user.Status
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        #endregion

    }
}