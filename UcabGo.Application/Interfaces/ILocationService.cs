using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Location>> GetAll(string userEmail);
        Task<Location> GetById(int id);
        Task<LocationDto> Create(LocationInput location);
        Task<LocationDto> Update(LocationUpdateInput location);
        Task<LocationDto> Delete(int id, string userEmail);
    }
}
