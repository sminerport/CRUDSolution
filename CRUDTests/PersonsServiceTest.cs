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
            PersonAddRequest? request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john_doe@example.com",
                DateOfBirth = new DateTime(1990, 3, 15),
                Gender = GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                Address = "123, Main Street, New York",
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse person_response_from_add = _personsService.AddPerson(request);

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            List<PersonResponse> persons_list = _personsService.GetAllPersons();
            Assert.Contains(person_response_from_add, persons_list);
        }

        #endregion AddPerson

        #region GetPersonPersonID

        // If we supply null as PersonID, we will return null as the value of the `PersonResponse` object
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);

            Assert.Null(person_response_from_get);
        }

        // If we supply a valid person, it should return the valid person details as PersonResponse object
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest country_request = new CountryAddRequest()
            {
                CountryName = "Mexico"
            };

            CountryResponse? country_response = _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john_doe@example.com",
                DateOfBirth = new DateTime(1990, 3, 15),
                Address = "123, Main Street, New York",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "doe@example.com",
                DateOfBirth = new DateTime(1990, 3, 15),
                Address = "123, Main Street, New York",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Jane Doe",
                Email = "jane@example.com",
                DateOfBirth = new DateTime(1980, 7, 19),
                Address = "456, Main Street, New York",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "John Smith",
                Email = "john_smith@example.com",
                DateOfBirth = new DateTime(1970, 5, 25),
                Address = "789, Main Street, New York",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Other,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2,
                person_request_3
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "doe@example.com",
                DateOfBirth = new DateTime(1990, 3, 15),
                Address = "123, Main Street, New York",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Jane Doe",
                Email = "jane@example.com",
                DateOfBirth = new DateTime(1980, 7, 19),
                Address = "456, Main Street, New York",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "John Smith",
                Email = "john_smith@example.com",
                DateOfBirth = new DateTime(1970, 5, 25),
                Address = "789, Main Street, New York",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Other,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2,
                person_request_3
            };

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
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                DateOfBirth = new DateTime(1990, 3, 15),
                Address = "123, Main Street, New York",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                DateOfBirth = new DateTime(1980, 7, 19),
                Address = "456, Main Street, New York (Smith)",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                DateOfBirth = new DateTime(1970, 5, 25),
                Address = "789, Main Street, New York (Rahman)",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Other,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2,
                person_request_3
            };

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
                    && person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }
            }
        }

        #endregion GetFilteredPersons
    }
}