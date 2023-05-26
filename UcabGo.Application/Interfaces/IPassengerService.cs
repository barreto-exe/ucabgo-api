﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IPassengerService
    {
        Task<Passenger> GetById(int id);
        Task<IEnumerable<PassengerDto>> GetPassengersByRide(int rideId);
        Task<PassengerDto> AcceptPassenger(string driverEmail, int rideId, int passengerId);
        Task<PassengerDto> IgnorePassenger(string driverEmail, int rideId, int passengerId);
        Task<PassengerDto> CancelPassenger(string driverEmail, int rideId, int passengerId);
    }
}
