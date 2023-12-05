using AutoMapper;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.MapperProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, ViewModels.CourseViewModel>();
        }
    }
}
