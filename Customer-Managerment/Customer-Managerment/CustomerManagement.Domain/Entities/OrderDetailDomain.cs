using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class OrderDetailDomain
    {
        public Guid IdOrderDetail { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public Guid? IdOrder { get; set; }

        public Guid? IdProduct { get; set; }

        public virtual Order? IdOrderNavigation { get; set; }

        public virtual Product? IdProductNavigation { get; set; }

        public void Validate()
        {
            if (Quantity == null || Quantity <= 0)
                throw new DomainException("Số lượng sản phẩm phải lớn hơn 0!", 400);
        }
    }
}
