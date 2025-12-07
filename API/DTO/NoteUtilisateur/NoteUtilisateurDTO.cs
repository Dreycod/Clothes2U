namespace API.DTO.NoteUtilisateur
{
    public class NoteUtilisateurDTO
    {
        public int NoteUtilisateurId { get; set; }
        public int Note { get; set; }
        public string Commentaire { get; set; }
        public DateTime DatePublication { get; set; }
        public string NomAuteur { get; set; }
        public int PhotoProfilAuteurId { get; set; }
        public int NoteurId { get; set; }
        public int NoteId { get; set; }
    }
}
