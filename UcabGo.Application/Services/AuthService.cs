using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }


        /// <exception cref="UserExistsException"></exception>
        public async Task<LoginDto> Register(RegisterInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginDto> Login(LoginInput input)
        {
            throw new NotImplementedException();
        }

        private static JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
