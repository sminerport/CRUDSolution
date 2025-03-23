using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a person object to the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person object to be added</param>
        /// <returns>Returns the person object after adding it (including newly generated person id)</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns all existing persons from the data store
        /// </summary>
        /// <returns>A list of <see cref="PersonResponse"></see> objects</returns>
        List<PersonResponse> GetAllPersons();
    }
}