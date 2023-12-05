using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class InternalEmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper; 

        public InternalEmployeeController(IEmployeeService employeeService,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult AddInternalEmployee()
        {
            return View(new CreateInternalEmployeeViewModel()); 
        }

        [HttpPost]
        public async Task<IActionResult> AddInternalEmployee(CreateInternalEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                // create an internal employee entity with default values filled out
                // and the values the user inputted
                var internalEmplooyee =
                    await _employeeService.CreateInternalEmployeeAsync(model.FirstName, model.LastName);

                // persist it
                await _employeeService.AddInternalEmployeeAsync(internalEmplooyee);
            }

            return RedirectToAction("Index", "EmployeeOverview");
        }

        [HttpGet]
        public async Task<IActionResult> InternalEmployeeDetails(
            [FromRoute(Name = "id")] Guid? employeeId)
        {
            if (!employeeId.HasValue)
            {                 
                return RedirectToAction("Index", "EmployeeOverview"); 
            }

            var internalEmployee = await _employeeService.FetchInternalEmployeeAsync(employeeId.Value); 
            if (internalEmployee == null)
            {
                return RedirectToAction("Index", "EmployeeOverview"); 
            }
             
            return View(_mapper.Map<InternalEmployeeDetailViewModel>(internalEmployee));  
        }

        [HttpPost]
        public async Task<IActionResult> ExecutePromotionRequest(
            [FromForm(Name = "id")] Guid? employeeId)
        {
            if (!employeeId.HasValue)
            {
                return RedirectToAction("Index", "EmployeeOverview");
            }

            var internalEmployee = await _employeeService
                .FetchInternalEmployeeAsync(employeeId.Value);

            if (internalEmployee == null)
            {
                return RedirectToAction("Index", "EmployeeOverview");
            } 
 
            return View("InternalEmployeeDetails", 
                _mapper.Map<InternalEmployeeDetailViewModel>(internalEmployee));
        }
    }
}
