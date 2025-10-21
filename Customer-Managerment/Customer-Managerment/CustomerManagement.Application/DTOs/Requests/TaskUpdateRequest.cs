namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class TaskUpdateRequest
    {
        public string? Title { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Status { get; set; }
    }
}
