namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class TaskCreationRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; } = 0;
        public int Status { get; set; } = 0;
        public Guid IdStaffAssigned { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }
    }

    public class TaskUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public Guid? IdStaffAssigned { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }
    }
}