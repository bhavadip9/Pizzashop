using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
    public interface IUserService
    {

        public List<Country> GetAllCounty();

        public List<State> GetStatesByCountry(int countryId);

        public List<City> GetCityByState(int stateId);

        Task<SendpasswordViewModel> SendPassword(string Email, string Password);

        public Task AddUser(ProfileViewModel user);
        public ProfileViewModel EditUser(ProfileViewModel profile);

        public bool DeleteUser(int id);

        public ProfileViewModel EditUser(int id);

        public List<Role> GetAllRole();

        public Task<AddPaginationViewmodel<UserListViewModel>> GetAllUser(int page, int pageSize, string search, string sortBy, string sortbyrole);

        public Task DeleteAsync(int id);
    }
}