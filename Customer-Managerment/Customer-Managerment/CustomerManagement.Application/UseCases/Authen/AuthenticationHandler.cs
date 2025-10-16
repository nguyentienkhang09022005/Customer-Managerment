using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Authen
{
    public class AuthenticationHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ForgotPasswordHandler> _logger;


        public AuthenticationHandler(IUserRepository userRepository,
                            ITokenService tokenService,
                            IMapper mapper,
                            IConfiguration config,
                            IHttpContextAccessor httpContextAccessor,
                            IRefreshTokenService refreshTokenService,
                            ILogger<ForgotPasswordHandler> logger)
        {
            _userRepository = userRepository;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _mapper = mapper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Login Handle
        public async Task<AuthenticationResponse> LoginHandleAsync(AuthenticationRequest authenticationRequest)
        {
            var userDomain = await _userRepository.GetUserByUsernameAsync(authenticationRequest.Username);
            if (userDomain == null)
            {
                throw new DomainException("Tên đăng nhập không đúng!", 409);
            }

            if (!BCrypt.Net.BCrypt.Verify(authenticationRequest.Password, userDomain.Password))
            {
                throw new DomainException("Mật khẩu không đúng!", 409);
            }

            var accessToken = _tokenService.generateAccessToken(userDomain);
            var refreshToken = _tokenService.generateRefreshToken(userDomain);

            // Set Refresh Token on Redis
            await _refreshTokenService.saveRefreshToken(userDomain.IdUser.ToString(),
                                                        refreshToken,
                                                        TimeSpan.FromDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "1"))
            );

            // Set cookie HttpOnly
            var response = _httpContextAccessor.HttpContext!.Response;
            response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                MaxAge = TimeSpan.FromDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "1"))
            });

            var userReponse = _mapper.Map<UserResponse>(userDomain);
            var authenticationResponse = new AuthenticationResponse
            {
                Token = accessToken,
                InfUser = userReponse,
            };

            return authenticationResponse;
        }

        // Logout Handle
        public async Task<string> LogoutHandleAsync()
        {
            string refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"] ?? string.Empty;
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new DomainException("Không tìm thấy RefreshToken!", 400);
            }
            var jwtToken = await VerifyToken(refreshToken);

            // Xóa refresh token trên redis
            var idUser = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(idUser))
            {
                throw new DomainException("RefreshToken không hợp lệ!", 400);
            }
            await _refreshTokenService.deleteRefreshToken(idUser);

            // Xóa cookie refresh token
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");

            return "Đăng xuất thành công!";
        }

        // Refresh Token Handle
        public async Task<AuthenticationResponse> RefreshTokenHandleAsync()
        {
            string refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"] ?? string.Empty;
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new DomainException("Không tìm thấy RefreshToken!", 400);
            }

            var jwtToken = await VerifyToken(refreshToken);
            var idUser = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(idUser))
            {
                throw new DomainException("RefreshToken không hợp lệ!", 400);
            }

            // Kiểm tra refresh token có tồn tại trên redis không
            var existRefreshToken = await _refreshTokenService.getRefreshToken(idUser);
            if (existRefreshToken == null || existRefreshToken != refreshToken)
            {
                throw new DomainException("RefreshToken không tồn tại!", 400);
            }

            var userDomain = await _userRepository.GetUserByIdAsync(Guid.Parse(idUser));
            if (userDomain == null)
            {
                throw new DomainException("Người dùng không tồn tại!", 404);
            }

            var newAccessToken = _tokenService.generateAccessToken(userDomain);

            var userReponse = _mapper.Map<UserResponse>(userDomain);
            var authenticationResponse = new AuthenticationResponse
            {
                Token = newAccessToken,
                InfUser = userReponse,
            };
            return authenticationResponse;
        }

        // Introspect Token Handle
        public async Task<IntrospectResponse> IntrospectTokenHandleAsync(IntrospectRequest introspectRequest)
        {
            Guid idUser = Guid.Empty;
            bool isValid = true;
            try
            {
                var jwtToken = await VerifyToken(introspectRequest.Token);
                string idUserStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (!string.IsNullOrEmpty(idUserStr))
                {
                    idUser = Guid.Parse(idUserStr);
                }
            }
            catch (ArgumentException ex)
            {
                isValid = false;
            }
            var introspectResponse = new IntrospectResponse
            {
                Valid = isValid,
                IdUser = idUser
            };
            return introspectResponse;
        }

        // Verify Token Handle
        public async Task<JwtSecurityToken> VerifyToken(string token)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
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

                var jwtToken = (JwtSecurityToken)validatedToken;

                
                return jwtToken;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new ArgumentException("Token đã hết hạn!");
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                throw new ArgumentException("chữ ký Token không hợp lệ!");
            }
            catch (Exception)
            {
                throw new ArgumentException("Token không hợp lệ!");
            }
        }
    }
}

