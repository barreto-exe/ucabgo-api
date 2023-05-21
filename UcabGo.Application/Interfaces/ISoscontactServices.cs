using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface ISoscontactServices
    {
        Task<IEnumerable<Soscontact>> GetAll(string userEmail);
        Task<Soscontact> GetById(int id);
        Task<SoscontactDto> Create(SoscontactInput sosContact);
        Task<SoscontactDto> Update(SoscontactUpdateInput sosContact);
        Task<SoscontactDto> Delete(string userEmail, int id);
    }
}
