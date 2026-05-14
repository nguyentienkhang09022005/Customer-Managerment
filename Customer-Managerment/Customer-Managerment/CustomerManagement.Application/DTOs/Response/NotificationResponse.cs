namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class NotificationResponse
    {
        public Guid IdNotification { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsRead { get; set; }
        public bool IsPinned { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid IdStaff { get; set; }
        public StaffResponse? Staff { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class NotificationListResponse
    {
        public List<NotificationResponse> Notifications { get; set; } = new();
        public int TotalUnread { get; set; }
    }
}