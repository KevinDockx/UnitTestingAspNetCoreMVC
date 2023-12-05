using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EmployeeManagement.Test
{
    public class EmployeeOverviewTests
    {
        private readonly EmployeeOverviewController _employeeOverviewController;
        private readonly InternalEmployee _firstEmployee;

        public EmployeeOverviewTests()
        {
            _firstEmployee = new InternalEmployee(
                "Megan", "Jones", 2, 3000, false, 2)
            {
                Id = Guid.Parse("bfdd0acd-d314-48d5-a7ad-0e94dfdd9155"),
                SuggestedBonus = 400
            };

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeesAsync())
                .ReturnsAsync(new List<InternalEmployee>() {
                    _firstEmployee,
                    new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1),
                    new InternalEmployee("Anne", "Adams", 3, 4000, false, 3)
                });

            //var mapperMock = new Mock<IMapper>();
            //mapperMock.Setup(m =>
            //    m.Map<InternalEmployee, 
            //        ViewModels.InternalEmployeeForOverviewViewModel>
            //        (It.IsAny<InternalEmployee>()))
            //    .   Returns(new ViewModels.InternalEmployeeForOverviewViewModel());

            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<MapperProfiles.EmployeeProfile>());
            var mapper = new Mapper(mapperConfiguration);

            _employeeOverviewController = new EmployeeOverviewController(
               employeeServiceMock.Object, mapper);
        }

        [Fact]
        public async Task Index_GetAction_MustReturnViewResult()
        {
            // Arrange
          
            // Act
            var result = await _employeeOverviewController.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_GetAction_MustReturnEmployeeOverviewViewModelAsViewModelType()
        {
            // Arrange

            // Act
            var result = await _employeeOverviewController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<EmployeeOverviewViewModel>(viewResult.Model);

            //Assert.IsType<EmployeeOverviewViewModel>(
            //  ((ViewResult)result).Model);
        }

        [Fact]
        public async Task Index_GetAction_MustReturnNumberOfInputtedInternalEmployees()
        { 
            // Arrange

            // Act
            var result = await _employeeOverviewController.Index();

            // Assert
            Assert.Equal(3,
                ((EmployeeOverviewViewModel)((ViewResult)result).Model)
                .InternalEmployees.Count);
        }

        [Fact]
        public async Task Index_GetAction_ReturnsViewResultWithInternalEmployees()
        {    
            // Arrange

            // Act
            var result = await _employeeOverviewController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<EmployeeOverviewViewModel>(viewResult.Model);
            Assert.Equal(3,viewModel.InternalEmployees.Count);
            var firstEmployee = viewModel.InternalEmployees[0];
            Assert.Equal(_firstEmployee.Id, firstEmployee.Id);
            Assert.Equal(_firstEmployee.FirstName, firstEmployee.FirstName);
            Assert.Equal(_firstEmployee.LastName, firstEmployee.LastName);
            Assert.Equal(_firstEmployee.Salary, firstEmployee.Salary);
            Assert.Equal(_firstEmployee.SuggestedBonus, firstEmployee.SuggestedBonus);
            Assert.Equal(_firstEmployee.YearsInService, firstEmployee.YearsInService);
        }

        [Fact]
        public void ProtectedIndex_GetActionForUserInAdminRole_MustRedirectToAdminIndex()
        {
            // Arrange
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Karen"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "UnitTest");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            };

            _employeeOverviewController.ControllerContext = 
                new ControllerContext()
                {
                    HttpContext = httpContext
                };

            // Act
            var result = _employeeOverviewController.ProtectedIndex();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminIndex", redirectToActionResult.ActionName);
            Assert.Equal("EmployeeManagement", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void ProtectedIndex_GetActionForUserInAdminRole_MustRedirectToAdminIndex_WithMoq()
        {
            // Arrange
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(x => x.IsInRole(It.Is<string>(s => s == "Admin")))
                .Returns(true);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User)
                .Returns(mockPrincipal.Object);


            _employeeOverviewController.ControllerContext =
                new ControllerContext()
                {
                    HttpContext = httpContextMock.Object
                };

            // Act
            var result = _employeeOverviewController.ProtectedIndex();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminIndex", redirectToActionResult.ActionName);
            Assert.Equal("EmployeeManagement", redirectToActionResult.ControllerName);
        }
    }
}
