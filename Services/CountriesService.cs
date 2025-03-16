using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //PRIVATE FIELD
        private readonly PersonsDbContext _db;

        //CONSTRUCTOR
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //MEDIANTE ESTA SENTENCIA SE PERMITE QUE TODAS LAS PRUEBAS FALLEN, PUES SIEMPRE SE ENVIARÁ ESTA EXCEPCIÓN EN VEZ DE LO SOLICITADO. ES CORRECTO QUE, DURANTE LA PRIMERA IMPLEMENTACIÓN DE LAS PRUEBAS, SE PERMITA QUE TODAS FALLEN.
            //throw new NotImplementedException();}
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Ese país ya existe.");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async  Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? countryResponseFromList = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);

            if (countryResponseFromList == null)
            {
                return null;
            }

            return countryResponseFromList.ToCountryResponse();
        }
    }
}
