using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class NoteMentionRepository : INoteMentionRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public NoteMentionRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<NoteMention> AddMentionAsync(NoteMention mention)
        {
            await using var context = _contextFactory.CreateDbContext();

            mention.IdMention = Guid.NewGuid();
            mention.CreatedAt = DateTime.UtcNow;

            await context.NoteMentions.AddAsync(mention);
            await context.SaveChangesAsync();
            return mention;
        }

        public async Task<IQueryable<NoteMention>> GetMentionsByNoteAsync(Guid idNote)
        {
            var context = _contextFactory.CreateDbContext();
            return context.NoteMentions
                .Include(m => m.IdStaffMentionedNavigation)
                .Where(m => m.IdNote == idNote)
                .AsNoTracking();
        }

        public async Task<IQueryable<NoteMention>> GetMentionsByStaffAsync(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.NoteMentions
                .Include(m => m.IdNoteNavigation)
                .Where(m => m.IdStaffMentioned == idStaff)
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking();
        }

        public async Task DeleteMentionsByNoteAsync(Guid idNote)
        {
            await using var context = _contextFactory.CreateDbContext();
            var mentions = await context.NoteMentions.Where(m => m.IdNote == idNote).ToListAsync();
            context.NoteMentions.RemoveRange(mentions);
            await context.SaveChangesAsync();
        }
    }
}