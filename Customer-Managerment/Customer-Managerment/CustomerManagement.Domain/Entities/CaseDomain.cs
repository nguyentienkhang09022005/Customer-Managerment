using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class CaseDomain
    {
        public Guid IdCase { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public Guid? IdUser { get; set; }

        public Guid? IdCustomer { get; set; }

        public Guid? IdOrder { get; set; }

        public CaseDomain() { }

        public CaseDomain(string status)
        {
            CheckStatus(status);
            Status = status;
        }

        private void CheckStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Trạng thái không được để trống!", 400);

            var valid = new[] { "Open", "In Progress", "Resolved", "Closed" };
            if (!valid.Contains(status))
                throw new DomainException("Trạng thái case phải là 'Open', 'In Progress', 'Resolved' hoặc 'Closed'!", 400);
        }
    }
}
