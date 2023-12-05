using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels
{
    public class InternalEmployeeForOverviewViewModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }
      
        public int YearsInService { get; set; }

        public decimal SuggestedBonus { get; set; }
       
        public decimal Salary { get; set; } 
    }
}
