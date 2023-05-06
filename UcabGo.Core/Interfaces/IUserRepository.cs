using UcabGo.Core.Entities;

namespace UcabGo.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task<User> Create(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}
