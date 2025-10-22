using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class OrderDomain
    {
        public Guid IdOrder { get; set; }

        public DateOnly? OrderDate { get; set; }

        public string? Status { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public Guid? IdUser { get; set; }

        public Guid? IdCustomer { get; set; }
        public OrderDomain() { }

        public OrderDomain(string status)
        {
            CheckStatus(status);
        }

        private void CheckStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Trạng thái đơn hàng không được để trống!", 400);

            var allowedStatuses = new[] { "Pending", "Paid", "Cancelled" };
            if (!allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                throw new DomainException("Trạng thái đơn hàng phải là 'Pending', 'Paid' hoặc 'Cancelled'!", 400);
        }
    }
}
