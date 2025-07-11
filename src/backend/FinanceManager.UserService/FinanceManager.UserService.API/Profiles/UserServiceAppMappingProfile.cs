using AutoMapper;
using FinanceManager.UserService.Contracts.DTOs;
using FinanceManager.UserService.Domain.Abstractions;
using FinanceManager.UserService.Domain.Entities;
using TimeZoneEntity = FinanceManager.UserService.Domain.Entities.TimeZone;

namespace FinanceManager.UserService.API.Profiles
{
    internal class UserServiceAppMappingProfile : Profile
    {
        public UserServiceAppMappingProfile() 
        {
            CreateMap<IdentityModel, BaseDto>();
            CreateMap<User, UserDto>();
            CreateMap<TimeZoneEntity, TimeZoneDto>();
            CreateMap<RefreshToken, RefreshTokenDto>();
        }
    }
}
