namespace API.DTO.Decision_suspension
{
    public class DecisionSuspensionDetailDTO
    {
        public int DecisionSuspensionId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public int UtilisateurId { get; set; }
        public int UtilisateurAdminId { get; set; }
        public int? AnnonceId { get; set; }

        public string Raison { get; set; } = string.Empty;

        // Relations (optionnel, mais utile)
        public string? UtilisateurNom { get; set; }
        public string? AdminNom { get; set; }
        public string? AnnonceTitre { get; set; }
    }
}
