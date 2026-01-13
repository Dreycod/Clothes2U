using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.SupportTicket
{
    public class TicketDetailViewDTO
    {
        
        public int Status { get; set; }
        public int TicketId { get; set; }
        public int UtilisateurId { get; set; }
        public string Subject { get; set; }
        public DateTime DateCreation { get; set; }
        public List<TicketMessageViewDTO>  Messages { get; set; }
        public string UtilisateurLogin { get; set; }
    }

    public class TicketMessageViewDTO
    {
        public int TicketMessageId { get; set; }
        public string Content { get; set; }
        public DateTime DateCreation { get; set; }
        public string UtilisateurLogin { get; set; }
        public int UtilisateurId { get; set; }
    }
}
