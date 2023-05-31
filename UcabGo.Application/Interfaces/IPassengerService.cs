using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IPassengerService
    {
        Task<Passenger> GetById(int id);
    }
}
