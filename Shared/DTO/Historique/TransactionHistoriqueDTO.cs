using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Historique
{
    public class TransactionHistoriqueDTO
    {
        public int TransactionId { get; set; }
        public double Montant { get; set; }
        public string? TypeTransaction { get; set; }
        public DateTime DateDebutNegociation { get; set; }
        public int UtilisateurId { get; set; }
    }
}
