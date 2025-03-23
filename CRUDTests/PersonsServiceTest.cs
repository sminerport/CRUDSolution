using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

using Services;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
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

            #endregion AddPerson
        }

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
    }
}