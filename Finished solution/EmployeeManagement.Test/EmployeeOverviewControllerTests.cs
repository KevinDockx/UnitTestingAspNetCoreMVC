using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Test
{
    public class EmployeeOverviewControllerTests
    {
        private readonly EmployeeOverviewController _employeeOverviewController;
        private readonly InternalEmployee _firstEmployee;
        public EmployeeOverviewControllerTests()
        {
            _firstEmployee = new InternalEmployee("Megan", "Jones", 2, 3000, false, 2)
            {
                Id = Guid.Parse("bfdd0acd-d314-48d5-a7ad-0e94dfdd9155"),
                SuggestedBonus = 400
            };

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock
                .Setup(m => m.FetchInternalEmployeesAsync())
                .ReturnsAsync(new List<InternalEmployee>() {
                    _firstEmployee,
                    new InternalEmployee("Jaimy", "Johnson", 3, 3400, true, 1)  
                    {
                        Id = Guid.Parse("7183748a-ebeb-4355-8084-f190f8a5a68f"),
                        SuggestedBonus = 500
                    },
                    new InternalEmployee("Anne", "Adams", 3, 4000, false, 3)
                    {
                        Id = Guid.Parse("e2bba7cf-dca7-433d-9ba6-79c834a02c48"),
                        SuggestedBonus = 600
                    }
                });

            //var mapperMock = new Mock<IMapper>();
            //mapperMock.Setup(m =>
            //    m.Map<InternalEmployee, ViewModels.InternalEmployeeForOverviewViewModel>
            //    (It.IsAny<InternalEmployee>()))
            //    .Returns(new ViewModels.InternalEmployeeForOverviewViewModel());

            //_employeeOverviewController = new EmployeeOverviewController(
            //   employeeServiceMock.Object, mapperMock.Object);

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
        public async Task Index_GetAction_MustReturnEmployeeOverviewViewModelAsModelType()
        {
            // Arrange
           
            // Act
            var result = await _employeeOverviewController.Index();

            // Assert
            Assert.IsAssignableFrom<EmployeeOverviewViewModel>(
                ((ViewResult)result).Model); 
        }

        [Fact]
        public async Task Index_GetAction_MustReturnNumberOfInputtedInternalEmployees()
        {
            // Arrange

            // Act
            var result = await _employeeOverviewController.Index();

            // Assert 
            Assert.Equal(3, ((EmployeeOverviewViewModel)((ViewResult)result).Model)
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
            var viewModel = Assert.IsAssignableFrom<EmployeeOverviewViewModel>(viewResult.Model);
            Assert.Equal(3, viewModel.InternalEmployees.Count);
            var firstEmployee = viewModel.InternalEmployees[0];
            Assert.Equal(_firstEmployee.Id, firstEmployee.Id);
            Assert.Equal(_firstEmployee.FirstName, firstEmployee.FirstName);
            Assert.Equal(_firstEmployee.LastName, firstEmployee.LastName);
            Assert.Equal(_firstEmployee.Salary, firstEmployee.Salary);
            Assert.Equal(_firstEmployee.SuggestedBonus, firstEmployee.SuggestedBonus);
            Assert.Equal(_firstEmployee.YearsInService, firstEmployee.YearsInService); 
        }
    }
}
 