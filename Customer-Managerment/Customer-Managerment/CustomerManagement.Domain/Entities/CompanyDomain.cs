
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class CompanyDomain
    {
        public Guid IdCompany { get; set; }

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Industry { get; set; }

        public DateOnly? EstablishmentDate { get; set; }

        public CompanyDomain(string email, string phone)
        {
            SetEmail(email);
            setPhone(phone);
        }

        public void SetEmail(string email)
        {
            Console.WriteLine($"[DEBUG] Giá trị email nhận được: '{email}'");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new DomainException("Email không hợp lệ!", 400);
            }
            Email = email;
        }

        public void setPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length > 12 || phone.Length < 10)
            {
                throw new DomainException("Số điện thoại phải lớn hơn 10 và nhỏ hơn 12 số", 400);
            }    
            Phone = phone;
        }
    }
}
