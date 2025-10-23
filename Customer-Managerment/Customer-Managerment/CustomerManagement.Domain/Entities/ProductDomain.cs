using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
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

        public ProductDomain(){}
    }
}
