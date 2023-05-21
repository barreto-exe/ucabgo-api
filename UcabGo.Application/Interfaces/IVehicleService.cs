using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Vehicle>> GetAll(string userEmail);
        Task<Vehicle> GetById(int id);
        Task<VehicleDto> Create(VehicleInput vehicle);
        Task<VehicleDto> Update(VehicleUpdateInput vehicle);
        Task<VehicleDto> Delete(string userEmail, int id);
    }
}
