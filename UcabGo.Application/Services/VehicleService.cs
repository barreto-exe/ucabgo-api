using AutoMapper;
using Microsoft.IdentityModel.Tokens;
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
            var itemsDtos = mapper.Map<IEnumerable<VehicleDto>>(items);
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

            // Clear whitespaces
            vehicle.Plate = vehicle.Plate.Replace(" ", "");
            vehicle.Model = vehicle.Model.Trim();
            vehicle.Brand = vehicle.Brand.Trim();
            vehicle.Color = vehicle.Color.Trim();


            if (vehicle.Brand.IsNullOrEmpty() || vehicle.Model.IsNullOrEmpty() || vehicle.Plate.IsNullOrEmpty() || vehicle.Color.IsNullOrEmpty())
            {
                throw new Exception("VEHICLE_NULL_FIELD");
            }

            // Venezuelan plates have a maximum of 7 characters
            if(vehicle.Plate.Length > 7)
            {
                throw new Exception("VEHICLE_INVALID_PLATE");
            }

            if (vehicle.Model.Length > 32 || vehicle.Color.Length > 20 || vehicle.Brand.Length > 32)
            {
                throw new Exception("VEHICLE_FIELD_LENGTH");
            }

            var vehicles = unitOfWork.VehicleRepository.GetAll();
            foreach (var v in vehicles)
            {
                if (v.Plate == vehicle.Plate)
                {
                    throw new Exception("VEHICLE_PLATE_REPEATED");
                }
            }

            User user = await userService.GetByEmail(vehicleInput.Email);
            vehicle.User = user.Id;
            await unitOfWork.VehicleRepository.Add(vehicle);
            await unitOfWork.SaveChangesAsync();

            var vehicleDb = await GetById(vehicle.Id);
            var dto = mapper.Map<VehicleDto>(vehicleDb);
            dto.Owner = mapper.Map<UserDto>(vehicleDb.UserNavigation);
            return dto;
        }
        public async Task<VehicleDto> Update(VehicleUpdateInput vehicle)
        {
            // Clear whitespaces
            vehicle.Plate = vehicle.Plate.Replace(" ", "");
            vehicle.Model = vehicle.Model.Trim();
            vehicle.Brand = vehicle.Brand.Trim();
            vehicle.Color = vehicle.Color.Trim();

            if (vehicle.Brand.IsNullOrEmpty() || vehicle.Model.IsNullOrEmpty() || vehicle.Plate.IsNullOrEmpty() || vehicle.Color.IsNullOrEmpty())
            {
                throw new Exception("VEHICLE_NULL_FIELD");
            }

            if (vehicle.Plate.Length > 7)
            {
                throw new Exception("VEHICLE_INVALID_PLATE");
            }

            if (vehicle.Model.Length > 32 || vehicle.Color.Length > 32 || vehicle.Brand.Length > 32)
            {
                throw new Exception("VEHICLE_FIELD_LENGTH");
            }

            var vehicleDb = await GetById(vehicle.Id);
            if (vehicleDb == null)
            {
                throw new Exception("VEHICLE_NOT_FOUND");
            }

            var vehicles = unitOfWork.VehicleRepository.GetAll();

            foreach (var v in vehicles)
            {
                if (v.Plate == vehicle.Plate && v.Id != vehicle.Id)
                {
                        throw new Exception("VEHICLE_PLATE_REPEATED");
                }
            }

            // This operation wasn't working since an empty string is not a nullable value

            /* vehicleDb.Brand = vehicle.Brand ?? vehicleDb.Brand;
               vehicleDb.Model = vehicle.Model ?? vehicleDb.Model;
               vehicleDb.Plate = vehicle.Plate ?? vehicleDb.Plate;
               vehicleDb.Color = vehicle.Color ?? vehicleDb.Color; */

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

            if (vehicle == null)
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
