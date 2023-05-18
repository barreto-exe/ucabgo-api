﻿using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Vehicle> VehicleRepository { get; }
        IRepository<Soscontact> SoscontactRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
