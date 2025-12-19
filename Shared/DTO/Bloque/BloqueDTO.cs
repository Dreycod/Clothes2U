namespace Shared.DTO.Bloque
{
    public class BloqueDTO
    {
        public int BloqueId { get; set; }
        public int BloqueurId { get; set; }
        public int UtilisateurBloqueId { get; set; }
        public int GetId()
        {
            return BloqueId;
        }
    }
}
