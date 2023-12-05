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
        private readonly IPromotionService _promotionService;

        public InternalEmployeeController(IEmployeeService employeeService,
            IMapper mapper,
            IPromotionService promotionService)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _promotionService = promotionService;
        }

        [HttpGet]
        public IActionResult AddInternalEmployee()
        {
            return View(new CreateInternalEmployeeViewModel()); 
        }

        [HttpPost]
        public async Task<IActionResult> AddInternalEmployee(
            CreateInternalEmployeeViewModel model)
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
                    await _employeeService.CreateInternalEmployeeAsync(
                        model.FirstName, model.LastName);

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
                if (Guid.TryParse(HttpContext?.Session?.GetString("EmployeeId"),
                      out Guid employeeIdFromSession))
                {
                    employeeId = employeeIdFromSession;
                }
                else if (Guid.TryParse(TempData["EmployeeId"]?.ToString(),
                     out Guid employeeIdFromTempData))
                {
                    employeeId = employeeIdFromTempData;
                }
                else
                {
                    return RedirectToAction("Index", "EmployeeOverview");
                }
            }

            var internalEmployee = await _employeeService
                .FetchInternalEmployeeAsync(employeeId.Value); 
            if (internalEmployee == null)
            {
                return RedirectToAction("Index", "EmployeeOverview"); 
            }
             
            return View(_mapper.Map<InternalEmployeeDetailViewModel>(
                internalEmployee));  
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

            if (await _promotionService.PromoteInternalEmployeeAsync(
                internalEmployee))
            {
                ViewBag.PromotionRequestMessage = "Employee was promoted.";

                // get the updated employee values
                internalEmployee = await _employeeService
                    .FetchInternalEmployeeAsync(employeeId.Value);
            }
            else
            {
                ViewBag.PromotionRequestMessage = 
                    "Sorry, this employee isn't eligible for promotion.";
            }

            return View("InternalEmployeeDetails", 
                _mapper.Map<InternalEmployeeDetailViewModel>(internalEmployee));
        }
    }
}
