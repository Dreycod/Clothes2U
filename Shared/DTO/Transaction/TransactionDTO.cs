using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Transaction
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public double Montant { get; set; }
        public int TransactionEtat { get; set; }
    }
}
