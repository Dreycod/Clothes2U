namespace API.DTO.DemandeRestauration
{
    public class DemandeRestaurationCreateDTO
    {
        public int UtilisateurId { get; set; }
        public int SuspensionId { get; set; }
        public string DemandeRestaurationText { get; set; } = string.Empty;
    }
}
