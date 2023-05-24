using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Destination.Inputs;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Destination>> GetAll(string userEmail);
        Task<Destination> GetById(int id);
        Task<DestinationDto> Create(DestinationInput input, bool isRegistering = false);
        Task<DestinationDto> Update(DestinationUpdateInput input);
        Task<DestinationDto> Delete(string userEmail, int id);
    }
}
