using AutoFixture;

using Entities;

using EntityFrameworkCoreMock;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

using Services;

using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        #region Fields

        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        #endregion Fields

        #region Constructor

        public PersonsServiceTest(
            ITestOutputHelper testOutputHelper
        )
        {
            _fixture = new Fixture();
            List<Country> countriesInitialData = new List<Country>() { };
            List<Person> personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(null);
            _personsService = new PersonsService(dbContext, _countriesService);
            _testOutputHelper = testOutputHelper;
        }

        #endregion Constructor

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Assert
            Func<Task> action = async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            // Act
            Func<Task> action = async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            // Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            // Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

            List<PersonResponse> persons_list = await _personsService.GetAllPersons();

            //Assert.Contains(person_response_from_add, persons_list);
            persons_list.Should().Contain(person_response_from_add);
        }

        #endregion AddPerson

        #region GetPersonPersonID

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

            // Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse? country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email@sample.com")
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);

            // Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_from_add);
        }

        #endregion GetPersonPersonID

        #region GetAllPersons

        // The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            // Act
            List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

            // Assert
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }

        // If we add a person, the GetAllPersons() should return a list with that person
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() {
                person_request_1,
                person_request_2,
                person_request_3,
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            // Act
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

            // print persons_list_from_sort
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_from_get.ToString());
            }

            // Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //Assert.Contains(person_response_from_add, persons_list_from_get);
            //}

            persons_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);
        }

        #endregion GetAllPersons

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() {
                person_request_1,
                person_request_2,
                person_request_3,
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // print persons_list_from_sort
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_from_search in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_from_search.ToString());
            }

            // Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //}

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Scott")
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() {
                person_request_1,
                person_request_2,
                person_request_3,
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");

            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            // print persons_list_from_sort
            _testOutputHelper.WriteLine("Actual: ");

            foreach (PersonResponse person_from_search in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_from_search.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (!string.IsNullOrEmpty(person_response_from_add.PersonName)
                    && person_response_from_add.PersonName.Contains(
                    value: "ma",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(
                        expected: person_response_from_add,
                        collection: persons_list_from_search);
                }
            }

            persons_list_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
        }

        #endregion GetFilteredPersons

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();

            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2,
                person_request_3
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            // Act
            List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPersons(
                allPersons: allPersons,
                sortBy: nameof(Person.PersonName),
                sortOrder: SortOrderOptions.DESC);

            // print persons_list_from_sort
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_from_sort in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_from_sort.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            // Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(
            //        expected: person_response_list_from_add[i],
            //        actual: persons_list_from_sort[i]);
            //}

            //persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);

            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        #endregion GetSortedPersons

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            Func<Task> action = async () =>
            {
                // Act
                await _personsService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = _fixture.Create<PersonUpdateRequest>();

            Func<Task> action = async () =>
            {
                // Act
                await _personsService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John")
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.CountryID, country_response_from_add.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            // Assert
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John")
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.CountryID, country_response_from_add.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            // Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

            // Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().Be(person_response_from_get);
        }

        #endregion UpdatePerson

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Jones")
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.CountryID, country_response_from_add.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            // Act
            bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

            // Assert
            //Assert.True(isDeleted);

            isDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Jones")
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.CountryID, country_response_from_add.CountryID)
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            // Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            // Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion DeletePerson
    }
}