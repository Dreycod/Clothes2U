using API.DTO;
using API.DTO.DemandeRestauration;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class ModerationMappingProfile : Profile
{
    public ModerationMappingProfile()
    {   
        CreateMap<Decision, DecisionDTO>()
            .ForMember(dest => dest.DateDecision, opt => opt.MapFrom(src => src.DecisionDate))
            .ForMember(dest => dest.LoginUtilisateur, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.TypeDecision, opt => opt.MapFrom(src => 
                src.DecisionAvertissement != null ? "Avertissement" :
                src.DecisionSanction != null && src.DecisionSanction.SanctionBannissement != null ? "Bannissement" :
                src.DecisionSanction != null && src.DecisionSanction.SanctionSuspension != null ? "Suspension" :
                "Inconnu"
            ));
    }
}