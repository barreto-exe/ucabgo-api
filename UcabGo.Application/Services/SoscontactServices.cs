using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class SoscontactServices : ISoscontactServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public SoscontactServices(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Soscontact>> GetAll(string userEmail)
        {
            var list = unitOfWork.SoscontactRepository.GetAll();
            var users = unitOfWork.UserRepository.GetAll();

            var result =
                from v in list
                join u in users on v.User equals u.Id
                where v.UserNavigation.Email == userEmail
                select v;

            return result.ToList();
        }
        public async Task<Soscontact> GetById(int id)
        {
            return await unitOfWork.SoscontactRepository.GetById(id);
        }
        public async Task<SoscontactDto> Create(SoscontactInput sosContact)
        {
            var item = mapper.Map<Soscontact>(sosContact);

            int idUser = (await userService.GetByEmail(sosContact.Email)).Id;
            item.User = idUser;
            await unitOfWork.SoscontactRepository.Add(item);
            await unitOfWork.SaveChangesAsync();

            var itemDb = await GetById(item.Id);
            var dto = mapper.Map<SoscontactDto>(itemDb);
            return dto;
        }
        public async Task<SoscontactDto> Update(SoscontactUpdateInput sosContact)
        {
            var itemDb = await GetById(sosContact.Id);

            itemDb.Name = sosContact.Name;
            itemDb.Phone = sosContact.Phone;

            unitOfWork.SoscontactRepository.Update(itemDb);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<SoscontactDto>(itemDb);
            return dto;
        }
        public async Task<SoscontactDto> Delete(string userEmail, int id)
        {
            var items = await GetAll(userEmail);
            var itemDb = items.FirstOrDefault(x => x.Id == id);

            if (itemDb == null)
            {
                throw new Exception("SOSCONTACT_NOT_FOUND");
            }

            await unitOfWork.SoscontactRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<SoscontactDto>(itemDb);
            return dto;
        }
    }
}
