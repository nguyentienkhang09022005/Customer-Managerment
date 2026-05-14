namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class EventParticipantRequest
    {
        public Guid IdEvent { get; set; }
        public Guid IdStaff { get; set; }
    }
}