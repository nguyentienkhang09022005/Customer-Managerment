using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class DealHandler
    {
        private readonly IDealRepository _dealRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        public DealHandler(IDealRepository dealRepository, 
                           IStaffRepository staffRepository, 
                           ICustomerRepository customerRepository, 
                           IMapper mapper,
                           IElasticsearchService elasticsearchService)
        {
            _dealRepository = dealRepository;
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
        }


        public async Task<DealResponse> CreateDealAsync(DealCreationRequest dealCreationRequest)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(dealCreationRequest.IdStaff);
            if (staff == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(dealCreationRequest.IdCustomer);
            if (customer == null)
            {
                throw new DomainException("Khách hàng không tồn tại!", 404);
            }

            var dealDomain = _mapper.Map<DealDomain>(dealCreationRequest);

            var createdDeal = await _dealRepository.AddDealAsync(dealDomain);

            var dealResponse = _mapper.Map<DealResponse>(createdDeal);
            dealResponse.infCustomerResponse = _mapper.Map<CustomerResponse>(customer);
            dealResponse.infStaffResponse = _mapper.Map<StaffResponse>(staff);

            await _elasticsearchService.IndexAsync(dealResponse, "deals");
            return dealResponse;
        }

        public async Task<string> DeleteDealAsync(Guid idDeal)
        {
            await _dealRepository.DeleteDealAsync(idDeal);
            await _elasticsearchService.DeleteAsync<DealResponse>(idDeal.ToString(), "deals"); // Xóa khỏi Elasticsearch

            return "Xóa deal thành công!";
        }

        public async Task<DealResponse> UpdateDealAsync(DealUpdateRequest dealUpdateRequest, Guid idDeal)
        {
            var existDeal = await _dealRepository.GetDealByIdAsync(idDeal);
            if (existDeal == null)
            {
                throw new DomainException("Deal không tồn tại!", 404);
            }
            _mapper.Map(dealUpdateRequest, existDeal);

            var updatedDeal = await _dealRepository.UpdateDealAsync(existDeal);

            var staff = await _staffRepository.GetStaffByIdAsync(updatedDeal.IdStaff);
            if (staff == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(updatedDeal.IdCustomer);
            if (customer == null)
            {
                throw new DomainException("Khách hàng không tồn tại!", 404);
            }

            var dealResponse = _mapper.Map<DealResponse>(updatedDeal);
            dealResponse.infCustomerResponse = _mapper.Map<CustomerResponse>(customer);
            dealResponse.infStaffResponse = _mapper.Map<StaffResponse>(staff);
            return dealResponse;
        }

        public async Task<DealResponse> GetDealByIdAsync(Guid idDeal)
        {
            var dealDomain = await _dealRepository.GetDealByIdAsync(idDeal);
            if (dealDomain == null)
            {
                throw new DomainException("Deal không tồn tại!", 404);
            }

            var dealResponse = _mapper.Map<DealResponse>(dealDomain);
            dealResponse.infCustomerResponse = _mapper.Map<CustomerResponse>(dealDomain.IdCustomer);
            dealResponse.infStaffResponse = _mapper.Map<StaffResponse>(dealDomain.IdStaff);
            return dealResponse;
        }
    }
}
