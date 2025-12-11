namespace API.DTO.Recense
{
    public class RecenseDetailDTO
    {
        public int RecenseId { get; set; }

        public int AnnonceId { get; set; }
        public string? AnnonceTitre { get; set; }

        public int TagId { get; set; }
        public string? LibelleTag { get; set; }
    }
}
