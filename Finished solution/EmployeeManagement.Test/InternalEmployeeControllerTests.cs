using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Services.Test;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Moq.Protected;
using System.Text;
using System.Text.Json;
using Xunit;

namespace EmployeeManagement.Test
{
    public class InternalEmployeeControllerTests
    {
        [Fact]
        public async Task AddInternalEmployee_InvalidInput_MustReturnBadRequest()
        {
            // Arrange
            var employeeServiceMock = new Mock<IEmployeeService>();
            var mapperMock = new Mock<IMapper>();
            var internalEmployeeController = new InternalEmployeeController(
                employeeServiceMock.Object, mapperMock.Object, null);

            var createInternalEmployeeViewModel =
                       new CreateInternalEmployeeViewModel();

            internalEmployeeController.ModelState
                .AddModelError("FirstName", "Required");

            // Act
            var result = await internalEmployeeController
                .AddInternalEmployee(createInternalEmployeeViewModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task InternalEmployeeDetails_InputFromTempData_MustReturnCorrectEmployee()
        {
            var expectedEmployeeId = 
                Guid.Parse("7183748a-ebeb-4355-8084-f190f8a5a68f");

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1)
                    {
                        Id = expectedEmployeeId,
                        SuggestedBonus = 500
                    });

            var mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
            var mapper = new Mapper(mapperConfiguration);

            var internalEmployeeController = new InternalEmployeeController(
              employeeServiceMock.Object, mapper, null);

            var tempDataDictionary = new TempDataDictionary(
                new DefaultHttpContext(),
                new Mock<ITempDataProvider>().Object);
            tempDataDictionary["EmployeeId"] = expectedEmployeeId;

            internalEmployeeController.TempData = tempDataDictionary;

            // Act
            var result = await internalEmployeeController
                .InternalEmployeeDetails(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<InternalEmployeeDetailViewModel>(
                viewResult.Model);
            Assert.Equal(expectedEmployeeId, viewModel.Id);
        }

        [Fact]
        public async Task InternalEmployeeDetails_InputFromSession_MustReturnCorrectEmployee()
        {
            // Arrange 
            var expectedEmployeeId =
                Guid.Parse("7183748a-ebeb-4355-8084-f190f8a5a68f");

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1)
                    {
                        Id = expectedEmployeeId,
                        SuggestedBonus = 500
                    });

            var mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
            var mapper = new Mapper(mapperConfiguration);

            var internalEmployeeController = new InternalEmployeeController(
              employeeServiceMock.Object, mapper, null);

            var defaultHttpContext = new DefaultHttpContext();

            var sessionMock = new Mock<ISession>();
            //sessionMock.Setup(s => s.GetString("EmployeeId"))
            //    .Returns(expectedEmployeeId.ToString());
            var guidAsBytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            sessionMock
                .Setup(s => s.TryGetValue(It.IsAny<string>(), out guidAsBytes))
                .Returns(true);

            // Supporting different values: 
            //sessionMock.Setup(s => s.TryGetValue("SomeKey", out someBytes))
            //  .Returns(true);
            //sessionMock.Setup(s => s.TryGetValue("SomeOtherKey", out someOtherBytes))
            //  .Returns(true);

            defaultHttpContext.Session = sessionMock.Object;

            internalEmployeeController.ControllerContext = new ControllerContext()
            {
                HttpContext = defaultHttpContext
            };

            // Act
            var result = await internalEmployeeController
                .InternalEmployeeDetails(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<InternalEmployeeDetailViewModel>(
                viewResult.Model);
            Assert.Equal(expectedEmployeeId, viewModel.Id);
        }

        [Fact]
        public async Task ExecutePromotionRequest_RequestPromotion_MustPromoteEmployee()
        {
            // Arrange 
            var expectedEmployeeId = Guid.NewGuid();
            var currentJobLevel = 1;

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeeAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new InternalEmployee(
                        "Jaimy", "Johnson", 3, 3400, true, currentJobLevel)
                    {
                        Id = expectedEmployeeId,
                        SuggestedBonus = 500
                    });

            var mapperConfiguration = new MapperConfiguration(
                cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
            var mapper = new Mapper(mapperConfiguration);

            var eligibleForPromotionHandlerMock = new Mock<HttpMessageHandler>();
            eligibleForPromotionHandlerMock.Protected()
                 .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(
                     JsonSerializer.Serialize(
                         new PromotionEligibility() { EligibleForPromotion = true },
                         new JsonSerializerOptions
                         {
                             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                         }),
                     Encoding.ASCII,
                     "application/json")
                });

            var httpClient = new HttpClient(eligibleForPromotionHandlerMock.Object);

            var promotionService = new PromotionService(httpClient,
                new EmployeeManagementTestDataRepository());

            var internalEmployeeController = new InternalEmployeeController(
                employeeServiceMock.Object, mapper, promotionService);

            // Act
            var result = await internalEmployeeController
                .ExecutePromotionRequest(expectedEmployeeId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<InternalEmployeeDetailViewModel>(
                viewResult.Model);
            Assert.Equal(expectedEmployeeId, viewModel.Id);
            Assert.Equal(++currentJobLevel, viewModel.JobLevel);
        }
    }
}
