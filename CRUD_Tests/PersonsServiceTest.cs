using System;
using ServiceContracts;
using Xunit;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;

namespace CRUD_Tests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person ID
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person Name",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            //Act
            PersonResponse personResponseFromAdd = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> personsList = _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponseFromAdd.PersonID != Guid.Empty);
            Assert.Contains(personResponseFromAdd, personsList);
        }

        #endregion

        #region GetPersonByPersonID

        //iF WE SUPPLY NULL AS PersonID, it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonId()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? personResponseFromGet = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(personResponseFromGet);
        }

        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest countryRequest = new CountryAddRequest()
            {
                CountryName = "Canada"
            };
            CountryResponse countryResponse = _countriesService.AddCountry(countryRequest);

            //Act
            PersonAddRequest personRequest = new PersonAddRequest()
            {
                PersonName = "Person Name",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = false
            };
            PersonResponse personResponseFromAdd = _personsService.AddPerson(personRequest);
            PersonResponse? personResponseFromGet = _personsService.GetPersonByPersonID(personResponseFromAdd.PersonID);

            //Assert
            Assert.Equal(personResponseFromAdd, personResponseFromGet);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> personsFromGet = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personsFromGet);
        }

        //First, we will add few persons, then, when we call GetAllPersons() it should return the same persons that were added
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

            PersonAddRequest personRequest1 = new PersonAddRequest()
            {
                PersonName = "Person Name",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = countryResponse1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = false
            };
            PersonAddRequest personRequest2 = new PersonAddRequest()
            {
                PersonName = "Person Name2",
                Email = "persona2@example.com",
                Address = "Sample Address2",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest personRequest3 = new PersonAddRequest()
            {
                PersonName = "Person Name3",
                Email = "persona3@example.com",
                Address = "Sample Address3",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Other,
                DateOfBirth = DateTime.Parse("2003-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personRequests = new List<PersonAddRequest>()
            {
                personRequest1,
                personRequest2,
                personRequest3
            };

            List<PersonResponse> personResponseListFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in personRequests)
            {
                PersonResponse personResponse = _personsService.AddPerson(personRequest);
                personResponseListFromAdd.Add(personResponse);
            }

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                _testOutputHelper.WriteLine(personResponseFromAdd.ToString());
            }

            //Act
            List<PersonResponse> personsListFromGet = _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromGet)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                Assert.Contains(personResponseFromAdd, personsListFromGet);
            }
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

            PersonAddRequest personRequest1 = new PersonAddRequest()
            {
                PersonName = "Person Name",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = countryResponse1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = false
            };
            PersonAddRequest personRequest2 = new PersonAddRequest()
            {
                PersonName = "Person Name2",
                Email = "persona2@example.com",
                Address = "Sample Address2",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest personRequest3 = new PersonAddRequest()
            {
                PersonName = "Person Name3",
                Email = "persona3@example.com",
                Address = "Sample Address3",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Other,
                DateOfBirth = DateTime.Parse("2003-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personRequests = new List<PersonAddRequest>()
            {
                personRequest1,
                personRequest2,
                personRequest3
            };

            List<PersonResponse> personResponseListFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in personRequests)
            {
                PersonResponse personResponse = _personsService.AddPerson(personRequest);
                personResponseListFromAdd.Add(personResponse);
            }

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                _testOutputHelper.WriteLine(personResponseFromAdd.ToString());
            }

            //Act
            List<PersonResponse> personsListFromSearch = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                Assert.Contains(personResponseFromAdd, personsListFromSearch);
            }
        }

        //First we add a few persons, the we search based on the person's name with some search string. It should return matching persons.
        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

            PersonAddRequest personRequest1 = new PersonAddRequest()
            {
                PersonName = "Person",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = countryResponse1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = false
            };
            PersonAddRequest personRequest2 = new PersonAddRequest()
            {
                PersonName = "Name",
                Email = "persona2@example.com",
                Address = "Sample Address2",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest personRequest3 = new PersonAddRequest()
            {
                PersonName = "3",
                Email = "persona3@example.com",
                Address = "Sample Address3",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Other,
                DateOfBirth = DateTime.Parse("2003-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personRequests = new List<PersonAddRequest>()
            {
                personRequest1,
                personRequest2,
                personRequest3
            };

            List<PersonResponse> personResponseListFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in personRequests)
            {
                PersonResponse personResponse = _personsService.AddPerson(personRequest);
                personResponseListFromAdd.Add(personResponse);
            }

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                _testOutputHelper.WriteLine(personResponseFromAdd.ToString());
            }

            //Act
            List<PersonResponse> personsListFromSearch = _personsService.GetFilteredPersons(nameof(Person.PersonName), "na");

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                if (personResponseFromAdd.PersonName != null)
                {
                    if (personResponseFromAdd.PersonName.Contains("na", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personResponseFromAdd, personsListFromSearch);
                    }
                }
            }
        }

        #endregion

        #region GetSortedPersons
        //Sort based on PersonName in DESC, IT SHOULD RETURN PERSONS LIST IN DESCENDING ORDER PERSON NAME
        [Fact]
        public void GetSortedPersons()
        {
            //Arrange
            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

            PersonAddRequest personRequest1 = new PersonAddRequest()
            {
                PersonName = "Person",
                Email = "persona@example.com",
                Address = "Sample Address",
                CountryID = countryResponse1.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = false
            };
            PersonAddRequest personRequest2 = new PersonAddRequest()
            {
                PersonName = "Name",
                Email = "persona2@example.com",
                Address = "Sample Address2",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Female,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest personRequest3 = new PersonAddRequest()
            {
                PersonName = "3",
                Email = "persona3@example.com",
                Address = "Sample Address3",
                CountryID = countryResponse2.CountryID,
                Gender = GenderOptions.Other,
                DateOfBirth = DateTime.Parse("2003-01-01"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> personRequests = new List<PersonAddRequest>()
            {
                personRequest1,
                personRequest2,
                personRequest3
            };

            List<PersonResponse> personResponseListFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in personRequests)
            {
                PersonResponse personResponse = _personsService.AddPerson(personRequest);
                personResponseListFromAdd.Add(personResponse);
            }

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseFromAdd in personResponseListFromAdd)
            {
                _testOutputHelper.WriteLine(personResponseFromAdd.ToString());
            }

            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            //Act
            List<PersonResponse> personsListFromSort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSort)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            personResponseListFromAdd = personResponseListFromAdd.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < personResponseListFromAdd.Count; i++)
            {
                Assert.Equal(personResponseListFromAdd[i], personsListFromSort[i]);
            }
        }

        #endregion

        #region UpdatePerson

        //Se proporciona nulo como PersonUpdateRequest, lanza excepción
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Se proporciona un ID inválido, lanza excepción
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Cuando el nombre de la persona es nulo, lanza excepción
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jhon",
                CountryID = countryResponseFromAdd.CountryID,
                Email = "jhon@example.com",
                Address = "address",
                Gender = GenderOptions.Male
            };
            PersonResponse personResponseFromAdd = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest? personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Se añade una nueva persona y se intenta actualizar su nombre y su email.
        [Fact]
        public void UpdatePerson_PersonFullDetails()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jhon",
                CountryID = countryResponseFromAdd.CountryID,
                Email = "persona@example.com",
                Address = "Sample Address",
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse personResponseFromAdd = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest? personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "William";
            personUpdateRequest.Email = "william@example.com";

            //Act
            PersonResponse personResponseFromUpdate = _personsService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponseFromGet = _personsService.GetPersonByPersonID(personResponseFromUpdate.PersonID);

            //Assert
            Assert.Equal(personResponseFromGet, personResponseFromUpdate);
        }
        #endregion

        #region DeletePerson

        //Se proporciona un ID válido, debe retornar true
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponseFromAdd = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Jhon",
                CountryID = countryResponseFromAdd.CountryID,
                DateOfBirth = Convert.ToDateTime("2010-01-01"),
                Email = "jhon@example.com",
                Address = "address",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonResponse personResponseFromAdd = _personsService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = _personsService.DeletePerson(personResponseFromAdd.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //Se proporciona un ID válido, debe retornar true
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Arrange

            //Act
            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}
