using AutoMapper;
using EmployeeManagement.ActionFilters;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IMapper _mapper;

        public StatisticsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var httpConnectionFeature = HttpContext.Features.Get<IHttpConnectionFeature>();
            return View(_mapper.Map<StatisticsViewModel>(httpConnectionFeature)); 
        }
    }
}
