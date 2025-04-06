using AutoFixture;

using CRUDExample.Controllers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsGettersService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;

            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;

            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsGettersService = _personsGetterServiceMock.Object;

            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsAdderService = _personsAdderServiceMock.Object;

            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsSorterService = _personsSorterServiceMock.Object;

            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsUpdaterService = _personsUpdaterServiceMock.Object;

            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsDeleterService = _personsDeleterServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(
                countriesService: _countriesService,
                logger: _logger,
                personsDeleterService: _personsDeleterService,
                personsGetterService: _personsGettersService,
                personsAdderService: _personsAdderService,
                personsUpdaterService: _personsUpdaterService,
                personsSorterService: _personsSorterService);

            _personsGetterServiceMock
                .Setup(temp => temp.GetFilteredPersons(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);

            _personsSorterServiceMock
                .Setup(temp => temp.GetSortedPersons(
                    It.IsAny<List<PersonResponse>>(),
                    It.IsAny<string>(),
                    It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons_response_list);

            // Act
            IActionResult result = await personsController.Index(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(persons_response_list);
        }

        #endregion Index

        #region Create

        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            // Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsAdderServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(
                countriesService: _countriesService,
                logger: _logger,
                personsDeleterService: _personsDeleterService,
                personsGetterService: _personsGettersService,
                personsAdderService: _personsAdderService,
                personsUpdaterService: _personsUpdaterService,
                personsSorterService: _personsSorterService);

            // Act
            IActionResult result = await personsController.Create(person_add_request);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion Create
    }
}