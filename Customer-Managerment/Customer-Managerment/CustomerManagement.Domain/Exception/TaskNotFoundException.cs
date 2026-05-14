namespace Customer_Managerment.CustomerManagement.Domain.Exceptions
{
    public class TaskNotFoundException : NotFoundException
    {
        public TaskNotFoundException() : base("Task không tìm thấy!")
        {
        }

        public TaskNotFoundException(string message) : base(message)
        {
        }
    }
}