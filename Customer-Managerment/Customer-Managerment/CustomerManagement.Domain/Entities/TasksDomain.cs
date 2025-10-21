namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class TasksDomain
    {
        public Guid IdTask { get; set; }

        public string? Title { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Status { get; set; }

        public Guid? IdUser { get; set; }

        public TasksDomain() { }
    }
}
