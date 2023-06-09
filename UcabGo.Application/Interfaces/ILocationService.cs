﻿using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface ILocationService
    {
        Task<LocationDto> GetHome(string userEmail);
        Task<IEnumerable<LocationDto>> GetDefaultLocations(string userEmail);
        Task<IEnumerable<LocationDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Location>> GetAll(string userEmail);
        Task<Location> GetById(int id);
        Task<LocationDto> Create(LocationInput location, bool isRegistering = false);
        Task<LocationDto> Delete(int id, string userEmail);
    }
}
