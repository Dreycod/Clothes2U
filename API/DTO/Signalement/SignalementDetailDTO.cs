namespace API.DTO.Signalement
{
    public class SignalementDetailDTO
    {
        public int SignalementId { get; set; }
        public DateTime SignalementDate { get; set; }
        public string SignalementMotif { get; set; } = null!;
        public string Type { get; set; } = null!;

        public int UtilisateurId { get; set; }
        public string LoginAuteur { get; set; } = null!;

        public SignalementAnnonceDTO? Annonce { get; set; }
        public SignalementAvisDTO? Avis { get; set; }
        public SignalementUtilisateurDTO? Utilisateur { get; set; }
    }
}
