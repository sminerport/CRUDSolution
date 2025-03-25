using Entities;

using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        #region Fields

        private List<Person> _persons;
        private readonly ICountriesService _countriesService;

        #endregion Fields

        #region Constructors

        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.AddRange(
                new Person()
                {
                    PersonID = Guid.Parse("75415A1C-90AE-44D3-BBA1-D1889C101180"),
                    PersonName = "Guendolen",
                    Email = "ggavaran0@who.int",
                    DateOfBirth = DateTime.Parse("1990-11-03"),
                    Gender = "Female",
                    CountryID = Guid.Parse("1B984A3C-0F58-4A52-A1C4-5AF3D061E285"),
                    Address = "1 La Follette Street",
                    ReceiveNewsLetters = false
                },
                new Person()
                {
                    PersonID = Guid.Parse("901A574E-3AC8-49D9-B72D-C1639F580F90"),
                    PersonName = "Arthur",
                    Email = "adrance2@admin.ch",
                    DateOfBirth = DateTime.Parse("2000-11-25"),
                    Gender = "Male",
                    CountryID = Guid.Parse("F7416243-E291-4ED6-9B51-CA0546B894F0"),
                    Address = "29412 Declaration Circle",
                    ReceiveNewsLetters = false
                },
                new Person()
                {
                    PersonID = Guid.Parse("FF2459AD-2087-4F3E-B4DA-03CCACC4327D"),
                    PersonName = "Ignace",
                    Email = "ienevold3@ftc.gov",
                    DateOfBirth = DateTime.Parse("1999-12-31"),
                    Gender = "Male",
                    CountryID = Guid.Parse("5D1F3C1E-9222-4262-9480-A4170FEB589B"),
                    Address = "01155 Northridge Park",
                    ReceiveNewsLetters = true
                },
                new Person()
                {
                    PersonID = Guid.Parse("528761BB-B1BC-4E34-91CF-A5A61B7BBECF"),
                    PersonName = "Christina",
                    Email = "cdurham4@hud.gov",
                    DateOfBirth = DateTime.Parse("1992-02-02"),
                    Gender = "Female",
                    CountryID = Guid.Parse("151F8B77-A4CC-4948-A360-DD4DE7912E3D"),
                    Address = "50 Donald Street",
                    ReceiveNewsLetters = false
                },
                new Person()
                {
                    PersonID = Guid.Parse("70BB0CE5-C600-4507-A75E-2FC528D56831"),
                    PersonName = "Fons",
                    Email = "fjsanse9@cdbaby.com",
                    DateOfBirth = DateTime.Parse("1998-05-27"),
                    Gender = "Male",
                    CountryID = Guid.Parse("9D3A7C28-17AA-43DA-AAD7-D1EDBE625D47"),
                    Address = "8931 Esker Hill",
                    ReceiveNewsLetters = false
                },
                new Person()
                {
                    PersonID = Guid.Parse("B742FC5A-8039-4E3F-BB90-59581239EC40"),
                    PersonName = "Davin",
                    Email = "dlandeg7@git.com",
                    DateOfBirth = DateTime.Parse("1992-03-15"),
                    Gender = "Male",
                    CountryID = Guid.Parse("5D1F3C1E-9222-4262-9480-A4170FEB589B"),
                    Address = "9 Cody Parkway",
                    ReceiveNewsLetters = true
                },
                new Person()
                {
                    PersonID = Guid.Parse("A7FBB0F0-C913-44EF-8914-F87D81ED66C7"),
                    PersonName = "Jeralee",
                    Email = "jklesse8@hp.com",
                    DateOfBirth = DateTime.Parse("1995-03-05"),
                    Gender = "Other",
                    CountryID = Guid.Parse("151F8B77-A4CC-4948-A360-DD4DE7912E3D"),
                    Address = "58498 Crest Line Drive",
                    ReceiveNewsLetters = false
                },
                new Person()
                {
                    PersonID = Guid.Parse("BB115169-D6AA-4883-8EA4-E69905756195"),
                    PersonName = "Shelden",
                    Email = "sneary9@geocities.jp",
                    DateOfBirth = DateTime.Parse("1992-08-11"),
                    Gender = "Male",
                    CountryID = Guid.Parse("9D3A7C28-17AA-43DA-AAD7-D1EDBE625D47"),
                    Address = "0 Heffernan Center",
                    ReceiveNewsLetters = true
                },
                new Person()
                {
                    PersonID = Guid.Parse("B417D4F7-5C94-427E-9E06-E9955C5A3EB1"),
                    PersonName = "Sigfried",
                    Email = "stryhorn6@ucsd.edu",
                    DateOfBirth = DateTime.Parse("1993-06-10"),
                    Gender = "Male",
                    CountryID = Guid.Parse("1B984A3C-0F58-4A52-A1C4-5AF3D061E285"),
                    Address = "5 Sauthoff Lane",
                    ReceiveNewsLetters = true
                });
            }
        }

        #endregion Constructors

        #region Convert Methods

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        #endregion Convert Methods

        #region Add Methods

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            // Validation: personAddRequest parameter can't be null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();

            person.PersonID = Guid.NewGuid();

            _persons.Add(person);

            return ConvertPersonToPersonResponse(person);
        }

        #endregion Add Methods

        #region Get Methods

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp => ConvertPersonToPersonResponse(temp))
                .ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null)
                return null;

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(
            string searchBy,
            string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("MM dd yyyy").Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Country) ?
                    temp.Country.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(
                        searchString,
                        StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default:
                    matchingPersons = allPersons;
                    break;
            }

            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(
            List<PersonResponse> allPersons,
            string sortBy,
            SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender?.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender?.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        #endregion Get Methods

        #region Update Methods

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

            if (matchingPerson == null)
                throw new ArgumentException("Person not found");

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            return ConvertPersonToPersonResponse(matchingPerson);
        }

        #endregion Update Methods

        #region Delete Methods

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null)
                return false;

            _persons.RemoveAll(temp => temp.PersonID == personID);

            return true;
        }

        #endregion Delete Methods
    }
}