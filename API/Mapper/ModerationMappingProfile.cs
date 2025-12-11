using API.DTO.Decision_suspension;
using API.DTO.DemandeRestauration;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class ModerationMappingProfile : Profile
{
    public ModerationMappingProfile()
    {
        CreateMap<DemandeRestauration, DemandeRestaurationDTO>()
            .ForMember(dest => dest.DemandeRestaurationId, opt => opt.MapFrom(src => src.DemandeRestaurationId));

        CreateMap<DemandeRestauration, DemandeRestaurationDetailDTO>()
            .ForMember(dest => dest.DemandeRestaurationId, opt => opt.MapFrom(src => src.DemandeRestaurationId))
            .ForMember(dest => dest.DemandeRestaurationText, opt => opt.MapFrom(src => src.DemandeRestaurationText))

            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.NomPlaignant, opt => opt.MapFrom(src => src.Plaignant.Login))

            .ForMember(dest => dest.SuspensionId, opt => opt.MapFrom(src => src.SuspensionId))
            .ForMember(dest => dest.MotifSuspension, opt => opt.MapFrom(src => src.Suspension.MotifSuspension))
            .ForMember(dest => dest.DateDebutSuspension, opt => opt.MapFrom(src => src.Suspension.DateDebutSuspension))
            .ForMember(dest => dest.DateFinSuspension, opt => opt.MapFrom(src => src.Suspension.DateFinSuspension));


        CreateMap<DemandeRestaurationCreateDTO, DemandeRestauration>();

        CreateMap<Decision_suspension, DecisionSuspensionDTO>()
            .ForMember(dest => dest.DecisionSuspensionId, opt => opt.MapFrom(src => src.Decision_suspensionId))
            .ForMember(dest => dest.DateDebut, opt => opt.MapFrom(src => src.DateDebutSuspension))
            .ForMember(dest => dest.DateFin, opt => opt.MapFrom(src => src.DateFinSuspension))
            .ForMember(dest => dest.EstTraitee, opt => opt.MapFrom(src => src.EstTraitee))
            .ForMember(dest => dest.Raison, opt => opt.MapFrom(src => src.MotifSuspension))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.UtilisateurAdminId, opt => opt.MapFrom(src => src.UtilisateurAdminId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId));

        CreateMap<Decision_suspension, DecisionSuspensionDetailDTO>()
            .ForMember(dest => dest.DecisionSuspensionId, opt => opt.MapFrom(src => src.Decision_suspensionId))
            .ForMember(dest => dest.DateDebut, opt => opt.MapFrom(src => src.DateDebutSuspension))
            .ForMember(dest => dest.DateFin, opt => opt.MapFrom(src => src.DateFinSuspension))
            .ForMember(dest => dest.EstTraitee, opt => opt.MapFrom(src => src.EstTraitee))
            .ForMember(dest => dest.Raison, opt => opt.MapFrom(src => src.MotifSuspension))

            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.UtilisateurNom,
                opt => opt.MapFrom(src => src.UtilisateurSuspendu != null ? src.UtilisateurSuspendu.Login : null))

            .ForMember(dest => dest.UtilisateurAdminId, opt => opt.MapFrom(src => src.UtilisateurAdminId))
            .ForMember(dest => dest.AdminNom,
                opt => opt.MapFrom(src => src.Decisionnaire != null ? src.Decisionnaire.Login : null))

            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.AnnonceTitre,
                opt => opt.MapFrom(src => src.AnnonceSuspendu != null ? src.AnnonceSuspendu.Title : null));

        CreateMap<DecisionSuspensionCreateDTO, Decision_suspension>()
            .ForMember(dest => dest.Decision_suspensionId, opt => opt.Ignore()) // la DB gère l'ID
            .ForMember(dest => dest.DateDebutSuspension, opt => opt.MapFrom(src => src.DateDebut))
            .ForMember(dest => dest.DateFinSuspension, opt => opt.MapFrom(src => src.DateFin))
            .ForMember(dest => dest.MotifSuspension, opt => opt.MapFrom(src => src.Raison))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.UtilisateurAdminId, opt => opt.MapFrom(src => src.UtilisateurAdminId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId));

    }
}