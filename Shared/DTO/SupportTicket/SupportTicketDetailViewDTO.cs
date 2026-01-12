using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.SupportTicket
{
    public class SupportTicketDetailViewDTO
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string MessageUtilisateur { get; set; }
        public string? MessageAdmin { get; set; }
        public string EmailUtilisateur { get; set; }
        public bool EmailVerifie { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
