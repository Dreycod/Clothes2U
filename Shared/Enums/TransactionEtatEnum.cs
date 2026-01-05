using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum TransactionEtatEnum
    {
        Creee = 0,        // Proposition envoyée
        Acceptee = 1,    // Vendeur accepte
        Refusee = 2,     // Acheteur refuse
        Payee = 3,
        Annulee = 4
    }
}
