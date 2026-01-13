using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Mail;
using Shared.DTO.SupportTicket;
using Shared.Enums;

namespace API.Services
{
    public class SupportService : ISupportService
    {
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketManager;
        private readonly IDataRepository<TicketMessage, int> _ticketMessageManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationMailService _emailService;
        private readonly IUtilisateurRepository _utilisateurManager;

        public SupportService(
            IMapper mapper,
            ITicketRepository ticketManager,
            IDataRepository<TicketMessage, int> ticketMessageManager,
            IUtilisateurRepository utilisateurManager,
            INotificationMailService emailService,
            ICurrentUserService currentUserService)
        {
            _mapper = mapper;
            _ticketManager = ticketManager;
            _ticketMessageManager = ticketMessageManager;
            _utilisateurManager = utilisateurManager;
            _emailService = emailService;
            _currentUserService = currentUserService;
        }

        public async Task CreateTicket(SupportTicketCreateDTO supportTicketCreateDTO)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            Ticket ticketToInsert = new Ticket()
            {
                TicketSubject = supportTicketCreateDTO.Subject,
                UtilisateurId = userId,
                Status = (int)StatusTicketEnum.OPEN,
                DateCreation = DateTime.UtcNow
            };
            await _ticketManager.AddAsync(ticketToInsert);
            TicketMessage messageToInsert = new TicketMessage()
            {
                DateEnvoi = DateTime.UtcNow,
                UtilisateurId = userId,
                Content = supportTicketCreateDTO.Message,
                TicketId = ticketToInsert.TicketId
            };
            await _ticketMessageManager.AddAsync(messageToInsert);
        }

        public async Task Reply(SupportTicketReplyDTO supportTicketReplyDTO)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            Ticket ticket = await _ticketManager.GetByIdAsync(supportTicketReplyDTO.TicketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Le ticket avec l'ID {supportTicketReplyDTO.TicketId} n'existe pas.");
            }
            if (ticket.Status == (int)StatusTicketEnum.CLOSED)
            {
                throw new InvalidOperationException("Impossible de répondre à un ticket clôturé.");
            }
            
            Utilisateur utilisateur = await _utilisateurManager.GetByIdAsync(ticket.UtilisateurId);
            if (utilisateur == null)
            {
                throw new KeyNotFoundException($"L'utilisateur avec l'ID {ticket.UtilisateurId} n'existe pas.");
            }
            
            TicketMessage messageToInsert = new TicketMessage()
            {
                DateEnvoi = DateTime.UtcNow,
                UtilisateurId = userId,
                Content = supportTicketReplyDTO.Message,
                TicketId = supportTicketReplyDTO.TicketId
            };
            await _ticketMessageManager.AddAsync(messageToInsert);
            
            MailDTO mail = new MailDTO()
            {
                MailObject = ticket.TicketSubject,
                MailContent = supportTicketReplyDTO.Message,
            };
            
            // Passer le nom de l'utilisateur et l'ID du ticket
            await _emailService.SendSupportMailAsync(
                utilisateur.Email, 
                utilisateur.Login, 
                ticket.TicketId, 
                mail
            );
            
            ticket.Status = (int)StatusTicketEnum.ANSWERED;
            await _ticketManager.UpdateAsync(ticket);
        }
    }
}