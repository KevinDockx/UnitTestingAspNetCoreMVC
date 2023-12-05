using AutoMapper;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.MapperProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<InternalEmployee, ViewModels.InternalEmployeeForOverviewViewModel>();
            CreateMap<InternalEmployee, ViewModels.InternalEmployeeDetailViewModel>(); 
        }
    }
}
