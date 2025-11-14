using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public StaffRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        // Add Staff
        public async Task<StaffDomain> AddStaffAsync(StaffDomain staffDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = _mapper.Map<Staff>(staffDomain);
            if (!string.IsNullOrEmpty(staff.Password))
            {
                staff.Password = BCrypt.Net.BCrypt.HashPassword(staff.Password);
            }
            staff.IdStaff = Guid.NewGuid();
            staff.CreatedAt = DateTime.Now;

            await context.Staff.AddAsync(staff);
            await context.SaveChangesAsync();
            return _mapper.Map<StaffDomain>(staff);
        }

        // Get Staff By Email
        public async Task<StaffDomain?> GetStaffByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Staff
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;
            return _mapper.Map<StaffDomain>(user);
        }

        // Get Staff By Username
        public async Task<StaffDomain?> GetStaffByUsernameAsync(string userName)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Staff
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == userName);
            if (user == null) return null;
            return _mapper.Map<StaffDomain>(user);
        }

        // Get Staff By Id
        public async Task<StaffDomain?> GetStaffByIdAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Staff
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(s => s.IdStaff == idStaff);

            if (staff == null)
                throw new NotFoundException("Không tìm thấy thông tin nhân viên!");
            return _mapper.Map<StaffDomain>(staff);
        }

        public IQueryable<StaffDomain> GetListStaff()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Staff
                .ProjectTo<StaffDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public IQueryable<StaffDomain> GetStaffById(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Staff
                .Where(s => s.IdStaff == idStaff)
                .ProjectTo<StaffDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<StaffDomain?> UpdateStaffAsync(StaffDomain staffDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Staff.FindAsync(staffDomain.IdStaff);
            if (staff == null) return null;

            // Cập nhật các thuộc tính của staff
            _mapper.Map(staffDomain, staff);
            await context.SaveChangesAsync();   
            return _mapper.Map<StaffDomain>(staff);
        }

        public async Task<bool> CheckStaffExistsAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Staff
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(s => s.IdStaff == idStaff);
        }

        public async Task DeleteStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Staff.FindAsync(idStaff);
            if (staff == null)
                throw new NotFoundException("Không tìm thấy nhân viên!");

            context.Staff.Remove(staff);
            await context.SaveChangesAsync();
        }
    }
}
