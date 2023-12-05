namespace EmployeeManagement.ViewModels
{
    public class StatisticsViewModel
    {
        public string LocalIpAddress { get; set; } = string.Empty;
        public int LocalPort { get; set; }
        public string RemoteIpAddress { get; set; } = string.Empty;
        public int  RemotePort { get; set; }
    }
}
