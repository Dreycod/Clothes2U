namespace API.DTO.Signalement
{
    public class SignalementAnnonceDTO
    {
        public int SignalementAnnonceId { get; set; }
        public int AnnonceId { get; set; }
        public string Titre { get; set; } = null!;
        public decimal Prix { get; set; }
    }
}
