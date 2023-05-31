using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IRideService rideService;
        private readonly ILocationService locationService;
        private readonly IMapper mapper;
        public PassengerService(IUnitOfWork unitOfWork, IUserService userService, IRideService rideService, ILocationService locationService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.rideService = rideService;
            this.locationService = locationService;
            this.mapper = mapper;
        }

        public async Task<Passenger> GetById(int id)
        {
            return await unitOfWork.PassengerRepository.GetById(id);
        }
        public async Task<PassengerDto> AskForRide(PassengerInput input)
        {
            //Get user id
            int idUser = (await userService.GetByEmail(input.Email)).Id;

            //Validate if ride exists and its available
            var ride = await rideService.GetById(input.Ride);
            bool isAvailable = Convert.ToBoolean(ride?.IsAvailable);
            if (ride == null || !isAvailable)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }

            //Validate if initial location exists
            var initialLocation = await locationService.GetById(input.InitialLocation);
            if (initialLocation == null || initialLocation.User != idUser)
            {
                throw new Exception("LOCATION_NOT_FOUND");
            }

            //TODO - Validate if user already asked for ride
            
            //...

            //Create passenger
            var item = mapper.Map<Passenger>(input);
            item.User = idUser;
            item.TimeSolicited = DateTime.Now;

            await unitOfWork.PassengerRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<PassengerDto>(item);
        }
    }
}
