using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class TasksDomain
    {
        public Guid IdTask { get; set; }

        public string? Title { get; set; }

        public DateOnly? DueDate { get; set; }

        public string? Status { get; set; }

        public Guid? IdUser { get; set; }

        public TasksDomain(string status) 
        { 
            CheckStatus(status);
            Status = status;
        }

        private void CheckStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Trạng thái công việc không được để trống!", 400);

            var allowedStatuses = new[] { "Pending", "Done" };

            if (!allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                throw new DomainException("Trạng thái công việc phải là 'Pending' hoặc 'Done'!", 400);
        }
    }
}
