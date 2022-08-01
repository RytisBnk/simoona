﻿using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Shrooms.Contracts.DataTransferObjects;
using Shrooms.Contracts.DataTransferObjects.VacationPages;
using Shrooms.Domain.Services.VacationPages;
using Shrooms.Presentation.Api.Controllers;
using Shrooms.Presentation.WebViewModels.Models.VacationPage;
using Shrooms.Tests.Extensions;
using Shrooms.Tests.ModelMappings;

namespace Shrooms.Tests.Controllers.WebApi
{
    [TestFixture]
    public class VacationPageControllerTests
    {
        private VacationPageController _vacationPageController;

        private IVacationPageService _vacationPageService;

        [SetUp]
        public void TestInitializer()
        {
            _vacationPageService = Substitute.For<IVacationPageService>();

            _vacationPageController = new VacationPageController(_vacationPageService, ModelMapper.Create());
            _vacationPageController.SetUpControllerForTesting();
        }

        [Test]
        public async Task GetVacationPage_Should_Return_Ok()
        {
            // Arrange
            _vacationPageService.GetVacationPage(Arg.Any<int>()).Returns(new VacationPageDto());

            // Act
            var httpActionResult = await _vacationPageController.GetVacationPage();
            var response = await httpActionResult.ExecuteAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetVacationPage_Should_Return_NotFound_If_VacationPage_Does_Not_Exist()
        {
            // Arrange
            _vacationPageService.GetVacationPage(Arg.Any<int>()).ReturnsNull();

            // Act
            var httpActionResult = await _vacationPageController.GetVacationPage();
            var response = await httpActionResult.ExecuteAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task EditVacationPage_Should_Return_BadRequest_If_Invalid_ModelState()
        {
            // Arrange
            var vacationPageViewModel = new VacationPageViewModel();

            // Act
            _vacationPageController.Validate(vacationPageViewModel);

            var httpActionResult = await _vacationPageController.EditVacationPage(vacationPageViewModel);
            var response = await httpActionResult.ExecuteAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task EditVacationPage_Should_Return_Ok()
        {
            // Arrange
            _vacationPageService.EditVacationPage(Arg.Any<UserAndOrganizationDto>(), Arg.Any<VacationPageDto>())
                .Returns(Task.CompletedTask);

            var vacationPageViewModel = new VacationPageViewModel
            {
                Content = "test"
            };

            // Act
            _vacationPageController.Validate(vacationPageViewModel);

            var httpActionResult = await _vacationPageController.EditVacationPage(vacationPageViewModel);
            var response = await httpActionResult.ExecuteAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}