using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to be added</param>
        /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>Returns a list of CountryResponse objects</returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Returns a country response object based on the given country id
        /// </summary>
        /// <param name="countryID">The ID (guid) to search</param>
        /// <returns>Matching country as CountryResponse</returns>

        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}