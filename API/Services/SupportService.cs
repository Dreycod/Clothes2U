using API.Models;
using API.Models.EntityFramework;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.SupportTicket;
using Shared.Enums;

namespace API.Services
{
    public class SupportService : ISupportService
    {
        private readonly Clothes2UDbContext _db;
        private readonly IEmailService _emailService;

        public SupportService(Clothes2UDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task CreateTicketAsync(int userId, SupportTicketCreateDTO dto)
        {
            var ticket = new SupportTicket
            {
                UtilisateurId = userId,
                Subject = dto.Subject,
                MessageUtilisateur = dto.Message,
                CreatedAt = DateTime.UtcNow,
                Status = StatutTicketEnum.OPEN
            };

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();
        }

        public async Task<List<SupportTicket>> GetOpenTicketsAsync()
        {
            return await _db.SupportTickets
                .Include(t => t.UtilisateurTicket)
                .Where(t => t.Status == StatutTicketEnum.OPEN)
                .ToListAsync();
        }

        public async Task ReplyAsync(int adminId, SupportTicketReplyDTO dto)
        {
            var ticket = await _db.SupportTickets
                .Include(t => t.UtilisateurTicket)
                .FirstOrDefaultAsync(t => t.SupportTicketId == dto.TicketId);

            if (ticket == null)
                throw new Exception("Ticket introuvable");

            ticket.MessageAdmin = dto.Message;
            ticket.AdminId = adminId;
            ticket.AnsweredAt = DateTime.UtcNow;
            ticket.Status = StatutTicketEnum.ANSWERED;

            await _db.SaveChangesAsync();

            // 📧 ENVOI MAIL SI EMAIL VÉRIFIÉ
            if (ticket.UtilisateurTicket.ValidEmail)
            {
                await _emailService.SendAsync(
                    ticket.UtilisateurTicket.Email,
                    $"Support Clothes2U – {ticket.Subject}",
                    dto.Message
                );
            }
        }
    }
}
