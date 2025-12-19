namespace Shared.DTO.Visualisation
{
    public class VisualisationDetailDTO
    {
        public int VisualisationId { get; set; }

        public int UtilisateurId { get; set; }
        public string? LoginUtilisateur { get; set; }

        public int AnnonceId { get; set; }
        public string? TitreAnnonce { get; set; }

        public DateTime DateVisualisation { get; set; }
    }
}
