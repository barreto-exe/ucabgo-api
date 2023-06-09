﻿using AutoMapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class RideService : IRideService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        private readonly IUserService userService;
        private readonly ILocationService locationService;
        public RideService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, ILocationService locationService, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.locationService = locationService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /*public async Task<IEnumerable<RideDto>> matchmaking_knn(UserDto solicitante, LocationDto ubicacion, bool sentidoUcab)
        {

            // Obtenemos rutas
            var routes = unitOfWork.RideRepository.GetAll();

            // K vecinos más cercanos a considerar
            // Esto ya no se usa, pero podría usarse en un futuro
            //int k = 10;

            // Lista de tuplas
            List<(int Id,double Distance)> ridesWithDistance = new List<(int,double)>();

            // Calculamos la distancia
            foreach (var r in routes)
            {
                var rDestination = await locationService.GetById(r.FinalLocation);
                double dist = GeoDistance(rDestination.Latitude, ubicacion.Latitude, rDestination.Longitude, ubicacion.Longitude);

                (int, double) rideAndDist = (r.Id, dist);
                ridesWithDistance.Add(rideAndDist);
            }

            // Ordenamos
            ridesWithDistance.Sort((a, b) => {
                int cmp = a.Distance.CompareTo(b.Distance);
                return cmp;
            });

            // Eliminamos las rutas que no están dentro del margen de caminata y añadimos las que sí
            List<RideDto> matchedRoutes = new List<RideDto>();
            foreach (var r in ridesWithDistance)
            {
                if (r.Distance >= solicitante.WalkingDistance)
                {
                    ridesWithDistance.Remove(r);
                }
                else
                {
                    var itemDb = await GetById(r.Id);
                    var dto = mapper.Map<RideDto>(itemDb);
                    dto.Driver = mapper.Map<UserDto>(itemDb.Driver);
                    dto.Vehicle = mapper.Map<VehicleDto>(itemDb.Vehicle);
                    dto.Destination = mapper.Map<LocationDto>(itemDb.FinalLocation);
                    matchedRoutes.Add(dto);
                }
            }

            return matchedRoutes;
        }*/

        //Haversine formula algorith to obtain the distance in meters between two geographical points
        public static double GeoDistance(double lat1, double lon1, double lat2, double lon2)
        {

            // Earth radius
            double radio_tierra = 6371000;

            // Convert latitudes and longitudes to radians
            double lat1_r = Math.PI * lat1 / 180.0;
            double lon1_r = Math.PI * lon1 / 180.0;
            double lat2_r = Math.PI * lat2 / 180.0;
            double lon2_r = Math.PI * lon2 / 180.0;

            double delta_lat = lat2_r - lat1_r;
            double delta_lon = lon2_r - lon1_r;

            double a = Math.Pow(Math.Sin(delta_lat / 2), 2) + Math.Cos(lat1_r) * Math.Cos(lat2_r) * Math.Pow(Math.Sin(delta_lon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distancia = radio_tierra * c;

            return distancia;
        }

        public async Task<IEnumerable<RideMatchDto>> GetMatchingAll(MatchingFilter filter)
        {
            var items = 
                unitOfWork.RideRepository.GetAllIncluding(
                    "VehicleNavigation",
                    "FinalLocationNavigation",
                    "DriverNavigation",
                    "Passengers.UserNavigation",
                    "Passengers.FinalLocationNavigation");

            var rides = from r in items
                        where r.IsAvailable &&
                              r.DriverNavigation.Email != filter.Email 
                        select new RideMatchDto
                        {
                            Ride = mapper.Map<RideDto>(r),
                        };

            var result =
                rides
                .ToList()
                .Where(r => filter.GoingToCampus ? 
                     r.Ride.Destination.Alias.Contains("UCAB") : 
                    !r.Ride.Destination.Alias.Contains("UCAB"))
                .OrderByDescending(x => x.MatchingPercentage)
                .ToList();

            //foreach (var r in result)
            //{
            //    if (filter.GoingToCampus)
            //    {
            //        r.MatchingPercentage = 1 - GeoDistance(r.Ride.LatitudeOrigin, filter.InitialLatitude, r.Ride.LongitudeOrigin, filter.InitialLongitude);
            //    }
            //    else
            //    {
            //        r.MatchingPercentage = 1 - GeoDistance(r.Ride.Destination.Latitude, filter.FinalLatitude, r.Ride.Destination.Longitude, filter.FinalLongitude);

            //    }

            //    if (r.MatchingPercentage < 0)
            //    {
            //        //result.Remove(r);
            //    }
            //}
            //result = result.OrderByDescending(x => x.MatchingPercentage).ToList();

            foreach (var r in result)
            {
                //Random between 0.7 and 1
                r.MatchingPercentage = 0.7 + (new Random().NextDouble() * 0.3);
            }
            return result.OrderByDescending(x => x.MatchingPercentage);
        }

        public async Task<IEnumerable<RideDto>> GetAll(string driverEmail, bool onlyAvailable = false)
        {
            var items = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.FinalLocationNavigation,
                    r => r.DriverNavigation,
                    r => r.Evaluations);

            IQueryable<Ride>? ridesFromDriver;
            if (onlyAvailable)
            {
                ridesFromDriver = from r in items
                                  where
                                    r.DriverNavigation.Email == driverEmail &&
                                    (r.IsAvailable ||
                                    (r.TimeStarted != null && r.TimeEnded == null && r.TimeCanceled == null))
                                  select r;
            }
            else
            {
                ridesFromDriver = from r in items
                                  where r.DriverNavigation.Email == driverEmail
                                  select r;
            }


            var ridesDtos = mapper.Map<IEnumerable<RideDto>>(ridesFromDriver.ToList());
            return ridesDtos;
        }
        public async Task<IEnumerable<Ride>> GetAll()
        {
            var result = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    r => r.VehicleNavigation,
                    r => r.FinalLocationNavigation,
                    r => r.DriverNavigation,
                    r => r.Passengers);

            return result.ToList();
        }
        public async Task<Ride> GetById(int id)
        {
            var result = unitOfWork
                .RideRepository
                .GetAllIncluding(
                    "VehicleNavigation",
                    "FinalLocationNavigation",
                    "DriverNavigation",
                    "Passengers.UserNavigation",
                    "Passengers.FinalLocationNavigation");

            var ride = result.FirstOrDefault(x => x.Id == id);
            return ride;
        }
        public async Task<IEnumerable<PassengerDto>> GetPassengers(int rideId)
        {
            var ride = await GetById(rideId);
            if (ride == null)
            {
                throw new Exception("RIDE_NOT_FOUND");
            }
            var passengers = ride.Passengers;
            var dtos = mapper.Map<IEnumerable<PassengerDto>>(passengers);
            return dtos;
        }
        public async Task<IEnumerable<User>> GetUsers(Ride ride)
        {
            if(ride == null) return null;

            List<User> users = new();
            if(ride?.Passengers?.Any() == false)
            {
                ride = await GetById(ride.Id);
            }
                
            users = ride!.Passengers.DistinctBy(x => x.User).Select(x => x.UserNavigation).ToList();
            return users.Append(ride.DriverNavigation);
        }
        public async Task<IEnumerable<UserDto>> GetUsers(RideDto ride)
        {
            if(ride == null) return null;

            var rideEntity = await GetById(ride.Id);

            var users = rideEntity.Passengers.DistinctBy(x => x.User).Select(x => x.UserNavigation).Append(rideEntity.DriverNavigation);
            var dtos = mapper.Map<IEnumerable<UserDto>>(users);
            return dtos;
        }

        public async Task<(IEnumerable<RideDto>, IEnumerable<string>)> CancelInactiveRides()
        {
            var rides = unitOfWork.RideRepository.GetAllIncluding("Passengers");

            //Get rides that were created, but not started, cancelled or ended and are older than 15 minutes
            var ridesToDelete = from r in rides
                                where r.TimeStarted == null &&
                                      r.TimeEnded == null &&
                                      r.TimeCanceled == null &&
                                      r.IsAvailable &&
                                      r.TimeCreated.AddMinutes(15) <= DateTime.Now
                                select r;

            //Cancelling rides 
            List<Ride> ridesCancelled = new();
            List<string> usersToNotify = new();
            foreach (var ride in ridesToDelete)
            {
                try
                {
                    ride.TimeCanceled = DateTime.Now;
                    ride.IsAvailable = false;
                    unitOfWork.RideRepository.Update(ride);

                    //For each passenger, set TimeCanceled 
                    foreach (var passenger in ride.Passengers)
                    {
                        if(passenger.TimeCancelled != null || passenger.TimeIgnored != null || passenger.TimeFinished != null)
                        {
                            continue;
                        }

                        passenger.TimeCancelled = ride.TimeCanceled;
                        unitOfWork.PassengerRepository.Update(passenger);
                    }

                    ridesCancelled.Add(ride);

                    var usersFromRide = await GetUsers(ride);
                    usersToNotify.AddRange(usersFromRide.Select(x => x.Email));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error deleting ride {ride.Id}\n" + ex.Message + "\n" + ex.StackTrace, ride.Id);
                }
            }
            await unitOfWork.SaveChangesAsync();

            return (mapper.Map<IEnumerable<RideDto>>(ridesCancelled), usersToNotify);
        }
    }
}
