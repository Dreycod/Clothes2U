namespace API.DTO.Signalement
{
    public class SignalementAvisDTO
    {
        public int SignalementAvisId { get; set; }
        public int AvisId { get; set; }
        public int Note { get; set; }
        public string Commentaire { get; set; } = null!;
        public string Auteur { get; set; } = null!;
    }
}
