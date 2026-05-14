using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using SendGrid.Helpers.Errors.Model;
using System.Text.RegularExpressions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ContactHandler
    {
        private readonly IContactRepository _contactRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        private static readonly string[] ValidContactStatuses = { "NEW", "IN_PROGRESS", "SUCCESS", "FAILED", "CLOSED", "CANCELED" };

        public ContactHandler(IContactRepository contactRepository,
                              IStaffRepository staffRepository,
                              ILeadRepository leadRepository,
                              ICustomerRepository customerRepository,
                              IElasticsearchService elasticsearchService,
                              IMapper mapper)
        {
            _contactRepository = contactRepository;
            _staffRepository = staffRepository;
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
            _elasticsearchService = elasticsearchService;
            _mapper = mapper;
        }

        public async Task<ContactResponse> CreateContactAsync(ContactCreationRequest request)
        {
            ValidateContactCreation(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var lead = await _leadRepository.GetLeadByIdAsync(request.IdLead);
            if (lead == null)
            {
                throw new LeadNotFoundException();
            }

            var contact = _mapper.Map<Contact>(request);
            contact.IdStaff = request.IdStaff;
            contact.IdLead = request.IdLead;

            var createdContact = await _contactRepository.AddContactAsync(contact);

            var response = _mapper.Map<ContactResponse>(createdContact);
            response.Lead = _mapper.Map<LeadResponse>(lead);
            response.Staff = _mapper.Map<StaffResponse>(staff);

            await _elasticsearchService.IndexAsync(response, "contacts");

            return response;
        }

        public async Task<string> DeleteContactAsync(Guid idContact)
        {
            var result = await _contactRepository.SoftDeleteContactAsync(idContact);
            if (!result)
            {
                throw new ContactNotFoundException();
            }

            await _elasticsearchService.DeleteAsync<ContactResponse>(idContact.ToString(), "contacts");
            return "Xóa hoạt động thành công!";
        }

        public async Task<ContactResponse> UpdateContactAsync(ContactUpdateRequest request, Guid idContact)
        {
            var existingContact = await _contactRepository.GetContactByIdAsync(idContact);
            if (existingContact == null)
            {
                throw new ContactNotFoundException();
            }

            ValidateContactUpdate(request);

            existingContact.Type = request.Type ?? existingContact.Type;
            existingContact.Title = request.Title ?? existingContact.Title;
            existingContact.Content = request.Content ?? existingContact.Content;

            if (!string.IsNullOrEmpty(request.Status) && request.Status != existingContact.Status)
            {
                if (!ValidContactStatuses.Contains(request.Status.ToUpper()))
                {
                    throw new ValidationException($"Status '{request.Status}' không hợp lệ!");
                }

                if (request.Status.ToUpper() == "SUCCESS")
                {
                    var leadToConvert = await _leadRepository.GetLeadByIdAsync(existingContact.IdLead);
                    if (leadToConvert != null)
                    {
                        var staffWithEmail = await _staffRepository.GetStaffByEmailAsync(leadToConvert.Email);
                        if (staffWithEmail != null)
                        {
                            throw new EmailAlreadyExistsException();
                        }

                        leadToConvert.Discriminator = PersonType.Customer;
                        leadToConvert.UpdatedAt = DateTime.UtcNow;

                        await _leadRepository.UpdateLeadAsync(leadToConvert);
                    }
                }
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                existingContact.Status = request.Status.ToUpper();
            }
            existingContact.UpdatedAt = DateTime.UtcNow;

            var updatedContact = await _contactRepository.UpdateContactAsync(existingContact);

            var staff = await _staffRepository.GetStaffByIdAsync(updatedContact.IdStaff);

            Person? lead = null;
            if (updatedContact.Status == "SUCCESS")
            {
                lead = await _customerRepository.GetCustomerByIdAsync(updatedContact.IdLead);
            }
            else
            {
                try
                {
                    lead = await _leadRepository.GetLeadByIdAsync(updatedContact.IdLead);
                }
                catch (Domain.Exceptions.NotFoundException)
                {
                }
            }

            var response = _mapper.Map<ContactResponse>(updatedContact);
            response.Lead = lead != null ? _mapper.Map<LeadResponse>(lead) : null;
            response.Staff = _mapper.Map<StaffResponse>(staff);

            await _elasticsearchService.IndexAsync(response, "contacts");

            return response;
        }

        public async Task<ContactResponse> GetContactByIdAsync(Guid idContact)
        {
            var contact = await _contactRepository.GetContactByIdAsync(idContact);
            if (contact == null)
            {
                throw new ContactNotFoundException();
            }

            var response = _mapper.Map<ContactResponse>(contact);
            response.Lead = _mapper.Map<LeadResponse>(contact.IdLeadNavigation);
            response.Staff = _mapper.Map<StaffResponse>(contact.IdStaffNavigation);

            return response;
        }

        private void ValidateContactCreation(ContactCreationRequest request)
        {
            if (request.IdStaff == Guid.Empty)
                throw new InvalidGuidException("IdStaff");

            if (request.IdLead == Guid.Empty)
                throw new InvalidGuidException("IdLead");

            if (!string.IsNullOrEmpty(request.Type) && request.Type.Length > 50)
                throw new InvalidLengthException("Type", 1, 50);

            if (!string.IsNullOrEmpty(request.Title) && request.Title.Length > 100)
                throw new InvalidLengthException("Title", 1, 100);
        }

        private void ValidateContactUpdate(ContactUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Type) && request.Type.Length > 50)
                throw new InvalidLengthException("Type", 1, 50);

            if (!string.IsNullOrEmpty(request.Title) && request.Title.Length > 100)
                throw new InvalidLengthException("Title", 1, 100);
        }
    }
}