namespace Shared.DTO.Decision_suspension
{
    public class DecisionSuspensionDTO
    {
        public int DecisionSuspensionId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool EstTraitee { get; set; }


        public int UtilisateurId { get; set; }
        public int UtilisateurAdminId { get; set; }
        public int? AnnonceId { get; set; }

        public string Raison { get; set; } = string.Empty;
        public int GetId()
        {
            return DecisionSuspensionId;
        }
    }
}
