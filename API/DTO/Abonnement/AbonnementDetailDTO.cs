namespace API.DTO.Abonnement
{
    public class AbonnementDetailDTO
    {
        public int AbonnementID { get; set; }

        public int UtilisateurSuiveurID { get; set; }
        public string? LoginUtilisateurSuiveur { get; set; }

        public int UtilisateurSuiviID { get; set; }
        public string? LoginUtilisateurSuivi { get; set; }
    }
}
