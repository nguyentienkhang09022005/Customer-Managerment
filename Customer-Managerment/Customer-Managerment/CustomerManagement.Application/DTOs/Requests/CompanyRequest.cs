namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CompanyRequest
    {
        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Industry { get; set; }

        public DateOnly? EstablishmentDate { get; set; }
    }
}
