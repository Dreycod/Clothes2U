using FrontBlazor.Models;

namespace FrontBlazor.Models;

public class SignUpResponse
{
    public string Token { get; set; }
    public UserDetails UserDetails { get; set; }
}
