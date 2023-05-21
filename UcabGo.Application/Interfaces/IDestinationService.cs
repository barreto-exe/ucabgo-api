using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Core.Data.Destinations.Dtos;
using UcabGo.Core.Data.Destinations.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Destination>> GetAll(string userEmail);
        Task<Destination> GetById(int id);
        Task<DestinationDto> Create(DestinationInput input);
        Task<DestinationDto> Update(DestinationUpdateInput input);
        Task<DestinationDto> Delete(string userEmail, int id);
    }
}
