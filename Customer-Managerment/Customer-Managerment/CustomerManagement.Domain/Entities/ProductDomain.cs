using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using FluentEmail.Core;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class ProductDomain
    {
        public Guid IdProduct { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public ProductDomain(string status) 
        {
            CheckStatus(status);
        }

        private void CheckStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Trạng thái sản phẩm không được để trống!", 400);

            var allowedStatuses = new[] { "Available", "Sold out" };

            if (!allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                throw new DomainException("Trạng thái sản phẩm phải là 'Available' hoặc 'Sold out'!", 400);
        }
    }
}
