namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class EventParticipant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdEvent { get; set; }
        public Guid IdStaff { get; set; }
        public int Status { get; set; } = 0;
        public DateTime? RespondedAt { get; set; }

        public virtual CalendarEvent? IdEventNavigation { get; set; }
        public virtual Person? IdStaffNavigation { get; set; }
    }
}