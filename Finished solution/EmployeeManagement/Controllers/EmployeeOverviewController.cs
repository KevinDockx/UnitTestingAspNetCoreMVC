using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Controllers
{
    public class EmployeeOverviewController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeOverviewController(IEmployeeService employeeService, 
            IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var internalEmployees = await _employeeService
                .FetchInternalEmployeesAsync();

            // with manual mapping
            //var internalEmployeeForOverviewViewModels =
            //    internalEmployees.Select(e => 
            //        new InternalEmployeeForOverviewViewModel()
            //        {
            //            Id = e.Id,
            //            FirstName = e.FirstName,
            //            LastName = e.LastName,
            //            Salary = e.Salary,
            //            SuggestedBonus = e.SuggestedBonus,
            //            YearsInService = e.YearsInService
            //        });

            // with AutoMapper
            var internalEmployeeForOverviewViewModels =
                _mapper.Map<IEnumerable<InternalEmployeeForOverviewViewModel>>(internalEmployees);

            return View(
                new EmployeeOverviewViewModel(internalEmployeeForOverviewViewModels));
        }

        [Authorize]
        public IActionResult ProtectedIndex()
        {
            // depending on the role, return a different result

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex", "EmployeeManagement");
            }

            return RedirectToAction("Index", "EmployeeManagement");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}