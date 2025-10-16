using System.ComponentModel.DataAnnotations;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class AuthenticationRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
