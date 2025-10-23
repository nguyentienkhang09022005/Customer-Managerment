namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CaseCreationRequest
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Guid? IdUser { get; set; }

        public Guid? IdCustomer { get; set; }

        public Guid? IdOrder { get; set; }
    }
}
