using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    [Authorize]
    public class NoteMutation
    {
        private readonly NoteHandler _noteHandler;
        private readonly IRealtimeNoteService _realtimeNoteService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NoteMutation(
            NoteHandler noteHandler,
            IRealtimeNoteService realtimeNoteService,
            IHttpContextAccessor httpContextAccessor)
        {
            _noteHandler = noteHandler;
            _realtimeNoteService = realtimeNoteService;
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

            var result = await _noteHandler.CreateNoteAsync(request);

            // Send realtime notification for new note
            if (!string.IsNullOrEmpty(result.LinkedEntityType))
            {
                await _realtimeNoteService.SendNoteToEntityAsync(
                    result.LinkedEntityType,
                    result.LinkedEntityId,
                    result);
            }

            return result;
        }

        public async Task<NoteResponse> UpdateNoteAsync(NoteUpdateInput input, Guid idNote)
        {
            var request = new NoteUpdateRequest
            {
                Content = input.Content,
                IsPinned = input.IsPinned
            };

            var result = await _noteHandler.UpdateNoteAsync(request, idNote);

            // Send realtime notification for updated note
            if (!string.IsNullOrEmpty(result.LinkedEntityType))
            {
                await _realtimeNoteService.SendNoteToEntityAsync(
                    result.LinkedEntityType,
                    result.LinkedEntityId,
                    result);
            }

            return result;
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
            var result = await _noteHandler.ReplyNoteAsync(idNote, parentId);

            // Send realtime for reply
            if (!string.IsNullOrEmpty(result.LinkedEntityType))
            {
                await _realtimeNoteService.SendNoteToEntityAsync(
                    result.LinkedEntityType,
                    result.LinkedEntityId,
                    result);
            }

            return result;
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}