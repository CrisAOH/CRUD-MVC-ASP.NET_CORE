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
        public CountriesService(bool initialized = true)
        {
            _countries = new List<Country>();
            if (initialized)
            {
                _countries.AddRange(new List<Country>() {
                    new Country()
                    {
                        CountryID = Guid.Parse("3BA2FBCC-B1AA-48C9-8175-1F0D410F8804"),
                        CountryName = "USA"
                    },

                    new Country()
                    {
                        CountryID = Guid.Parse("20DEA190-7755-470B-BD49-FC8E1E6887F5"),
                        CountryName = "Mexico"
                    },

                    new Country()
                    {
                        CountryID = Guid.Parse("A5A63E9C-CF28-4039-B95C-6C5980C1DF7B"),
                        CountryName = "UK"
                    },

                    new Country()
                    {
                        CountryID = Guid.Parse("97580049-15E0-4019-A75A-DE32297D3164"),
                        CountryName = "Canada"
                    },

                    new Country()
                    {
                        CountryID = Guid.Parse("D3BE1E3E-F733-44EB-B812-1ED2DA2B0CD8"),
                        CountryName = "India"
                    },
                });
            }
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
