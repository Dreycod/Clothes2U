using Shared.DTO.SupportTicket;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper
{
    public class SupportServiceMappingProfile : Profile
    {
        public SupportServiceMappingProfile() 
        {
            CreateMap<SupportTicket, SupportTicketViewDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SupportTicketId))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dest => dest.MessageUtilisateur, opt => opt.MapFrom(src => src.MessageUtilisateur))
                .ForMember(dest => dest.MessageAdmin, opt => opt.MapFrom(src => src.MessageAdmin))
                .ForMember(dest => dest.EmailUtilisateur, opt => opt.MapFrom(src => src.UtilisateurTicket.Email))
                .ForMember(dest => dest.EmailVerifie, opt => opt.MapFrom(src => src.UtilisateurTicket.ValidEmail))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
