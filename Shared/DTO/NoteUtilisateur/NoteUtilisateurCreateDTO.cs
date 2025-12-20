using Shared.Interfaces;

namespace Shared.DTO.NoteUtilisateur
{
    public class NoteUtilisateurCreateDTO
    {
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public int CibleId { get; set; }    
    }
}