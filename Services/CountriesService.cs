using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //PRIVATE FIELD
        private readonly List<Country> _countries;

        //CONSTRUCTOR
        public CountriesService()
        {
            _countries = new List<Country>();
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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

            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Ese país ya existe.");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

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

            Country? countryResponseFromList = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (countryResponseFromList == null)
            {
                return null;
            }

            return countryResponseFromList.ToCountryResponse();
        }
    }
}
