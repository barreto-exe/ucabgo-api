using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public VehicleService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllDtos(string userEmail)
        {
            var items = await GetAll(userEmail);
            var itemsDtos = items.Select(v => new VehicleDto
            {
                Id = v.Id,
                Brand = v.Brand,
                Model = v.Model,
                Plate = v.Plate,
                Color = v.Color,
                Owner = mapper.Map<UserDto>(v.UserNavigation),
            });

            return itemsDtos;
        }
        public async Task<IEnumerable<Vehicle>> GetAll(string userEmail)
        {
            var vehicles = unitOfWork.VehicleRepository.GetAllIncluding(x => x.UserNavigation);
            var users = unitOfWork.UserRepository.GetAll();

            var result = 
                from v in vehicles
                join u in users on v.User equals u.Id
                where v.UserNavigation.Email == userEmail
                select v;

            return result.ToList();
        }
        public async Task<Vehicle> GetById(int id)
        {
            var vehicle = await unitOfWork.VehicleRepository.GetById(id);
            return vehicle;
        }
        public async Task<VehicleDto> Create(VehicleInput vehicleInput)
        {
            var vehicle = mapper.Map<Vehicle>(vehicleInput);
            
            int idUser = (await userService.GetByEmail(vehicleInput.Email)).Id;
            vehicle.User = idUser;
            await unitOfWork.VehicleRepository.Add(vehicle);
            await unitOfWork.SaveChangesAsync();

            var vehicleDb = await GetById(vehicle.Id);
            var dto = mapper.Map<VehicleDto>(vehicleDb);
            dto.Owner = mapper.Map<UserDto>(vehicleDb.UserNavigation);
            return dto;
        }
        public async Task<VehicleDto> Update(VehicleUpdateInput vehicle)
        {
            var vehicleDb = await GetById(vehicle.Id);

            if(vehicleDb == null)
            {
                throw new Exception("VEHICLE_NOT_FOUND");
            }

            vehicleDb.Brand = vehicle.Brand;
            vehicleDb.Model = vehicle.Model;
            vehicleDb.Plate = vehicle.Plate;
            vehicleDb.Color = vehicle.Color;

            unitOfWork.VehicleRepository.Update(vehicleDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<VehicleDto>(vehicleDb);
            return dto;
        }
        public async Task<VehicleDto> Delete(string userEmail, int id)
        {
            var usersVehicles = await GetAll(userEmail);
            var vehicle = usersVehicles.FirstOrDefault(v => v.Id == id);

            if(vehicle == null)
            {
                throw new Exception("VEHICLE_NOT_FOUND");
            }

            await unitOfWork.VehicleRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<VehicleDto>(vehicle);
            return dto;
        }
    }
}
