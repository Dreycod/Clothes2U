using Shared.Interfaces;

namespace Shared.DTO.NoteUtilisateur
{
    public class NoteUtilisateurDTO : NoteUtilisateurCreateDTO, IEntity
    {
        public int NoteUtilisateurId { get; set; }
        public DateTime DatePublication { get; set; }
        public string NomAuteur { get; set; }
        public int PhotoProfilAuteurId { get; set; }
        public int NoteurId { get; set; }
        public int NoteId { get; set; }
        public int GetId()
        {
            return NoteUtilisateurId;
        }
    }
}
