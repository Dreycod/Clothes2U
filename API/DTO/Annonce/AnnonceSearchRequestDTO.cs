namespace API.DTO.Annonce
{
    public class AnnonceSearchRequestDTO
    {
        public int? CategorieId { get; set; }
        public int? SousCategorieId { get; set; }
        public int? TailleId { get; set; }
        public int? EtatId { get; set; }
        public int? MarqueId { get; set; }
        public decimal? PrixMin { get; set; }
        public decimal? PrixMax { get; set; }
        public string? MotCle { get; set; }
    }
}
