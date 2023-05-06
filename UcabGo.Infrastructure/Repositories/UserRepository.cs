using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;

namespace UcabGo.Core.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UcabgoContext context;
        public UserRepository(UcabgoContext context)
        {
            this.context = context;
        }

        public async Task<User> Create(User user)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
