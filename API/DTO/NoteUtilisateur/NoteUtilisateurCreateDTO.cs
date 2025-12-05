namespace API.DTO.NoteUtilisateur
{
    public class NoteUtilisateurCreateDTO
    {
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public int CibleId { get; set; }    
    }
}