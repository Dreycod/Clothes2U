using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.DemandeRestauration;

namespace API.Mapper;

public class DemandeRestaurationMappingProfile : Profile
{
    public DemandeRestaurationMappingProfile()
    {
        CreateMap<DemandeRestauration, DemandeRestaurationDTO>()
            .ForMember(dest => dest.DemandeRestaurationId, opt => opt.MapFrom(src => src.DemandeRestaurationId))
            .ForMember(dest => dest.LoginUtilisateur, opt => opt.MapFrom(src => src.Decision.Utilisateur.Login))
            .ReverseMap();
        CreateMap<DemandeRestauration, DemandeRestaurationDetailDTO>()
            .ForMember(dest => dest.DecisionId, opt => opt.MapFrom(src => src.DecisionId))
            .ForMember(dest => dest.DemandeRestaurationId, opt => opt.MapFrom(src => src.DemandeRestaurationId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.Decision.UtilisateurId))
            .ReverseMap();
    }
    
}