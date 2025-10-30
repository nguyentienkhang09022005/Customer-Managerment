using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
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
    }
}
