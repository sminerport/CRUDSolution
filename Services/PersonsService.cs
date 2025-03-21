using ServiceContracts;
using ServiceContracts.DTO;

using Entities;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;

        public PersonsService()
        {
            _persons = new List<Person>();
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            // Validation: personAddRequest parameter can't be null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            // Validation: PersonName can't be null
            if (personAddRequest.PersonName == null)
            {
                throw new ArgumentException(nameof(personAddRequest.PersonName));
            }

            Person person = personAddRequest.ToPerson();

            person.PersonID = Guid.NewGuid();

            _persons.Add(person);

            return person.ToPersonResponse();
        }
    }
}