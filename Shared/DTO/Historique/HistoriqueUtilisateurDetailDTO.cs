using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Historique
{
    public class HistoriqueUtilisateurDetailDTO
    {
        public int HistoriqueUtilisateurId { get; set; }
        public int UtilisateurId { get; set; }
        public string? LoginUtilisateur { get; set; }
        public string? TypeTransaction { get; set; }
        public int? AnnonceId { get; set; }
        public decimal? Montant { get; set; }
        public DateTime? DateTransaction { get; set; }
        public DateTime DateSuppressionCompte { get; set; }
        public int? AdminId { get; set; }
        public string? AdminLogin { get; set; }
    }
}
