namespace Shared.DTO.Bloque
{
    public class BloqueDetailDTO
    {
        public int BloqueId { get; set; }
        public int BloqueurId { get; set; }
        public string? BloqueurLogin { get; set; }
        public int UtilisateurBloqueId { get; set; }
        public string? UtilisateurBloqueLogin { get; set; }
        public int GetId()
        {
            return BloqueId;
        }
    }
}
