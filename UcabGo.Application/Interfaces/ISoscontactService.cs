﻿using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface ISoscontactService
    {
        Task<IEnumerable<SoscontactDto>> GetAllDtos(string userEmail);
        Task<IEnumerable<Soscontact>> GetAll(string userEmail);
        Task<Soscontact> GetById(int id);
        Task<SoscontactDto> Create(SoscontactInput sosContact);
        Task<SoscontactDto> Update(SoscontactUpdateInput sosContact);
        Task<SoscontactDto> Delete(string userEmail, int id);
    }
}
