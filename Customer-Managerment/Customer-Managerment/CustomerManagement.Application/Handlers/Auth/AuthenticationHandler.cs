using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Customer_Managerment.CustomerManagement.Application.Handlers.Auth
{
    public class AuthenticationHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthenticationHandler> _logger;

        public AuthenticationHandler(
            IStaffRepository staffRepository,
            ITokenService tokenService,
            IMapper mapper,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IRefreshTokenService refreshTokenService,
            ILogger<AuthenticationHandler> logger)
        {
            _staffRepository = staffRepository;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _mapper = mapper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<AuthenticationResponse> LoginHandleAsync(AuthenticationRequest request)
        {
            var staff = await _staffRepository.GetStaffByUsernameAsync(request.Username);
            if (staff == null)
            {
                throw new DomainException("Tên đăng nhập không đúng!", 409);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash))
            {
                throw new DomainException("Mật khẩu không đúng!", 409);
            }

            var accessToken = _tokenService.GenerateAccessToken(staff);
            var refreshToken = _tokenService.GenerateRefreshToken(staff);

            await _refreshTokenService.SaveRefreshTokenAsync(
                staff.Id.ToString(),
                refreshToken,
                TimeSpan.FromDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "1"))
            );

            var response = _httpContextAccessor.HttpContext!.Response;
            response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                MaxAge = TimeSpan.FromDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "1"))
            });

            var staffResponse = _mapper.Map<StaffResponse>(staff);
            return new AuthenticationResponse
            {
                Token = accessToken,
                InfStaff = staffResponse,
            };
        }

        public async Task<string> LogoutHandleAsync()
        {
            string refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"] ?? string.Empty;
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new DomainException("Không tìm thấy RefreshToken!", 400);
            }
            var jwtToken = await VerifyToken(refreshToken);

            var idStaff = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(idStaff))
            {
                throw new DomainException("RefreshToken không hợp lệ!", 400);
            }
            await _refreshTokenService.DeleteRefreshTokenAsync(idStaff);

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return "Đăng xuất thành công!";
        }

        public async Task<AuthenticationResponse> RefreshTokenHandleAsync()
        {
            string refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"] ?? string.Empty;
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new DomainException("Không tìm thấy RefreshToken!", 400);
            }

            var jwtToken = await VerifyToken(refreshToken);
            var idStaff = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(idStaff))
            {
                throw new DomainException("RefreshToken không hợp lệ!", 400);
            }

            var existRefreshToken = await _refreshTokenService.GetRefreshTokenAsync(idStaff);
            if (existRefreshToken == null || existRefreshToken != refreshToken)
            {
                throw new DomainException("RefreshToken không tồn tại!", 400);
            }

            var staff = await _staffRepository.GetStaffByIdAsync(Guid.Parse(idStaff));
            if (staff == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }

            var newAccessToken = _tokenService.GenerateAccessToken(staff);
            var staffResponse = _mapper.Map<StaffResponse>(staff);

            return new AuthenticationResponse
            {
                Token = newAccessToken,
                InfStaff = staffResponse,
            };
        }

        public async Task<IntrospectResponse> IntrospectTokenHandleAsync(IntrospectRequest request)
        {
            Guid idStaff = Guid.Empty;
            bool isValid = true;
            try
            {
                var jwtToken = await VerifyToken(request.Token);
                string idStaffStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (!string.IsNullOrEmpty(idStaffStr))
                {
                    idStaff = Guid.Parse(idStaffStr);
                }
            }
            catch (ArgumentException)
            {
                isValid = false;
            }

            return new IntrospectResponse
            {
                Valid = isValid,
                IdUser = idStaff
            };
        }

        public async Task<JwtSecurityToken> VerifyToken(string token)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var tokenHandle = new JwtSecurityTokenHandler();

            try
            {
                tokenHandle.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidIssuer = jwtSettings["Issuer"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (JwtSecurityToken)validatedToken;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new ArgumentException("Token đã hết hạn!");
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                throw new ArgumentException("Chữ ký Token không hợp lệ!");
            }
            catch (Exception)
            {
                throw new ArgumentException("Token không hợp lệ!");
            }
        }
    }
}