namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class TaskResponse
    {
        public Guid IdTask { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid IdStaffAssigned { get; set; }
        public StaffResponse? StaffAssigned { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid? LinkedEntityId { get; set; }
    }
}