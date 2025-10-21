namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class TasksResponse
    {
        public Guid IdTask { get; set; }

        public string? Title { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Status { get; set; }
    }
}
