namespace API.DTO.Signalement
{
    public class SignalementUtilisateurDTO
    {
        public int SignalementUtilisateurId { get; set; }
        public int UtilisateurSignaleId { get; set; }
        public string Login { get; set; } = null!;
    }
}
