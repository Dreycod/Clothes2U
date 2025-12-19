namespace Shared.DTO.DemandeRestauration
{
    public class DemandeRestaurationDetailDTO
    {
        public int DemandeRestaurationId { get; set; }
        public string? DemandeRestaurationText { get; set; }

        public int UtilisateurId { get; set; }
        public string? NomPlaignant { get; set; }

        public int SuspensionId { get; set; }
        public string? MotifSuspension { get; set; }

        public DateTime DateDebutSuspension { get; set; }
        public DateTime DateFinSuspension { get; set; }
    }
}
