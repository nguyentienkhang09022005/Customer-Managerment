using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public UserRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        // Add User
        public async Task AddUserAsync(UserDomain users)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = _mapper.Map<User>(users);
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            user.IdUser = Guid.NewGuid();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        // Get User By Email
        public async Task<UserDomain?> GetUserByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;
            return _mapper.Map<UserDomain>(user);
        }

        // Get User By Id
        public async Task<UserDomain?> GetUserByIdAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (user == null)
                throw new NotFoundException("Không tìm thấy thông tin người dùng!");
            return _mapper.Map<UserDomain>(user);
        }
    }
}
