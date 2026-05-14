using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public NoteRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Note?> GetNoteByIdAsync(Guid idNote)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Notes
                .Include(n => n.IdStaffNavigation)
                .Include(n => n.Replies)
                .Include(n => n.Mentions)
                .FirstOrDefaultAsync(n => n.IdNote == idNote);
        }

        public async Task<IQueryable<Note>> GetNotesByEntityAsync(string entityType, Guid entityId)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Notes
                .Include(n => n.IdStaffNavigation)
                .Include(n => n.Mentions)
                .Where(n => n.LinkedEntityType == entityType && n.LinkedEntityId == entityId && !n.IsDeleted && n.ParentNoteId == null)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();
        }

        public async Task<IQueryable<Note>> GetPinnedNotesAsync(string entityType, Guid entityId)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Notes
                .Include(n => n.IdStaffNavigation)
                .Where(n => n.LinkedEntityType == entityType && n.LinkedEntityId == entityId && n.IsPinned && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            await using var context = _contextFactory.CreateDbContext();

            note.IdNote = Guid.NewGuid();
            note.CreatedAt = DateTime.UtcNow;
            note.IsDeleted = false;

            await context.Notes.AddAsync(note);
            await context.SaveChangesAsync();
            return note;
        }

        public async Task<Note> UpdateNoteAsync(Note note)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingNote = await context.Notes.FirstOrDefaultAsync(n => n.IdNote == note.IdNote);

            if (existingNote == null)
                throw new NotFoundException("Không tìm thấy Note!");

            existingNote.Content = note.Content;
            existingNote.IsPinned = note.IsPinned;
            existingNote.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingNote;
        }

        public async Task<bool> SoftDeleteNoteAsync(Guid idNote)
        {
            await using var context = _contextFactory.CreateDbContext();
            var note = await context.Notes.FirstOrDefaultAsync(n => n.IdNote == idNote);

            if (note == null)
                return false;

            note.IsDeleted = true;
            note.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }
    }
}