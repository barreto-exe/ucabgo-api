namespace UcabGo.Core.Data.Auth.Inputs
{
    public class LoginInput : BaseRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
