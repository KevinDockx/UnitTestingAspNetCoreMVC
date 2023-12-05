namespace EmployeeManagement.ViewModels
{
    public class EmployeeOverviewViewModel
    {
        public List<InternalEmployeeForOverviewViewModel> InternalEmployees { get; set; }

        public EmployeeOverviewViewModel(
            IEnumerable<InternalEmployeeForOverviewViewModel> internalEmployeeViewModels)
        {
            InternalEmployees = internalEmployeeViewModels.ToList();
        }
    }
}
