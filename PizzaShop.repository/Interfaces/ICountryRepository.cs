
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
    public interface ICountryRepository
    {
        string GetCountryName(int id);

        public string GetCityName(int id);

        public string GetStateName(int id);
      
        public List<Country> GetAllCountry();
        List<State> GetStatesByCountry(int countryId);
        List<City> GetCityByState(int stateId);
    }
}