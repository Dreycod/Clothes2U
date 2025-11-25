namespace API.DTO.Decision_suspension
{
    public class DecisionSuspensionSearchRequestDTO
    {
        public int? UtilisateurId { get; set; }
        public int? UtilisateurAdminId { get; set; }
        public int? AnnonceId { get; set; }

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }
}
