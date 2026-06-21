using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class NoteQuery
    {
        private readonly INoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public NoteQuery(INoteRepository noteRepository, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<NoteResponse> GetNotesByEntity(string entityType, Guid entityId)
        {
            var notes = _noteRepository.GetNotesByEntityAsync(entityType, entityId).Result;
            return notes.ProjectTo<NoteResponse>(_mapper.ConfigurationProvider);
        }

        public NoteResponse? GetNoteById(Guid idNote)
        {
            var note = _noteRepository.GetNoteByIdAsync(idNote).Result;
            return note == null ? null : _mapper.Map<NoteResponse>(note);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<NoteResponse> GetPinnedNotes(string entityType, Guid entityId)
        {
            var notes = _noteRepository.GetPinnedNotesAsync(entityType, entityId).Result;
            return notes.ProjectTo<NoteResponse>(_mapper.ConfigurationProvider);
        }
    }
}