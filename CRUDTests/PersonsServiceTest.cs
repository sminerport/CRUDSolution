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
            _countriesService = new CountriesService(initialize: false);
            _testOutputHelper = testOutputHelper;
        }

        #endregion Constructor

        #region HelperMethods

        private PersonAddRequest CreatePersonAddRequest(
            string? personName = "Babe Ruth",
            string? email = "babe_ruth@gmail.com",
            DateTime? dateOfBirth = null,
            GenderOptions gender = GenderOptions.Male,
            Guid? countryID = null,
            string? address = "123, Main Street, New York",
            bool receiveNewsLetters = true)
        {
            return new PersonAddRequest
            {
                PersonName = personName,
                Email = email,
                DateOfBirth = dateOfBirth ?? new DateTime(1990, 3, 15),
                Gender = gender,
                CountryID = countryID ?? Guid.NewGuid(),
                Address = address,
                ReceiveNewsLetters = receiveNewsLetters
            };
        }

        private static readonly PersonAddRequest[] _samplePersons = new[]
        {
            new PersonAddRequest
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                DateOfBirth = new DateTime(1990, 10, 31),
                Gender = GenderOptions.Female,
                CountryID = Guid.NewGuid(),
                Address = "876, Elm Street, San Francisco",
                ReceiveNewsLetters = true
            },
            new PersonAddRequest
            {
                PersonName = "Smith",
                Email = " smith@example.com",
                DateOfBirth = new DateTime(1975, 5, 15),
                Gender = GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                Address = "567, Pine Street, Los Angeles",
                ReceiveNewsLetters = false
            },
            new PersonAddRequest
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                DateOfBirth = new DateTime(1985, 7, 25),
                Gender = GenderOptions.Other,
                CountryID = Guid.NewGuid(),
                Address = "345, Oak Street, San Diego",
                ReceiveNewsLetters = true
            }
        };

        public static List<PersonAddRequest> CreateMultipleSamplePersons(int count)
        {
            if (count <= 0)
            {
                return new List<PersonAddRequest>();
            }

            var result = new List<PersonAddRequest>();
            for (int i = 0; i < count; i++)
            {
                var basePerson = _samplePersons[i % _samplePersons.Length];

                // Always clone so we don't keep the same Guid references, etc.
                // You can do a "shallow clone" or new up each property.
                result.Add(new PersonAddRequest
                {
                    PersonName = basePerson.PersonName + (count > _samplePersons.Length ? $"_{i}" : ""),
                    Email = basePerson?.Email?.Replace("@", $"{i}@"),
                    DateOfBirth = basePerson?.DateOfBirth?.AddDays(i),
                    Gender = basePerson.Gender,
                    CountryID = Guid.NewGuid(),
                    Address = basePerson.Address + $" (clone {i})",
                    ReceiveNewsLetters = basePerson.ReceiveNewsLetters
                });
            }

            return result;
        }

        #endregion HelperMethods

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
            PersonAddRequest request = CreatePersonAddRequest();

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
            CountryAddRequest country_request = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse? country_response = _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = CreatePersonAddRequest(countryID: country_response.CountryID);

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
            CountryAddRequest country_request_1 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryAddRequest country_request_2 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = CreateMultipleSamplePersons(3);

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

            // print persons_list_from_sort
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

        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryAddRequest country_request_2 = new CountryAddRequest
            {
                CountryName = "India"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = CreateMultipleSamplePersons(3);

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

            // print persons_list_from_sort
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

        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "France" };

            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "Germany" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = CreateMultipleSamplePersons(3);

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
        }

        #endregion GetFilteredPersons

        #region GetSortedPersons

        [Fact]
        public void GetSortedPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "France" };

            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "Germany" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            List<PersonAddRequest> person_requests = CreateMultipleSamplePersons(3);

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

            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            // Act
            List<PersonResponse> persons_list_from_sort = _personsService.GetSortedPersons(
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
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(
                    expected: person_response_list_from_add[i],
                    actual: persons_list_from_sort[i]);
            }
        }

        #endregion GetSortedPersons

        #region UpdatePerson

        [Fact]
        public void UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_InvlaidPersonID()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };

            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = CreatePersonAddRequest(
                personName: "John",
                countryID: country_response_from_add.CountryID);

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _personsService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdate()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = CreatePersonAddRequest(
                personName: "John",
                countryID: country_response_from_add.CountryID);

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            // Act
            PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

            // Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }

        #endregion UpdatePerson

        #region DeletePerson

        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = CreatePersonAddRequest(
                personName: "Jones",
                countryID: country_response_from_add.CountryID);

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            // Act
            bool isDeleted = _personsService.DeletePerson(person_response_from_add.PersonID);

            Assert.True(isDeleted);
        }

        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = CreatePersonAddRequest(
                personName: "Jones",
                countryID: country_response_from_add.CountryID);

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            // Act
            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);
        }

        #endregion DeletePerson
    }
}