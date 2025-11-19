namespace FrontBlazor.Models
{
    public class SignUpResponse
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
