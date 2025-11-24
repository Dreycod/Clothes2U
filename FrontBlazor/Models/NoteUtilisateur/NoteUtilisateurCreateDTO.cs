namespace FrontBlazor.Models.NoteUtilisateur
{
    public class NoteUtilisateurCreateDTO
    {
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public int NoteurId { get; set; }
        public int NoteId { get; set; }
    }
}
