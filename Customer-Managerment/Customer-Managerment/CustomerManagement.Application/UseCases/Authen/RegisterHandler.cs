using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using FluentEmail.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Authen
{
    public class RegisterHandler
    {
        private readonly IFluentEmail _email;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ForgotPasswordHandler> _logger;


        private static string DefaultRole = "User";

        public RegisterHandler(IFluentEmail email,
                          IMemoryCache memoryCache,
                          IUserRepository userRepository,
                          ILogger<ForgotPasswordHandler> logger)
        {
            _email = email;
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _logger = logger;
            _logger = logger;
        }


        public async Task<string> SendOtpToRegisterAsync(RegisterRequest registerRequest)
        {
            // Nếu tồn tại email thì trả về lỗi
            var checkEmail = await _userRepository.GetUserByEmailAsync(registerRequest.Email);
            if (checkEmail != null)
            {
                throw new DomainException("Email đã tồn tại!", 409);
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                throw new DomainException("Hai mật khẩu không khớp!", 409);
            }

            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                var cacheKey = $"OTP_Register_{registerRequest.Email}";

                var cacheData = new RegisterCacheData
                {
                    Otp = otp,
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    UserName = registerRequest.UserName,
                    Password = registerRequest.Password
                };

                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // OTP hết hạn sau 2 phút
                });

                var randomTag = Guid.NewGuid().ToString("N").Substring(0, 5);
                // Gửi OTP đến mail
                var response = await _email
                    .To(registerRequest.Email)
                    .Tag(randomTag)
                    .Subject("Mã OTP xác thực đăng ký tài khoản")
                    .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 2 phút).</p>", true)
                    .SendAsync();

                if (!response.Successful)
                {
                    var error = string.Join(", ", response.ErrorMessages);
                    throw new DomainException($"Gửi OTP thất bại! {error}", 500);
                }

                return "OTP đã được gửi thành công! Vui lòng kiểm tra email của bạn.";
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, 500);
            }
        }

        public async Task<string> ConfirmOTPForRegisterHandleAsync(ConfirmOTPRequest confirmOTPRequest)
        {
            var cacheKey = $"OTP_Register_{confirmOTPRequest.Email}";
            if (!_memoryCache.TryGetValue<RegisterCacheData>(cacheKey, out var cacheData))
            {
                throw new DomainException("OTP không hợp lệ hoặc đã hết hạn!", 400);
            }

            // Nếu OTP đúng thì tạo user mới
            if (cacheData.Otp != confirmOTPRequest.OTP)
            {
                throw new DomainException("OTP không đúng!", 400);
            }

            try
            {
                // Tạo user mới
                var newUser = new UserDomain(cacheData.Email, cacheData.Password);
                newUser.Fullname = cacheData.FullName;
                newUser.Username = cacheData.UserName;
                newUser.Role = DefaultRole;

                await _userRepository.AddUserAsync(newUser);

                _memoryCache.Remove(cacheKey);

                return "Đăng ký thành công!";
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, 500);
            }
        }
    }
}
