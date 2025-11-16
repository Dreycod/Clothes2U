namespace FrontBlazor.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public UserDetails UserDetails { get; set; }
    }

    public class UserDetails
    {
        public int UtilisateurId { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public DateTime Dateinscription { get; set; }
        public string? Description { get; set; }
        public int? StatutId { get; set; }
        public int? AdresseId { get; set; }
    }

    public class SignUpResponse
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
