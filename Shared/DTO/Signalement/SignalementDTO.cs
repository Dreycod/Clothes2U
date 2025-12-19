namespace Shared.DTO.Signalement
{
    public class SignalementDTO
    {
        public int SignalementId { get; set; }
        public DateTime SignalementDate { get; set; }
        public string SignalementMotif { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string LoginUtilisateurSignale { get; set; } = null!;
        public int? PhotoProfilUtilisateurId { get; set; } 
    }
}
