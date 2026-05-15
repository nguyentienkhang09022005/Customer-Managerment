using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public TeamMemberRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<TeamMember?> GetByIdAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TeamMember>> GetByEntityAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers
                .Where(t => t.EntityType == entityType && t.EntityId == entityId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TeamMember>> GetByStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers
                .Where(t => t.IdStaff == idStaff)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TeamMember?> GetTeamMemberAsync(string entityType, Guid entityId, Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers
                .FirstOrDefaultAsync(t => t.EntityType == entityType && t.EntityId == entityId && t.IdStaff == idStaff);
        }

        public async Task<bool> HasOwnerAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers
                .AnyAsync(t => t.EntityType == entityType && t.EntityId == entityId && t.Role == 0);
        }

        public async Task<TeamMember> AddAsync(TeamMember teamMember)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.TeamMembers.AddAsync(teamMember);
            await context.SaveChangesAsync();
            return teamMember;
        }

        public async Task<TeamMember> UpdateAsync(TeamMember teamMember)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.TeamMembers.Update(teamMember);
            await context.SaveChangesAsync();
            return teamMember;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            var teamMember = await context.TeamMembers.FindAsync(id);
            if (teamMember == null)
                return false;

            context.TeamMembers.Remove(teamMember);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountByEntityAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.TeamMembers
                .CountAsync(t => t.EntityType == entityType && t.EntityId == entityId);
        }
    }
}