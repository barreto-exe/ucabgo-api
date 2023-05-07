using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Auth.Dto
{
    public class LoginDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
