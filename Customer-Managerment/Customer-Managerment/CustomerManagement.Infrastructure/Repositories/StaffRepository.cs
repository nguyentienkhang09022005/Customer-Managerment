    using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
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

        public async Task<Person> AddStaffAsync(Person staff)
        {
            await using var context = _contextFactory.CreateDbContext();

            staff.Id = Guid.NewGuid();
            staff.Discriminator = PersonType.Staff;
            staff.CreatedAt = DateTime.UtcNow;
            staff.IsDeleted = false;

            if (!string.IsNullOrEmpty(staff.PasswordHash))
            {
                staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(staff.PasswordHash);
            }

            await context.Persons.AddAsync(staff);
            await context.SaveChangesAsync();
            return staff;
        }

        public async Task<Person?> GetStaffByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Email == email && p.Discriminator == PersonType.Staff);

            return staff;
        }

        public async Task<Person?> GetStaffByUsernameAsync(string userName)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Username == userName && p.Discriminator == PersonType.Staff);

            return staff;
        }

        public async Task<Person?> GetStaffByIdAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);

            if (staff == null)
                throw new NotFoundException("Không tìm thấy thông tin nhân viên!");

            return staff;
        }

        public IQueryable<Person> GetListStaff()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Discriminator == PersonType.Staff)
                .AsNoTracking();
        }

        public IQueryable<Person> GetStaffById(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Id == idStaff && p.Discriminator == PersonType.Staff)
                .AsNoTracking();
        }

        public async Task<Person?> UpdateStaffAsync(Person staff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingStaff = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == staff.Id && p.Discriminator == PersonType.Staff);

            if (existingStaff == null)
                return null;

            existingStaff.Fullname = staff.Fullname;
            existingStaff.Email = staff.Email;
            existingStaff.Phone = staff.Phone;
            existingStaff.Location = staff.Location;
            existingStaff.Role = staff.Role;
            existingStaff.Salary = staff.Salary;
            existingStaff.UpdatedAt = DateTime.UtcNow;
            existingStaff.PasswordHash = staff.PasswordHash;

            await context.SaveChangesAsync();
            return existingStaff;
        }

        public async Task<bool> CheckStaffExistsAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .AnyAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);
        }

        public async Task<bool> SoftDeleteStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);

            if (staff == null)
                return false;

            staff.IsDeleted = true;
            staff.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);

            if (staff == null || !staff.IsDeleted)
                return false;

            staff.IsDeleted = false;
            staff.DeletedAt = null;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Person>> GetStaffByRoleAsync(string role)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Staff && p.Role == role && !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateStaffStatusAsync(Guid idStaff, int status)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);

            if (staff == null)
                return false;

            staff.Status = status;
            staff.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLastActiveAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var staff = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == idStaff && p.Discriminator == PersonType.Staff);

            if (staff == null)
                return false;

            staff.LastActiveAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Person>> GetOnlineStaffsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Staff && p.Status == 1 && !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Person>> GetStaffsByStatusAsync(int status)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Staff && p.Status == status && !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}