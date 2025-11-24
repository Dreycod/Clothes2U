using FrontBlazor.Models.User;

namespace FrontBlazor.Models.LoginRegister
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
