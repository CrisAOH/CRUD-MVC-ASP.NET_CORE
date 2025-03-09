using System;
using System.Collections.Generic;
using ServiceContracts;
using Entities;
using Services;
using ServiceContracts.DTO;
using Xunit;


namespace CRUD_Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry

        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            
            //Act
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countriesFromGetAllCountries);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actualCountryResponseList);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryRequestList = new List<CountryAddRequest>()
            {
                new CountryAddRequest()
                {
                    CountryName = "USA"
                },
                new CountryAddRequest()
                {
                    CountryName = "UK"
                }
            };

            //Act
            List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();
            foreach (CountryAddRequest countryRequest in countryRequestList)
            {
                countriesListFromAddCountry.Add(_countriesService.AddCountry(countryRequest));
            }

            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            foreach (CountryResponse expectedCountry in countriesListFromAddCountry)
            {
                Assert.Contains(expectedCountry, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryByCountryID

        //Si se proporciona null como CountryID, debería retornar null como CountryResponse
        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? countryResponseFromGetMethod = _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(countryResponseFromGetMethod);
        }

        //Si se proporciona un CountryID válido, debe retornar los detalles del país como CountryResponse object
        [Fact]
        public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponseFromGet = _countriesService.GetCountryByCountryID(countryResponseFromAdd.CountryID);

            //Assert
            Assert.Equal(countryResponseFromAdd, countryResponseFromGet);
        }

        #endregion
    }
}
