using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class UserDomain
    {
        public Guid IdUser { get; set; }

        public string? Fullname { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserDomain(string email, string password) 
        {
            SetEmail(email);
            SetPassword(password);
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new DomainException("Email không hợp lệ!", 400);
            }
            Email = email;
        }

        // Phương thức kiểm tra tính hợp lệ của password
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new DomainException("Mật khẩu phải có ít nhất 6 ký tự!", 400);
            }
            Password = password;
        }
    }
}
