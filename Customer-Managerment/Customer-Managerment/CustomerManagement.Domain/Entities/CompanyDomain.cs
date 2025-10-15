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

        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Email không hợp lệ!");
            }
            Email = email;
        }

        public void setPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length > 12 || phone.Length < 10)
            {
                throw new ArgumentException("Số điện thoại phải lớn hơn 10 và nhỏ hơn 12 số");
            }    
            Phone = phone;
        }
    }
}
