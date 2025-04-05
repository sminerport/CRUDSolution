using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using Entities;

using Microsoft.Extensions.Logging;

using OfficeOpenXml;

using RepositoryContracts;

using Serilog;

using SerilogTimings;

using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        #region Fields

        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        #endregion Fields

        #region Constructors

        public PersonsService(
            IPersonsRepository personsRepository,
            ILogger<PersonsService> logger,
            IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        #endregion Constructors

        #region Add Methods

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            // Validation: personAddRequest parameter can't be null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();

            person.PersonID = Guid.NewGuid();

            await _personsRepository.AddPerson(person);

            return person.ToPersonResponse();
        }

        #endregion Add Methods

        #region Get Methods

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            var persons = await _personsRepository.GetAllPersons();

            return persons
               .Select(temp => temp.ToPersonResponse())
               .ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(
            string searchBy,
            string? searchString)
        {
            _logger.LogInformation("{MethodName} of {ClassName}", nameof(GetFilteredPersons), nameof(PersonsService));

            List<Person> persons;

            using (Operation.Time("Time for Filtered Persons from Database"))
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.DateOfBirth.Value.ToString("MM/dd/YYYY").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                        await _personsRepository.GetFilteredPersons(temp =>
                            temp.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            }

            _diagnosticContext.Set("Persons", persons);

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(
            List<PersonResponse> allPersons,
            string sortBy,
            SortOrderOptions sortOrder)
        {
            _logger.LogInformation("{MethodName} of {ClassName}", nameof(PersonsService), nameof(GetSortedPersons));
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (matchingPerson == null)
                throw new ArgumentException("Person not found");

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();
        }

        #endregion Update Methods

        #region Delete Methods

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
                return false;

            await _personsRepository.DeletePersonByPersonID(personID.Value);

            return true;
        }

        #endregion Delete Methods

        #region Export Methods

        public async Task<MemoryStream> GetPersonsCSV()
        {
            var memoryStream = new MemoryStream();
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            // Wrap both the StreamWriter and CsvWriter in using blocks
            using (var streamWriter = new StreamWriter(
                stream: memoryStream,
                encoding: System.Text.Encoding.UTF8,
                bufferSize: 1024,
                leaveOpen: true))
            using (var csvWriter = new CsvWriter(
                writer: streamWriter,
                csvConfiguration))
            {
                // Write header
                csvWriter.WriteField(nameof(PersonResponse.PersonName));
                csvWriter.WriteField(nameof(PersonResponse.Email));
                csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                csvWriter.WriteField(nameof(PersonResponse.Age));
                csvWriter.WriteField(nameof(PersonResponse.Gender));
                csvWriter.WriteField(nameof(PersonResponse.Country));
                csvWriter.WriteField(nameof(PersonResponse.Address));
                csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

                csvWriter.NextRecord();

                // Fetch data
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    csvWriter.WriteField(person.PersonName);
                    csvWriter.WriteField(person.Email);
                    if (person.DateOfBirth != null)
                        csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                    else
                        csvWriter.WriteField("");
                    csvWriter.WriteField(person.Age);
                    csvWriter.WriteField(person.Gender);
                    csvWriter.WriteField(person.Country);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.ReceiveNewsLetters);
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                }
            } // Disposal of csvWriter and streamWriter automatically flushes them

            // Reset position
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
                workSheet.Cells["B1"].Value = nameof(PersonResponse.Email);
                workSheet.Cells["C1"].Value = nameof(PersonResponse.DateOfBirth);
                workSheet.Cells["D1"].Value = nameof(PersonResponse.Age);
                workSheet.Cells["E1"].Value = nameof(PersonResponse.Gender);
                workSheet.Cells["F1"].Value = nameof(PersonResponse.Country);
                workSheet.Cells["G1"].Value = nameof(PersonResponse.Address);
                workSheet.Cells["H1"].Value = nameof(PersonResponse.ReceiveNewsLetters);

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;

                    if (person.DateOfBirth.HasValue)
                        workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();

                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        #endregion Export Methods
    }
}