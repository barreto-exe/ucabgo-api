using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IUnitOfWork unitOfWork;
        public PassengerService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Passenger> GetById(int id)
        {
            return await unitOfWork.PassengerRepository.GetById(id);
        }
    }
}
