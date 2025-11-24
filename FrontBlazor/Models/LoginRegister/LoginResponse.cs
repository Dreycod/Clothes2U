using FrontBlazor.Models;

namespace FrontBlazor.Models;
public class LoginResponse
{
    public string Token { get; set; }
    public UserDetails UserDetails { get; set; }
}

