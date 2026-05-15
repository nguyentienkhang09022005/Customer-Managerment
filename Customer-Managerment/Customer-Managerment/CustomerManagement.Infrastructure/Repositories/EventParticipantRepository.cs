using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public EventParticipantRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<EventParticipant?> GetByIdAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.EventParticipants.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<EventParticipant>> GetByEventAsync(Guid idEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.EventParticipants
                .Where(p => p.IdEvent == idEvent)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EventParticipant?> GetParticipantAsync(Guid idEvent, Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.EventParticipants
                .FirstOrDefaultAsync(p => p.IdEvent == idEvent && p.IdStaff == idStaff);
        }

        public async Task<EventParticipant> AddAsync(EventParticipant participant)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.EventParticipants.AddAsync(participant);
            await context.SaveChangesAsync();
            return participant;
        }

        public async Task<EventParticipant> UpdateAsync(EventParticipant participant)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.EventParticipants.Update(participant);
            await context.SaveChangesAsync();
            return participant;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            var participant = await context.EventParticipants.FindAsync(id);
            if (participant == null)
                return false;

            context.EventParticipants.Remove(participant);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveByEventAsync(Guid idEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            var participants = await context.EventParticipants
                .Where(p => p.IdEvent == idEvent)
                .ToListAsync();

            if (!participants.Any())
                return false;

            context.EventParticipants.RemoveRange(participants);
            await context.SaveChangesAsync();
            return true;
        }
    }
}