using CRUDTests.Helpers;

using Entities;

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

        #endregion Fields

        #region Constructor

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }

        #endregion Constructor

        #region AddPerson

        [Fact]
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? request = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _personsService.AddPerson(request);
            });
        }

        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? request = new PersonAddRequest()
            {
                PersonName = null
            };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _personsService.AddPerson(request);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest request = TestDataHelper.CreateDefaultPersonAddRequest();

            // Act
            PersonResponse person_response_from_add = _personsService.AddPerson(request);

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            List<PersonResponse> persons_list = _personsService.GetAllPersons();
            Assert.Contains(person_response_from_add, persons_list);
        }

        #endregion AddPerson

        #region GetPersonPersonID

        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);

            // Assert
            Assert.Null(person_response_from_get);
        }

        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest country_request = TestDataHelper.CreateCountryAddRequest("USA");

            CountryResponse? country_response = _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = TestDataHelper.CreateDefaultPersonAddRequest(
                countryID: country_response.CountryID);

            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);

            // Act
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion GetPersonPersonID

        #region GetAllPersons

        // The GetAllPersons() should return an empty list by default
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            // Act
            List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

            Assert.Empty(persons_from_get);
        }

        // If we add a person, the GetAllPersons() should return a list with that person
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = TestDataHelper.CreateCountryAddRequest("USA");
            CountryAddRequest country_request_2 = TestDataHelper.CreateCountryAddRequest("India");

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = TestDataHelper.CreateMultipleSamplePersons(3);

            person_requests[0].CountryID = country_response_1.CountryID;
            person_requests[1].CountryID = country_response_2.CountryID;
            person_requests[2].CountryID = country_response_1.CountryID;

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            // Act
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> persons_list_from_get = _personsService.GetAllPersons();

            // print persons_list_from_search
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_from_get.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }

        #endregion GetAllPersons

        #region GetFilteredPersons

        // If the search text is empty and search by is "PersonName", it should return all persons

        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest country_request_1 = TestDataHelper.CreateCountryAddRequest("USA");

            CountryAddRequest country_request_2 = TestDataHelper.CreateCountryAddRequest("India");

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = TestDataHelper.CreateMultipleSamplePersons(3);

            person_requests[0].CountryID = country_response_1.CountryID;
            person_requests[1].CountryID = country_response_2.CountryID;
            person_requests[2].CountryID = country_response_1.CountryID;

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // print persons_list_from_search
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_from_search in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_from_search.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        // First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest country_request_1 = TestDataHelper.CreateCountryAddRequest("France");

            CountryAddRequest country_request_2 = TestDataHelper.CreateCountryAddRequest("Germany");

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = TestDataHelper.CreateMultipleSamplePersons(3);

            person_requests[0].CountryID = country_response_1.CountryID;
            person_requests[1].CountryID = country_response_2.CountryID;
            person_requests[2].CountryID = country_response_1.CountryID;

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");

            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            // print persons_list_from_search
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
        }

        #endregion GetFilteredPersons
    }
}