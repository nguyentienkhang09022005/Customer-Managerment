namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
