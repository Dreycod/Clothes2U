using Shared.DTO.DemandeRestauration;
using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.Decision;
using Shared.DTO.MotInterdit;

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
        CreateMap<MotInterdit, MotInterditDTO>().ReverseMap();
        CreateMap<Decision, DecisionPostDTO>()
            .ForMember(dest => dest.UtlisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.DateDecision, opt => opt.MapFrom(src => src.DecisionDate))
            .ConstructUsing((src, context) =>
            {
                if (src.DecisionAvertissement != null)
                {
                    return new DecisionAvertissementPostDTO
                    {
                        UtlisateurId = src.UtilisateurId,
                        DateDecision = src.DecisionDate
                    };
                }
                else if (src.DecisionSanction?.SanctionSuspension != null)
                {
                    var elementDto = context.Mapper.Map<ElementDecisionDTO>(src.DecisionSanction.ElementDecision);
                    return new SanctionSuspensionPostDTO
                    {
                        UtlisateurId = src.UtilisateurId,
                        DateDecision = src.DecisionDate,
                        DateFinSuspension = src.DecisionSanction.SanctionSuspension.DateFinSuspension,
                        ElementDecision = elementDto
                    };
                }
                else if (src.DecisionSanction?.SanctionBannissement != null)
                {
                    var elementDto = context.Mapper.Map<ElementDecisionDTO>(src.DecisionSanction.ElementDecision);
                    return new SanctionBannissementPostDTO
                    {
                        UtlisateurId = src.UtilisateurId,
                        DateDecision = src.DecisionDate,
                        ElementDecision = elementDto
                    };
                }
                
                throw new InvalidOperationException("Type de décision inconnu");
            });
        CreateMap<ElementDecision, ElementDecisionDTO>()
            .ConstructUsing((src, context) =>
            {
                if (src.ElementDecisionAnnonce != null)
                {
                    return new ElementDecisionAnnonceDTO
                    {
                        AnnonceId = src.ElementDecisionAnnonce.AnnonceId
                    };
                }
                else if (src.ElementDecisionMessage != null)
                {
                    return new ElementDecisionMessageDTO
                    {
                        MessageId = src.ElementDecisionMessage.MessageId
                    };
                }
                else if (src.ElementDecisionAvis != null)
                {
                    return new ElementAvisDTO
                    {
                        AvisId = src.ElementDecisionAvis.AvisId
                    };
                }
                else if (src.ElementDecisionUtilisateur != null)
                {
                    return new ElementUtilisateurDTO();
                }
                
                throw new InvalidOperationException("Type d'élément de décision inconnu");
            });
    }
}
