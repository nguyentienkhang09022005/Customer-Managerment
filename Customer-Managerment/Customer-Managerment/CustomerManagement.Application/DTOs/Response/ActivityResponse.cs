namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class ActivityResponse
    {
        public Guid IdActivity { get; set; }

        public string? CustomerName { get; set; }

        public string? EmployeeName { get; set; }

        public string? Type { get; set; }

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateOnly? Date { get; set; }
    }
}
