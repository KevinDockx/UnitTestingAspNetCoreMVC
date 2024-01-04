using AutoMapper;
using EmployeeManagement.ActionFilters;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

public class StatisticsController(IMapper mapper) : Controller
{
    private readonly IMapper _mapper = mapper;

    [CheckShowStatisticsHeader]
    public IActionResult Index()
    {
        var httpConnectionFeature = HttpContext.Features
            .Get<IHttpConnectionFeature>();
        return View(_mapper.Map<StatisticsViewModel>(httpConnectionFeature)); 
    }
}
