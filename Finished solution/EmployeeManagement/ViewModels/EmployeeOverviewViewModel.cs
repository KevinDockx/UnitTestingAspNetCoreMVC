namespace EmployeeManagement.ViewModels;

public class EmployeeOverviewViewModel(
    IEnumerable<InternalEmployeeForOverviewViewModel> internalEmployeeViewModels)
{
    public List<InternalEmployeeForOverviewViewModel> InternalEmployees { get; set; } = internalEmployeeViewModels.ToList();
}
