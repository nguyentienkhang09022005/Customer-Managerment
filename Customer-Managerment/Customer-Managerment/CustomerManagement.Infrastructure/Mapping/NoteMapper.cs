using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class NoteMapper : Profile
    {
        public NoteMapper()
        {
            // Note -> NoteResponse
            CreateMap<Note, NoteResponse>()
                .ForMember(dest => dest.IdNote, opt => opt.MapFrom(src => src.IdNote))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.LinkedEntityType, opt => opt.MapFrom(src => src.LinkedEntityType))
                .ForMember(dest => dest.LinkedEntityId, opt => opt.MapFrom(src => src.LinkedEntityId))
                .ForMember(dest => dest.ParentNoteId, opt => opt.MapFrom(src => src.ParentNoteId))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation))
                .ForMember(dest => dest.Replies, opt => opt.Ignore())
                .ForMember(dest => dest.Mentions, opt => opt.MapFrom(src => src.Mentions));

            // NoteMention -> NoteMentionResponse
            CreateMap<NoteMention, NoteMentionResponse>()
                .ForMember(dest => dest.IdMention, opt => opt.MapFrom(src => src.IdMention))
                .ForMember(dest => dest.IdNote, opt => opt.MapFrom(src => src.IdNote))
                .ForMember(dest => dest.IdStaffMentioned, opt => opt.MapFrom(src => src.IdStaffMentioned))
                .ForMember(dest => dest.StaffMentioned, opt => opt.MapFrom(src => src.IdStaffMentionedNavigation))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // NoteCreationRequest -> Note
            CreateMap<NoteCreationRequest, Note>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.LinkedEntityType, opt => opt.MapFrom(src => src.LinkedEntityType))
                .ForMember(dest => dest.LinkedEntityId, opt => opt.MapFrom(src => src.LinkedEntityId))
                .ForMember(dest => dest.ParentNoteId, opt => opt.MapFrom(src => src.ParentNoteId))
                .ForMember(dest => dest.IdNote, opt => opt.Ignore())
                .ForMember(dest => dest.IsPinned, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IdStaffNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.ParentNote, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore())
                .ForMember(dest => dest.Mentions, opt => opt.Ignore());

            // NoteUpdateRequest -> Note
            CreateMap<NoteUpdateRequest, Note>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned));
        }
    }
}