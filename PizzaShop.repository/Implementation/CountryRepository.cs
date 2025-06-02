using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class CountryRepository : ICountryRepository
    {
        private readonly NewPizzashopContext _context;

        public CountryRepository(NewPizzashopContext context)
        {
            _context = context;
        }


        public string GetCountryName(int id)
        {
            var country = _context.Countries.FirstOrDefault(x => x.CountryId == id);
            return country?.CountryName ?? string.Empty;
        }

        public string GetStateName(int id)
        {
            var state = _context.States.FirstOrDefault(x => x.StateId == id);
            return state?.StateName ?? string.Empty;
        }

        public string GetCityName(int id)
        {
            var city = _context.Cities.FirstOrDefault(x => x.CityId == id);
            return city?.CityName ?? string.Empty;
        }


     

        public List<Country> GetAllCountry()
        {
            return _context.Countries.ToList();
        }
        public List<State> GetStatesByCountry(int countryId)
        {
            return _context.States.Where(s => s.CountryId == countryId).ToList();
        }

        public List<City> GetCityByState(int stateId)
        {
            var city = _context.Cities.Where(s => s.StateId == stateId).ToList();
            return city;
        }
    }
}