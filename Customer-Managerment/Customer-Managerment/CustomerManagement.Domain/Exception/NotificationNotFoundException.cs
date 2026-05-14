namespace Customer_Managerment.CustomerManagement.Domain.Exceptions
{
    public class NotificationNotFoundException : NotFoundException
    {
        public NotificationNotFoundException() : base("Notification không tìm thấy!")
        {
        }

        public NotificationNotFoundException(string message) : base(message)
        {
        }
    }
}