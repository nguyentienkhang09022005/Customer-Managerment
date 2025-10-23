using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class UsersHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        public UsersHandler(IUserRepository userRepository, IMapper mapper, IElasticsearchService elasticsearchService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<List<UserResponse>> GetListUsersAsync()
        {
            var users = await _userRepository.GetListUsersAsync();
            return _mapper.Map<List<UserResponse>>(users);
        }

        public async Task<UserResponse> GetInfUserAsync(Guid idUser)
        {
            var infUser = await _userRepository.GetUserByIdAsync(idUser);
            return _mapper.Map<UserResponse>(infUser);
        }

        public async Task<UserResponse> CreateUserAsync(UserCreationRequest userCreationRequest)
        {
            var userExists = await _userRepository.GetUserByEmailAsync(userCreationRequest.Email);
            if (userExists != null)
                throw new DomainException("Email này đã được sử dụng!", 409);


            var userDomain = _mapper.Map<UserDomain>(userCreationRequest);
            var newUser = await _userRepository.AddUserAsync(userDomain);

            var userResponse = _mapper.Map<UserResponse>(newUser);

            await _elasticsearchService.IndexUserAsync(userResponse); // Thêm thông tin vào ES

            return userResponse;
        }

        public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest userUpdateRequest, Guid idUser)
        {
            var userEntity = await _userRepository.GetExistingUserAsync(idUser);
            if (userEntity == null)
            {
                throw new DomainException("Không tìm thấy người dùng cần đổi thông tin", 404);
            }
            var userDomain = _mapper.Map<UserDomain>(userEntity);

            _mapper.Map(userUpdateRequest, userDomain);

            var updatedUser = await _userRepository.UpdateUserAsync(userDomain);
            var userResponse = _mapper.Map<UserResponse>(updatedUser);

            await _elasticsearchService.IndexUserAsync(userResponse); // Sửa thông tin trong ES
            return userResponse;
        }

        public async Task<string> DeleteUserAsync(Guid idUser)
        {
            await _userRepository.DeleteUserAsync(idUser);

            await _elasticsearchService.DeleteUserAsync(idUser); // Xóa người dùng khỏi ES
            return "Xóa người dùng thành công!";
        }

        public async Task<List<UserResponse>> SearchUsersAsync(string keyword)
        {
            return await _elasticsearchService.SearchUsersAsync(keyword);
        }
    }
}
