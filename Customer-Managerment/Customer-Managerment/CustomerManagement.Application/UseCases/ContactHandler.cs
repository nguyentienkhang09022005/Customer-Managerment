using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ContactHandler
    {
        private readonly IContactRepository _contactRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;

        public ContactHandler(IContactRepository contactRepository, 
                              IStaffRepository staffRepository, 
                              ILeadRepository leadRepository,
                              IMapper mapper)
        {
            _contactRepository = contactRepository;
            _staffRepository = staffRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
        }

        public async Task<ContactResponse> CreateContactAsync(ContactCreationRequest contactCreationRequest)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(contactCreationRequest.IdStaff);
            if (staff == null){
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }

            var lead = await _leadRepository.GetLeadByIdAsync(contactCreationRequest.IdLead);
            if (lead == null){
                throw new DomainException("Khách hàng tiềm năng không tồn tại!", 404);
            }

            var contactDomain = _mapper.Map<ContactDomain>(contactCreationRequest);

            var createdContact = await _contactRepository.AddContactAsync(contactDomain);
            
            var contactResponse = _mapper.Map<ContactResponse>(createdContact);
            //contactResponse.infLeadResponse = _mapper.Map<LeadResponse>(lead);
            contactResponse.infStaffResponse = _mapper.Map<StaffResponse>(staff);
            return contactResponse;
        }

        public async Task<string> DeleteContactAsync(Guid idContact)
        {
            await _contactRepository.DeleteContactAsync(idContact);

            return "Xóa hoạt động thành công!";
        }

        public async Task<ContactResponse> UpdateContactAsync(ContactUpdateRequest contactUpdateRequest, Guid idContact)
        {
            var existContact = await _contactRepository.GetContactByIdAsync(idContact);
            if (existContact == null){
                throw new DomainException("Hoạt động không tồn tại!", 404);
            }
            _mapper.Map(contactUpdateRequest, existContact);

            var updatedContact = await _contactRepository.UpdateContactAsync(existContact);

            var staff = await _staffRepository.GetStaffByIdAsync(updatedContact.IdStaff);
            if (staff == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }

            var lead = await _leadRepository.GetLeadByIdAsync(updatedContact.IdLead);
            if (lead == null)
            {
                throw new DomainException("Khách hàng tiềm năng không tồn tại!", 404);
            }

            var contactResponse = _mapper.Map<ContactResponse>(updatedContact);
            //contactResponse.infLeadResponse = _mapper.Map<LeadResponse>(lead);
            contactResponse.infStaffResponse = _mapper.Map<StaffResponse>(staff);
            return contactResponse;
        }

        public async Task<ContactResponse> GetContactByIdAsync(Guid idContact)
        {
            var contactDomain = await _contactRepository.GetContactByIdAsync(idContact);
            if (contactDomain == null){
                throw new DomainException("Hoạt động không tồn tại!", 404);
            }

            var contactResponse = _mapper.Map<ContactResponse>(contactDomain);
            contactResponse.infLeadResponse = _mapper.Map<LeadResponse>(contactDomain.IdLead);
            contactResponse.infStaffResponse = _mapper.Map<StaffResponse>(contactDomain.IdStaff);
            return contactResponse;
        }

        public async Task<List<ContactResponse>> GetAllContactsAsync()
        {
            var contactDomains = await _contactRepository.GetListContactAsync();
            return _mapper.Map<List<ContactResponse>>(contactDomains);
        }
    }
}
