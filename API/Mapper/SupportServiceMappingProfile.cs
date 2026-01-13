using Shared.DTO.SupportTicket;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper
{
    public class SupportServiceMappingProfile : Profile
    {
        public SupportServiceMappingProfile()
        {
            CreateMap<Ticket, SupportTicketViewDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TicketSubject))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.DateLastMessage, opt => opt.MapFrom(src => 
                    src.Messages.Any() ? src.Messages.Max(m => m.DateEnvoi) : src.DateCreation))
                .ForMember(dest => dest.LoginUser, opt => opt.MapFrom(src => src.Utilisateur.Login ));

            CreateMap<Ticket, TicketDetailViewDTO>()
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.TicketSubject))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => src.DateCreation))
                .ForMember(dest => dest.UtilisateurLogin, opt => opt.MapFrom(src => src.Utilisateur != null ? src.Utilisateur.Login : string.Empty))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));
            CreateMap<TicketMessage, TicketMessageViewDTO>()
                .ForMember(dest => dest.TicketMessageId, opt => opt.MapFrom(src => src.TicketMessageId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => src.DateEnvoi))
                .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
                .ForMember(dest => dest.UtilisateurLogin, opt => opt.MapFrom(src => src.Utilisateur != null ? src.Utilisateur.Login : string.Empty));

            
            
            
        }
    }
}
