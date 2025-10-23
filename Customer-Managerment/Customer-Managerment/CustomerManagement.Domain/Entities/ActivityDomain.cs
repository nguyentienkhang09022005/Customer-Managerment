using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class ActivityDomain
    {
        public Guid IdActivity { get; set; }

        public string? Type { get; set; }

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateOnly? Date { get; set; }

        public Guid? IdUser { get; set; }

        public Guid? IdCustomer { get; set; }

        public virtual User? IdUserNavigation { get; set; }

        public virtual User? IdCustomerNavigation { get; set; }

        public ActivityDomain(string type)
        {
            ValidateType(type);
            Type = type;
        }

        private void ValidateType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new DomainException("Loại hoạt động không được để trống!", 400);

            var allowedTypes = new[] { "Call", "Meeting", "Email", "Other" };

            if (!allowedTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
                throw new DomainException("Loại hoạt động phải là 'Call', 'Meeting', 'Email' hoặc 'Other'!", 400);
        }
    }
}
