namespace DynamicFormBuilder.ViewModels
{
    public class DashboardViewModel
    {
            public int StudentsCount { get; set; }
            public int EmployeesCount { get; set; }
            public int CustomersCount { get; set; }
            public int ParticipantsCount { get; set; }

        public List<string> RecentActivities { get; set; } = new();
        

    }
}
