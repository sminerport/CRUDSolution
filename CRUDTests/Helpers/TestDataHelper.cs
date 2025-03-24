using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests.Helpers
{
    public static class TestDataHelper
    {
        /// <summary>
        /// Returns a valid PersonAddRequest.
        /// </summary>
        /// <returns>PersonAddRequest</returns>
        public static PersonAddRequest CreateDefaultPersonAddRequest(
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
                Email =" smith@example.com",
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

        /// <summary>
        /// Returns a valid CountryAddRequest.
        /// </summary>
        /// <param name="countryName">The name of the country to addr.</param>
        /// <returns>CountryAddRequest</returns>
        public static CountryAddRequest CreateCountryAddRequest(string countryName = "USA")
        {
            return new CountryAddRequest
            {
                CountryName = countryName
            };
        }
    }
}