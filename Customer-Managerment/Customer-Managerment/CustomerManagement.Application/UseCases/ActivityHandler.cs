using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Activities
{
    public class ActivityHandler
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityHandler> _logger;


        public ActivityHandler(IActivityRepository activityRepository, 
                               IUserRepository userRepository, 
                               IMapper mapper, ILogger<ActivityHandler> logger)
        {
            _activityRepository = activityRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ActivityResponse>> GetListActivitiesAsync(Guid idUser)
        {
            var activities = await _activityRepository.GetListActivitiesAsync(idUser);
            return _mapper.Map<List<ActivityResponse>>(activities);
        }

        public async Task<ActivityResponse> GetInfActivityAsync(Guid idActivity)
        {
            var activity = await _activityRepository.GetActivityByIdAsync(idActivity);
            return _mapper.Map<ActivityResponse>(activity);
        }

        public async Task<ActivityResponse> CreateActivityAsync(ActivityCreationRequest request)
        {
            bool userExists = await _userRepository.CheckUserExistsAsync(request.IdUser);
            if (!userExists)
                throw new DomainException("Người dùng không tồn tại, không thể tạo hoạt động!", 404);

            var domain = _mapper.Map<ActivityDomain>(request);

            var newActivity = await _activityRepository.AddActivityAsync(domain);
            return _mapper.Map<ActivityResponse>(newActivity);
        }

        public async Task<ActivityResponse> UpdateActivityAsync(ActivityUpdateRequest request, Guid idActivity)
        {
            var existing = await _activityRepository.GetExistingActivityAsync(idActivity);
            if (existing == null)
                throw new DomainException("Không tìm thấy hoạt động cần cập nhật!", 404);

            var domain = _mapper.Map<ActivityDomain>(existing);
            _mapper.Map(request, domain);

            var updated = await _activityRepository.UpdateActivityAsync(domain, existing);
            return _mapper.Map<ActivityResponse>(updated);
        }

        public async Task<string> DeleteActivityAsync(Guid idActivity)
        {
            await _activityRepository.DeleteActivityAsync(idActivity);
            return "Xóa hoạt động thành công!";
        }
    }
}
