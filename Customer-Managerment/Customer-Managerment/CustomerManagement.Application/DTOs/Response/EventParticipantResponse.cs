namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class EventParticipantResponse
    {
        public Guid Id { get; set; }
        public Guid IdEvent { get; set; }
        public Guid IdStaff { get; set; }
        public StaffResponse? Staff { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? RespondedAt { get; set; }
    }
}