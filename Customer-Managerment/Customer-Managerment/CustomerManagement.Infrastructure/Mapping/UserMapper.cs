using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDomain>();

            CreateMap<UserDomain, User>()
                .ForMember(dest => dest.IdUser, opt => opt.Ignore());

            CreateMap<UserDomain, UserResponse>();

            CreateMap<User, UserResponse>();

            CreateMap<UserCreationRequest, UserDomain>();

            CreateMap<UserUpdateRequest, UserDomain>();
        }
    }
}
