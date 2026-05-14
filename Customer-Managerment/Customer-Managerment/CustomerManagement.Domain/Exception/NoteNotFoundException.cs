namespace Customer_Managerment.CustomerManagement.Domain.Exceptions
{
    public class NoteNotFoundException : NotFoundException
    {
        public NoteNotFoundException() : base("Note không tìm thấy!")
        {
        }

        public NoteNotFoundException(string message) : base(message)
        {
        }
    }
}