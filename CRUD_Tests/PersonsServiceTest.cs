using System;
using ServiceContracts;
using Xunit;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using RepositoryContracts;
using Moq;
using AutoFixture.Kernel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CRUD_Tests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            _personsService = new PersonsService(_personsRepository);
            
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
        }

        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person ID
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse personResponseFromAdd = await _personsService.AddPerson(personAddRequest);
            personResponseExpected.PersonID = personResponseFromAdd.PersonID;

            //Assert
            Assert.True(personResponseFromAdd.PersonID != Guid.Empty);
            Assert.Equal(personResponseExpected, personResponseFromAdd);
        }

        #endregion

        #region GetPersonByPersonID

        //iF WE SUPPLY NULL AS PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonId_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? personResponseFromGet = await _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(personResponseFromGet);
        }

        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public  async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "something@example.com").With(temp => temp.Country, null as Country).Create();

            PersonResponse personResponseExpected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? personResponseFromGet = await _personsService.GetPersonByPersonID(person.PersonID);

            //Assert
            Assert.Equal(personResponseExpected, personResponseFromGet);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> personsFromGet = await _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personsFromGet);
        }

        //First, we will add few persons, then, when we call GetAllPersons() it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "something@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "anybody@example.com").With(temp => temp.Country, null as Country).Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponseExpected.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> personsListFromGet = await _personsService.GetAllPersons();

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromGet)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                Assert.Contains(personResponseExpected, personsListFromGet);
            }
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "something@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "anybody@example.com").With(temp => temp.Country, null as Country).Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponseExpected.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> personsListFromSearch = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                Assert.Contains(personResponseExpected, personsListFromSearch);
            }
        }

        //First we add a few persons, the we search based on the person's name with some search string. It should return matching persons.
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccesful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "something@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "anybody@example.com").With(temp => temp.Country, null as Country).Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponseExpected.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> personsListFromSearch = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            //Assert
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                Assert.Contains(personResponseExpected, personsListFromSearch);
            }
        }

        #endregion

        #region GetSortedPersons
        //Sort based on PersonName in DESC, IT SHOULD RETURN PERSONS LIST IN DESCENDING ORDER PERSON NAME
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "something@example.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "anybody@example.com").With(temp => temp.Country, null as Country).Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            _testOutputHelper.WriteLine("EXPECTED: ");
            foreach (PersonResponse personResponseExpected in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponseExpected.ToString());
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            //Act
            List<PersonResponse> personsListFromSort = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("ACTUAL: ");
            foreach (PersonResponse personResponseFromGet in personsListFromSort)
            {
                _testOutputHelper.WriteLine(personResponseFromGet.ToString());
            }

            personResponseListExpected = personResponseListExpected.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < personResponseListExpected.Count; i++)
            {
                Assert.Equal(personResponseListExpected[i], personsListFromSort[i]);
            }
        }

        #endregion

        #region UpdatePerson

        //Se proporciona nulo como PersonUpdateRequest, lanza excepción
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Se proporciona un ID inválido, lanza excepción
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Cuando el nombre de la persona es nulo, lanza excepción
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse personResponseFromAdd = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //Se añade una nueva persona y se intenta actualizar su nombre y su email.
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.PersonName, "Natalia").With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();
            
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);

            //Assert
            Assert.Equal(personResponseExpected, personResponseFromUpdate);
        }
        #endregion

        #region DeletePerson

        //Se proporciona un ID válido, debe retornar true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.PersonName, "Natalia").With(temp => temp.Email, "someone@example.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsService.DeletePerson(person.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //Se proporciona un ID válido, debe retornar true
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange

            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}
