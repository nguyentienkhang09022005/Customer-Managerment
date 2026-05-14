using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class NoteMutation
    {
        private readonly NoteHandler _noteHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NoteMutation(NoteHandler noteHandler, IHttpContextAccessor httpContextAccessor)
        {
            _noteHandler = noteHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NoteResponse> CreateNoteAsync(NoteInput input)
        {
            var request = new NoteCreationRequest
            {
                Content = input.Content,
                Type = input.Type.ToString(),
                IdStaff = input.IdStaff,
                LinkedEntityType = input.LinkedEntityType,
                LinkedEntityId = input.LinkedEntityId,
                ParentNoteId = input.ParentNoteId
            };

            return await _noteHandler.CreateNoteAsync(request);
        }

        public async Task<NoteResponse> UpdateNoteAsync(NoteUpdateInput input, Guid idNote)
        {
            var request = new NoteUpdateRequest
            {
                Content = input.Content,
                IsPinned = input.IsPinned
            };

            return await _noteHandler.UpdateNoteAsync(request, idNote);
        }

        public async Task<bool> DeleteNoteAsync(Guid idNote)
        {
            await _noteHandler.DeleteNoteAsync(idNote);
            return true;
        }

        public async Task<NoteResponse> PinNoteAsync(Guid idNote)
        {
            return await _noteHandler.PinNoteAsync(idNote);
        }

        public async Task<NoteResponse> UnpinNoteAsync(Guid idNote)
        {
            return await _noteHandler.UnpinNoteAsync(idNote);
        }

        public async Task<NoteResponse> ReplyNoteAsync(Guid idNote, Guid parentId)
        {
            return await _noteHandler.ReplyNoteAsync(idNote, parentId);
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}