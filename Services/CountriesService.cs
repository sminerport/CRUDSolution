using Entities;

using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>() {
                    new Country()
                    {
                        CountryID = Guid.Parse("1B984A3C-0F58-4A52-A1C4-5AF3D061E285"),
                        CountryName = "USA"
                    },
                    new Country()
                    {
                        CountryID = Guid.Parse("F7416243-E291-4ED6-9B51-CA0546B894F0"),
                        CountryName = "Canada"
                    },
                    new Country()
                    {
                        CountryID = Guid.Parse("5D1F3C1E-9222-4262-9480-A4170FEB589B"),
                        CountryName = "Mexico"
                    },
                    new Country()
                    {
                        CountryID = Guid.Parse("151F8B77-A4CC-4948-A360-DD4DE7912E3D"),
                        CountryName = "Brazil"
                    },
                    new Country()
                    {
                        CountryID = Guid.Parse("9D3A7C28-17AA-43DA-AAD7-D1EDBE625D47"),
                        CountryName = "Argentina"
                    }
                });
            }
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            // Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            // Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            // Validation: CountryName should be unique
            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate CountryID
            country.CountryID = Guid.NewGuid();

            // Add country object into _countries
            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? country = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country == null)
                return null;

            return country.ToCountryResponse();
        }
    }
}