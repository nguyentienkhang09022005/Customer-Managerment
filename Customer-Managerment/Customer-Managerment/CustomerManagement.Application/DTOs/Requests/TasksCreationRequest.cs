namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class TasksCreationRequest
    {
        public string? Title { get; set; }

        public DateOnly? DueDate { get; set; }

        public Guid IdUser { get; set; }
    }
}
