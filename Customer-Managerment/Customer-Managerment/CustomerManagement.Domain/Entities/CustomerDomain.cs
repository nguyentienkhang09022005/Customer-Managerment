using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class CustomerDomain
    {
        public Guid IdCustomer { get; set; }

        public DateTime? CreatedAt { get; set; }

        public PersonDomain personDomain { get; set; }

        public virtual Person IdCustomerNavigation { get; set; } = null!;
    }
}
