using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using FluentEmail.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Authen
{
    public class ForgotPasswordHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IFluentEmail _email;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(IStaffRepository staffRepository, 
                                     IMemoryCache memoryCache, 
                                     IFluentEmail email, 
                                     ILogger<ForgotPasswordHandler> logger)
        {
            _staffRepository = staffRepository;
            _memoryCache = memoryCache;
            _email = email;
            _logger = logger;
        }
        public async Task<string> SendOTPForForgotPasswordHandleAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            // Nếu không tồn tại email thì trả về lỗi
            var checkEmail = await _staffRepository.GetStaffByEmailAsync(forgotPasswordRequest.Email);
            if (checkEmail == null)
            {
                throw new DomainException("Email không tồn tại!", 404);
            }
            try
            {
                var otp = new Random().Next(100000, 999999).ToString();
                var cacheKey = $"OTP_ForgotPassword_{forgotPasswordRequest.Email}";
                var cacheData = new ForgotPasswordCacheData
                {
                    Otp = otp,
                    Email = forgotPasswordRequest.Email
                };
                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // OTP hết hạn sau 2 phút
                });

                var randomTag = Guid.NewGuid().ToString("N").Substring(0, 5);
                var response = await _email
                    .To(forgotPasswordRequest.Email)
                    .Tag(randomTag)
                    .Subject("Mã OTP hỗ trợ quên mật khẩu")
                    .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 2 phút).</p>", true)
                    .SendAsync();

                if (!response.Successful)
                {
                    throw new DomainException("Gửi OTP thất bại!", 500);
                }
                return "OTP đã được gửi đến bạn!";
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, 500);
            }
        }

        // Hàm xử lý xác thực OTP cho quên mật khẩu và đổi mật khẩu mới
        public async Task<string> ConfirmOTPForForgotPasswordHandleAsync(ChangePasswordRequest changePasswordRequest)
        {
            var cacheKey = $"OTP_ForgotPassword_{changePasswordRequest.Email}";
            if (!_memoryCache.TryGetValue<ForgotPasswordCacheData>(cacheKey, out var cacheData))
            {
                throw new DomainException("OTP không hợp lệ hoặc đã hết hạn!", 500);
            }
            // Nếu OTP đúng thì đổi mật khẩu
            if (cacheData.Otp != changePasswordRequest.OTP)
            {
                throw new DomainException("OTP không đúng!", 400);
            }
            if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmPassword)
            {
                throw new DomainException("Mật khẩu xác thực không khớp!", 400);
            }
            try
            {
                var staff = await _staffRepository.GetStaffByEmailAsync(changePasswordRequest.Email);
                if (staff == null)
                {
                    throw new DomainException("Email không tồn tại!", 404);
                }
                staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
                await _staffRepository.UpdateStaffAsync(staff);

                _memoryCache.Remove(cacheKey);
                return "Đổi mật khẩu thành công!";
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, 500);
            }
        }
    }
}
