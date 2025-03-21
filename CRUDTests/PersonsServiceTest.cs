using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

using Services;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;

        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
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
                CountryID = null,
                Address = "123, Main Street, New York",
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse response = _personsService.AddPerson(request);
            Assert.True(response.PersonID != Guid.Empty);

            #endregion AddPerson
        }
    }
}