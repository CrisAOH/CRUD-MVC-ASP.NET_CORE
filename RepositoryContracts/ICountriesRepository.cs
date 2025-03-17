using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic fot managing Person entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Country object</returns>
        Task<Country> AddCountry (Country country);

        /// <summary>
        /// Returns all countries
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the gicen country ID.
        /// </summary>
        /// <param name="countryID">Country ID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryID (Guid countryID);

        /// <summary>
        /// Returns a country object based on the given country name.
        /// </summary>
        /// <param name="countryName">Country name</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName (string countryName);
    }
}
