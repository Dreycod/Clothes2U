namespace API.DTO.DemandeRestauration
{
    public class DemandeRestaurationDTO
    {
        public int DemandeRestaurationId { get; set; }
        public int UtilisateurId { get; set; }
        public int SuspensionId { get; set; }
        public string? DemandeRestaurationText { get; set; }
    }
}
