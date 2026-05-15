using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class NoteHandler
    {
        private readonly INoteRepository _noteRepository;
        private readonly INoteMentionRepository _noteMentionRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly INotificationRepository _notificationRepository;
        // private readonly IElasticsearchService _elasticsearchService;
        private readonly IMapper _mapper;

        public NoteHandler(
            INoteRepository noteRepository,
            INoteMentionRepository noteMentionRepository,
            IStaffRepository staffRepository,
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _noteRepository = noteRepository;
            _noteMentionRepository = noteMentionRepository;
            _staffRepository = staffRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<NoteResponse> CreateNoteAsync(NoteCreationRequest request)
        {
            ValidateNoteCreation(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var note = _mapper.Map<Note>(request);

            var createdNote = await _noteRepository.AddNoteAsync(note);

            // Process @mentions in content
            await ProcessMentionsAsync(createdNote, staff.Fullname);

            var response = _mapper.Map<NoteResponse>(createdNote);
            response.Staff = _mapper.Map<StaffResponse>(staff);

            // await _elasticsearchService.IndexAsync(response, "notes");

            return response;
        }

        public async Task<string> DeleteNoteAsync(Guid idNote)
        {
            var result = await _noteRepository.SoftDeleteNoteAsync(idNote);
            if (!result)
            {
                throw new NoteNotFoundException();
            }

            // await _elasticsearchService.DeleteAsync<NoteResponse>(idNote.ToString(), "notes");
            return "Xóa bình luận thành công!";
        }

        public async Task<NoteResponse> UpdateNoteAsync(NoteUpdateRequest request, Guid idNote)
        {
            var existingNote = await _noteRepository.GetNoteByIdAsync(idNote);
            if (existingNote == null)
            {
                throw new NoteNotFoundException();
            }

            ValidateNoteUpdate(request);

            if (!string.IsNullOrEmpty(request.Content))
                existingNote.Content = request.Content;
            if (request.IsPinned.HasValue)
                existingNote.IsPinned = request.IsPinned.Value;

            existingNote.UpdatedAt = DateTime.UtcNow;

            var updatedNote = await _noteRepository.UpdateNoteAsync(existingNote);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedNote.IdStaff);
            var response = _mapper.Map<NoteResponse>(updatedNote);
            response.Staff = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "notes");
            return response;
        }

        public async Task<NoteResponse> PinNoteAsync(Guid idNote)
        {
            var existingNote = await _noteRepository.GetNoteByIdAsync(idNote);
            if (existingNote == null)
            {
                throw new NoteNotFoundException();
            }

            existingNote.IsPinned = true;
            existingNote.UpdatedAt = DateTime.UtcNow;

            var updatedNote = await _noteRepository.UpdateNoteAsync(existingNote);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedNote.IdStaff);
            var response = _mapper.Map<NoteResponse>(updatedNote);
            response.Staff = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "notes");
            return response;
        }

        public async Task<NoteResponse> UnpinNoteAsync(Guid idNote)
        {
            var existingNote = await _noteRepository.GetNoteByIdAsync(idNote);
            if (existingNote == null)
            {
                throw new NoteNotFoundException();
            }

            existingNote.IsPinned = false;
            existingNote.UpdatedAt = DateTime.UtcNow;

            var updatedNote = await _noteRepository.UpdateNoteAsync(existingNote);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedNote.IdStaff);
            var response = _mapper.Map<NoteResponse>(updatedNote);
            response.Staff = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "notes");
            return response;
        }

        public async Task<NoteResponse> ReplyNoteAsync(Guid idNote, Guid parentId)
        {
            var parentNote = await _noteRepository.GetNoteByIdAsync(parentId);
            if (parentNote == null)
            {
                throw new NoteNotFoundException("Parent note không tìm thấy!");
            }

            var note = await _noteRepository.GetNoteByIdAsync(idNote);
            if (note == null)
            {
                throw new NoteNotFoundException();
            }

            note.ParentNoteId = parentId;
            note.LinkedEntityType = parentNote.LinkedEntityType;
            note.LinkedEntityId = parentNote.LinkedEntityId;
            note.UpdatedAt = DateTime.UtcNow;

            var updatedNote = await _noteRepository.UpdateNoteAsync(note);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedNote.IdStaff);
            var response = _mapper.Map<NoteResponse>(updatedNote);
            response.Staff = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "notes");
            return response;
        }

        public async Task<NoteResponse> GetNoteByIdAsync(Guid idNote)
        {
            var note = await _noteRepository.GetNoteByIdAsync(idNote);
            if (note == null)
            {
                throw new NoteNotFoundException();
            }
            return _mapper.Map<NoteResponse>(note);
        }

        private async Task ProcessMentionsAsync(Note note, string authorName)
        {
            // Find @mentions in content using regex
            var mentionPattern = new Regex(@"@(\w+)");
            var matches = mentionPattern.Matches(note.Content);

            foreach (Match match in matches)
            {
                var username = match.Groups[1].Value;
                var staff = await _staffRepository.GetStaffByUsernameAsync(username);

                if (staff != null)
                {
                    // Create note mention
                    var mention = new NoteMention
                    {
                        IdNote = note.IdNote,
                        IdStaffMentioned = staff.Id
                    };
                    await _noteMentionRepository.AddMentionAsync(mention);

                    // Create notification for mentioned staff
                    var notification = new Notification
                    {
                        Title = "Bạn được nhắn đến trong một bình luận",
                        Message = $"{authorName} đã nhắn đến bạn trong một bình luận: {note.Content.Substring(0, Math.Min(50, note.Content.Length))}...",
                        Type = NotificationTypeConstant.NotificationMention,
                        IdStaff = staff.Id,
                        RelatedEntityType = "Note",
                        RelatedEntityId = note.IdNote
                    };
                    await _notificationRepository.AddNotificationAsync(notification);
                }
            }
        }

        private void ValidateNoteCreation(NoteCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new RequiredFieldException("Content");

            if (request.Content.Length > 5000)
                throw new InvalidLengthException("Content", 1, 5000);

            if (!NoteTypeConstant.IsValid(request.Type))
                throw new ValidationException("Type không hợp lệ!");

            if (!TaskLinkedEntityConstant.IsValid(request.LinkedEntityType))
                throw new ValidationException("LinkedEntityType không hợp lệ!");
        }

        private void ValidateNoteUpdate(NoteUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Content))
            {
                if (request.Content.Length > 5000)
                    throw new InvalidLengthException("Content", 1, 5000);
            }
        }
    }
}