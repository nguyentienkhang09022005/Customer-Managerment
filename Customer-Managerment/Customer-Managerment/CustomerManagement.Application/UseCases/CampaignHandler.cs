using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Campaigns
{
    public class CampaignHandler
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CampaignHandler(ICampaignRepository campaignRepository, IUserRepository userRepository, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<CampaignResponse>> GetListCampaignsAsync(Guid idUser)
        {
            var list = await _campaignRepository.GetListCampaignsAsync(idUser);
            return _mapper.Map<List<CampaignResponse>>(list);
        }

        public async Task<CampaignResponse> GetInfCampaignAsync(Guid idCampaign)
        {
            var campaign = await _campaignRepository.GetCampaignByIdAsync(idCampaign);
            return _mapper.Map<CampaignResponse>(campaign);
        }

        public async Task<CampaignResponse> CreateCampaignAsync(CampaignCreationRequest req)
        {
            if (!await _userRepository.CheckUserExistsAsync(req.IdUser))
                throw new DomainException("Người dùng không tồn tại!", 404);

            var domain = _mapper.Map<CampaignDomain>(req);

            var entity = await _campaignRepository.AddCampaignAsync(domain);
            return _mapper.Map<CampaignResponse>(entity);
        }

        public async Task<CampaignResponse> UpdateCampaignAsync(CampaignUpdateRequest req, Guid idCampaign)
        {
            var campaign = await _campaignRepository.GetExistingCampaignAsync(idCampaign);
            if (campaign == null) throw new DomainException("Không tìm thấy chiến dịch!", 404);

            var domain = _mapper.Map<CampaignDomain>(campaign);
            _mapper.Map(req, domain);
            var updated = await _campaignRepository.UpdateCampaignAsync(domain, campaign);
            return _mapper.Map<CampaignResponse>(updated);
        }

        public async Task<string> DeleteCampaignAsync(Guid idCampaign)
        {
            await _campaignRepository.DeleteCampaignAsync(idCampaign);
            return "Xóa chiến dịch thành công!";
        }
    }
}
