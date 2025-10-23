namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CaseResponse
    {
        public Guid IdCase { get; set; }

        public string? Title { get; set; }

        public string? CustomerName { get; set; }

        public string? EmployeeName { get; set; }   

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ResolveAt { get; set; }
    }
}
