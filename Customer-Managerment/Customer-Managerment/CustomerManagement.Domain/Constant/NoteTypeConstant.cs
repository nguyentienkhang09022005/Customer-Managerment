namespace Customer_Managerment.CustomerManagement.Domain.Constant
{
    public class NoteTypeConstant
    {
        public const string NoteTypeComment = "COMMENT";
        public const string NoteTypeUpdate = "UPDATE";
        public const string NoteTypeSystem = "SYSTEM";

        public static readonly string[] NoteTypes = {
            NoteTypeComment,
            NoteTypeUpdate,
            NoteTypeSystem
        };

        public static bool IsValid(string type)
        {
            return NoteTypes.Contains(type.ToUpper());
        }
    }
}