using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Transaction
{
    public class CreateTransactionDTO
    {
        public int ConversationId { get; set; }
        public int Montant { get; set; }
    }
}
