using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Api.MiddleWare
{
    public class GraphQLExceptionFilter : IErrorFilter
    {
        private readonly ILogger<GraphQLExceptionFilter> _logger;

        public GraphQLExceptionFilter(ILogger<GraphQLExceptionFilter> logger)
        {
            _logger = logger;
        }

        public IError OnError(IError error)
        {
            if (error.Exception is DomainException domainEx)
            {
                return ErrorBuilder.New()
                    .SetMessage(domainEx.Message)
                    .SetExtension("status", domainEx.StatusCode)
                    .Build();
            }

            if (error.Code == "AUTH_NOT_AUTHENTICATED")
            {
                return ErrorBuilder.New()
                    .SetMessage("Vui lòng đăng nhập để thực hiện hành động này.")
                    .SetCode(error.Code)
                    .SetExtension("status", 401) 
                    .Build();
            }

            if (error.Code == "AUTH_NOT_AUTHORIZED" || error.Exception is UnauthorizedAccessException)
            {
                return ErrorBuilder.New()
                    .SetMessage("Bạn không có quyền truy cập tài nguyên này.")
                    .SetCode(error.Code)
                    .SetExtension("status", 403)
                    .Build();
            }

            _logger.LogWarning("Caught an unhandled error. Code: '{ErrorCode}', Message: '{ErrorMessage}', ExceptionType: '{ExceptionType}'",
                    error.Code,
                    error.Message,
                    error.Exception?.GetType().FullName);
            _logger.LogError(error.Exception, "Lỗi hệ thống không xác định: {Message}", error.Message);

            return ErrorBuilder.New()
                .SetMessage("Lỗi hệ thống không xác định.")
                .SetExtension("status", 500)
                .Build();
        }
    }
}