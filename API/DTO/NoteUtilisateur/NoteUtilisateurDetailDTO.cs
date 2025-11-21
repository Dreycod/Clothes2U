namespace API.DTO.NoteUtilisateur
{
    public class NoteUtilisateurDetailDTO
    {
        public int NoteUtilisateurId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public DateTime DatePublication { get; set; }

        public int NoteurId { get; set; }
        public string? LoginAuteur { get; set; }

        public int NoteId { get; set; }
        public string? LoginCible { get; set; }
    }
}
