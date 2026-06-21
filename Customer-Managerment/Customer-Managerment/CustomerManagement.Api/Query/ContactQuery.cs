using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class ContactQuery
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public ContactQuery(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<ContactResponse> GetContacts()
        {
            var contacts = _contactRepository.GetListContact();
            return contacts.ProjectTo<ContactResponse>(_mapper.ConfigurationProvider);
        }

        public IQueryable<ContactResponse> GetContactById(Guid idContact)
        {
            var contact = _contactRepository.GetContactById(idContact);
            return contact.ProjectTo<ContactResponse>(_mapper.ConfigurationProvider);
        }

        public async Task<PagedResponse<ContactResponse>> GetContactsPaged(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200;

            var (items, totalCount) = await _contactRepository.GetListContactPagedAsync(page, pageSize);
            var dtoItems = _mapper.Map<List<ContactResponse>>(items);
            return new PagedResponse<ContactResponse> { Items = dtoItems, TotalCount = totalCount };
        }
    }
}
