using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string generateAccessToken(StaffDomain staff)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, staff.IdStaff.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, staff.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, staff.Fullname ?? string.Empty),
                new Claim(ClaimTypes.Role, staff.Role ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var accessToken = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public string generateRefreshToken(StaffDomain staff)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, staff.IdStaff.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, staff.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, staff.Fullname ?? string.Empty),
                new Claim(ClaimTypes.Role, staff.Role ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var refreshToken = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(refreshToken);
        }
    }
}
