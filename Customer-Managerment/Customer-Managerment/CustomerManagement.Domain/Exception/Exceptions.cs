namespace Customer_Managerment.CustomerManagement.Domain.Exceptions
{
    // Validation Exceptions (400)
    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message, 400) { }
    }

    public class RequiredFieldException : ValidationException
    {
        public RequiredFieldException(string fieldName) : base($"{fieldName} không được để trống!") { }
    }

    public class InvalidEmailException : ValidationException
    {
        public InvalidEmailException() : base("Định dạng email không hợp lệ!") { }
    }

    public class InvalidFormatException : ValidationException
    {
        public InvalidFormatException(string fieldName, string expectedFormat)
            : base($"{fieldName} không đúng định dạng. Vui lòng nhập {expectedFormat}!") { }
    }

    public class InvalidGuidException : ValidationException
    {
        public InvalidGuidException(string fieldName) : base($"{fieldName} không hợp lệ!") { }
    }

    public class InvalidLengthException : ValidationException
    {
        public InvalidLengthException(string fieldName, int min, int max)
            : base($"{fieldName} phải có độ dài từ {min} đến {max} ký tự!") { }
    }

    public class InvalidPasswordException : ValidationException
    {
        public InvalidPasswordException() : base("Mật khẩu phải có ít nhất 6 ký tự!") { }
    }

    // Not Found Exceptions (404)
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName) : base($"{entityName} không tồn tại!", 404) { }
    }

    public class StaffNotFoundException : NotFoundException
    {
        public StaffNotFoundException() : base("Nhân viên") { }
    }

    public class LeadNotFoundException : NotFoundException
    {
        public LeadNotFoundException() : base("Khách hàng tiềm năng") { }
    }

    public class CustomerNotFoundException : NotFoundException
    {
        public CustomerNotFoundException() : base("Khách hàng") { }
    }

    public class ContactNotFoundException : NotFoundException
    {
        public ContactNotFoundException() : base("Hoạt động") { }
    }

    public class DealNotFoundException : NotFoundException
    {
        public DealNotFoundException() : base("Deal") { }
    }

    // Conflict Exceptions (409)
    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message, 409) { }
    }

    public class EmailAlreadyExistsException : ConflictException
    {
        public EmailAlreadyExistsException() : base("Email đã tồn tại trong hệ thống!") { }
    }

    public class UsernameAlreadyExistsException : ConflictException
    {
        public UsernameAlreadyExistsException() : base("Username đã tồn tại trong hệ thống!") { }
    }

    public class DuplicateEntryException : ConflictException
    {
        public DuplicateEntryException(string fieldName)
            : base($"{fieldName} đã tồn tại trong hệ thống!") { }
    }

    // Unauthorized Exceptions (401)
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message = "Không có quyền truy cập!") : base(message, 401) { }
    }

    public class InvalidCredentialsException : UnauthorizedException
    {
        public InvalidCredentialsException() : base("Tên đăng nhập hoặc mật khẩu không đúng!") { }
    }

    // Business Rule Exceptions (422)
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message, 422) { }
    }

    public class InvalidStatusTransitionException : BusinessRuleException
    {
        public InvalidStatusTransitionException(string fromStatus, string toStatus)
            : base($"Không thể chuyển trạng thái từ {fromStatus} sang {toStatus}!") { }
    }

    public class CannotConvertLeadException : BusinessRuleException
    {
        public CannotConvertLeadException(string reason) : base($"Không thể chuyển đổi khách hàng tiềm năng: {reason}") { }
    }

    public class InvalidStatusException : ValidationException
    {
        public InvalidStatusException(string status) : base($"Trạng thái '{status}' không hợp lệ!") { }
    }

    public class TeamMemberNotFoundException : NotFoundException
    {
        public TeamMemberNotFoundException() : base("Thành viên nhóm") { }
    }

    public class AuditLogNotFoundException : NotFoundException
    {
        public AuditLogNotFoundException() : base("Nhật ký kiểm toán") { }
    }

    public class CalendarEventNotFoundException : NotFoundException
    {
        public CalendarEventNotFoundException() : base("Sự kiện lịch") { }
    }

    public class EventParticipantNotFoundException : NotFoundException
    {
        public EventParticipantNotFoundException() : base("Người tham gia sự kiện") { }
    }
}