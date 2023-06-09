﻿using AutoMapper;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.Chat.Dtos;
using UcabGo.Core.Data.Chat.Input;
using UcabGo.Core.Data.Evaluation.Dtos;
using UcabGo.Core.Data.Evaluation.Inputs;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Inputs;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.User.Inputs;
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
            CreateMap<UserUpdateInput, User>();

            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(x => x.UserNavigation));
            CreateMap<VehicleDto, Vehicle>();
            CreateMap<VehicleInput, Vehicle>();

            CreateMap<Soscontact, SoscontactDto>();
            CreateMap<SoscontactDto, Soscontact>();
            CreateMap<SoscontactInput, Soscontact>();

            CreateMap<Ride, RideDto>()
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(x => x.DriverNavigation))
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(x => x.VehicleNavigation))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(x => x.FinalLocationNavigation));
            CreateMap<RideDto, Ride>();
            CreateMap<RideInput, Ride>()
                .ForMember(dest => dest.FinalLocation, opt => opt.MapFrom(x => x.Destination));

            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(x => x.UserNavigation));
            CreateMap<LocationDto, Location>();
            CreateMap<LocationInput, Location>();
            CreateMap<HomeInput, LocationInput>();

            CreateMap<Passenger, PassengerDto>()
                .ForMember(dest => dest.FinalLocation, opt => opt.MapFrom(x => x.FinalLocationNavigation))
                .ForMember(dest => dest.User, opt => opt.MapFrom(x => x.UserNavigation));
            CreateMap<PassengerDto, Passenger>();
            CreateMap<PassengerInput, Passenger>();

            CreateMap<Chatmessage, ChatmessageDto>();
            CreateMap<ChatmessageDto, Chatmessage>();
            CreateMap<ChatmessageInput, Chatmessage>();

            CreateMap<Evaluation, EvaluationDto>()
                .ForMember(dest => dest.Evaluated, opt => opt.MapFrom(x => x.EvaluatedNavigation))
                .ForMember(dest => dest.Evaluator, opt => opt.MapFrom(x => x.EvaluatorNavigation))
                .ForMember(dest => dest.Ride, opt => opt.MapFrom(x => x.RideNavigation));
            CreateMap<EvaluationDto, Evaluation>();
            CreateMap<EvaluationInput, Evaluation>()
                .ForMember(dest => dest.Ride, opt => opt.MapFrom(x => x.RideId))
                .ForMember(dest => dest.Evaluator, opt => opt.MapFrom(x => x.EvaluatorId))
                .ForMember(dest => dest.Evaluated, opt => opt.MapFrom(x => x.EvaluatedId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(x => x.EvaluatorType));

        }
    }
}
