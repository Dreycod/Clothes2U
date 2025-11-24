namespace API.DTO.Signalement
{
    public class SignalementCreateDTO
    {
        public string SignalementMotif { get; set; } = null!;
        public int SignalementTypeId { get; set; }
        public int UtilisateurId { get; set; }
        public int? AnnonceId { get; set; }
        public int? AvisId { get; set; }
        public int? UtilisateurSignaleId { get; set; }
    }
}
