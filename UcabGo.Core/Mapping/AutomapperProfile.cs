using AutoMapper;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Destination.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Inputs;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Core.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<RegisterInput, User>();

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<VehicleDto, Vehicle>();
            CreateMap<VehicleInput, Vehicle>();

            CreateMap<Soscontact, SoscontactDto>();
            CreateMap<SoscontactDto, Soscontact>();
            CreateMap<SoscontactInput, Soscontact>();

            CreateMap<Destination, DestinationDto>();
            CreateMap<DestinationDto, Destination>();
            CreateMap<DestinationInput, Destination>();

            CreateMap<Ride, RideDto>()
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
                .ForMember(dest => dest.Destination, opt => opt.Ignore());
            CreateMap<RideDto, Ride>();
            CreateMap<RideInput, Ride>();
        }
    }
}
