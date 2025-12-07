namespace FrontBlazor.Models
{
    public class Abonnement : IEntity
    {
        public int AbonnementId { get; set; }
        public int UtilisateurSuiveurId { get; set; }
        public int UtilisateurSuiviId { get; set; }

        public int GetId()
        {
            return AbonnementId;
        }
    }
}
