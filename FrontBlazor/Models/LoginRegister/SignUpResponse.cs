using FrontBlazor.Models.User;

namespace FrontBlazor.Models.LoginRegister
{
    public class SignUpResponse
    {
        public string Token { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
