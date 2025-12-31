using Shared.DTO.DemandeRestauration;
using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO;
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
            ))
            .ForMember(dest => dest.Statut, opt => opt.MapFrom(src => 
                    src.DecisionSanction != null ? (bool?)src.DecisionSanction.EstEnCours : null
            ))
            .ForMember(dest => dest.FinSuspension, opt => opt.MapFrom(src => 
                src.DecisionSanction.SanctionSuspension != null ? (DateTime?)src.DecisionSanction.SanctionSuspension.DateFinSuspension : null
                ))
            
            ;
            
        CreateMap<MotInterdit, MotInterditDTO>().ReverseMap();
        
        CreateMap<Decision, DecisionPostDTO>()
            .ConstructUsing((src, context) =>
            {
                var elementDto = context.Mapper.Map<ElementDecisionDTO>(src.ElementDecision);
                
                if (src.DecisionAvertissement != null)
                {
                    return new DecisionAvertissementPostDTO
                    {
                        UtilisateurId = src.UtilisateurId,
                        ElementDecision = elementDto
                    };
                }
                else if (src.DecisionSanction?.SanctionSuspension != null)
                {
                    return new SanctionSuspensionPostDTO
                    {
                        UtilisateurId = src.UtilisateurId,
                        DateFinSuspension = src.DecisionSanction.SanctionSuspension.DateFinSuspension,
                        ElementDecision = elementDto
                    };
                }
                else if (src.DecisionSanction?.SanctionBannissement != null)
                {
                    return new SanctionBannissementPostDTO
                    {
                        UtilisateurId = src.UtilisateurId,
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
        
        
       CreateMap<Decision, DecisionDetailDTO>()
            .ConstructUsing((src, context) =>
            {
                var elementDto = context.Mapper.Map<ElementDecisionDTO>(src.ElementDecision);
                
                if (src.DecisionSanction?.SanctionSuspension != null)
                {
                    return new DecisionSuspensionDetailDTO
                    {
                        DateSanction = src.DecisionDate,
                        DateFinSuspension = src.DecisionSanction.SanctionSuspension.DateFinSuspension,
                        ElementDecision = elementDto
                    };
                }
                else if (src.DecisionSanction?.SanctionBannissement != null)
                {
                    return new DecisionBannissementDetailDTO
                    {
                        DateSanction = src.DecisionDate,
                        ElementDecision = elementDto
                    };
                }
                
                throw new InvalidOperationException("Type de décision sanction inconnu");
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
        CreateMap<Signalement, ActivitySignalement>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.SignalementDate))
            .ForMember(dest => dest.LoginUser, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.SignalementId, opt => opt.MapFrom(src => src.SignalementId))
            .ForMember(dest => dest.UtiliseurSignaleLogin, opt => opt.MapFrom(src =>
                src.SignalementsAnnonce != null ? src.SignalementsAnnonce.Annonce.Utilisateur.Login :
                src.SignalementsMessage != null ? src.SignalementsMessage.Message.Utilisateur.Login :
                src.SignalementsAvis != null ? src.SignalementsAvis.Avis.Auteur.Login :
                src.SignalementsUtilisateur != null ? src.SignalementsUtilisateur.UtilisateurSignale.Login : null
            ));
        CreateMap<DemandeRestauration, ActivityRestauration>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.LoginUser, opt => opt.MapFrom(src => src.Decision.Utilisateur.Login))
            .ForMember(dest => dest.RestaurationId, opt => opt.MapFrom(src => src.DemandeRestaurationId));
    }
}