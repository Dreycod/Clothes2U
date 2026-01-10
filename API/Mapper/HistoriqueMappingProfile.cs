using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.Historique;

namespace API.Mapper
{
    public class HistoriqueMappingProfile : Profile
    {
        public HistoriqueMappingProfile() 
        {
            CreateMap<HistoriqueUtilisateur, HistoriqueUtilisateurDetailDTO>()
                .ForMember(dest => dest.HistoriqueUtilisateurId, opt => opt.MapFrom(src => src.HistoriqueUtilisateurId))
                .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
                .ForMember(dest => dest.LoginUtilisateur, opt => opt.MapFrom(src => src.UserHist.Login))
                .ForMember(dest => dest.TypeTransaction, opt => opt.MapFrom(src => src.TypeTransaction))
                .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
                .ForMember(dest => dest.Montant, opt => opt.MapFrom(src => src.Montant))
                .ForMember(dest => dest.DateTransaction, opt => opt.MapFrom(src => src.DateTransaction))
                .ForMember(dest => dest.DateSuppressionCompte, opt => opt.MapFrom(src => src.DateSuppressionCompte))
                .ForMember(dest => dest.AdminId, opt => opt.MapFrom(src => src.AdminId))
                .ForMember(dest => dest.AdminLogin, opt => opt.MapFrom(src => src.Admin.Login))
                .ReverseMap();
        }
    }
}
