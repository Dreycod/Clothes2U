using Shared.DTO.Abonnement;
using Shared.DTO.Bloque;
using Shared.DTO.Favoris;
using Shared.DTO.Visualisation;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class ActivityMappingProfile : Profile
{
    public ActivityMappingProfile()
    {
        CreateMap<FavorisDTO, Favoris>().ReverseMap();
        CreateMap<Bloque, BloqueDTO>()
            .ForMember(dest => dest.BloqueId, opt => opt.MapFrom(src => src.BloqueId))
            .ForMember(dest => dest.BloqueurId, opt => opt.MapFrom(src => src.UtilisateurBloqueurId))
            .ForMember(dest => dest.UtilisateurBloqueId, opt => opt.MapFrom(src => src.UtilisateurBloqueId))
            .ReverseMap();

        CreateMap<Bloque, BloqueDetailDTO>()
            .ForMember(dest => dest.BloqueId, opt => opt.MapFrom(src => src.BloqueId))
            .ForMember(dest => dest.BloqueurId, opt => opt.MapFrom(src => src.UtilisateurBloqueurId))
            .ForMember(dest => dest.UtilisateurBloqueId, opt => opt.MapFrom(src => src.UtilisateurBloqueId))
            .ForMember(dest => dest.BloqueurLogin, opt => opt.MapFrom(src => src.UtilisateurBloqueur.Login))
            .ForMember(dest => dest.UtilisateurBloqueLogin, opt => opt.MapFrom(src => src.UtilisateurBloque.Login))
            .ReverseMap();
        
         CreateMap<Abonnement, AbonnementDTO>()
            .ForMember(dest => dest.AbonnementId, opt => opt.MapFrom(src => src.AbonnementId))
            .ForMember(dest => dest.UtilisateurSuiveurId, opt => opt.MapFrom(src => src.UtilisateurSuiveurId))
            .ForMember(dest => dest.UtilisateurSuiviId, opt => opt.MapFrom(src => src.UtilisateurSuivisId))
            .ReverseMap();

        CreateMap<Abonnement, AbonnementDetailDTO>()
            .ForMember(dest => dest.AbonnementID, opt => opt.MapFrom(src => src.AbonnementId))
            .ForMember(dest => dest.UtilisateurSuiveurID, opt => opt.MapFrom(src => src.UtilisateurSuiveurId))
            .ForMember(dest => dest.LoginUtilisateurSuiveur, opt => opt.MapFrom(src => src.UtilisateurSuiveur.Login))
            .ForMember(dest => dest.UtilisateurSuiviID, opt => opt.MapFrom(src => src.UtilisateurSuivisId))
            .ForMember(dest => dest.LoginUtilisateurSuivi, opt => opt.MapFrom(src => src.UtilisateurSuivis.Login))
            .ReverseMap();

        CreateMap<Visualisation, VisualisationDTO>()
            .ReverseMap();

        CreateMap<Visualisation, VisualisationDetailDTO>()
            .ForMember(dest => dest.VisualisationId, opt => opt.MapFrom(src => src.VisualisationId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.LoginUtilisateur, opt => opt.MapFrom(src => src.UtilisateurVisu.Login))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.TitreAnnonce, opt => opt.MapFrom(src => src.AnnonceVisu.Title))
            .ForMember(dest => dest.DateVisualisation, opt => opt.MapFrom(src => src.DateVisualisation))
            .ReverseMap();

        CreateMap<VisualisationCreateDTO, Visualisation>()
            .ForMember(dest => dest.VisualisationId, opt => opt.Ignore())
            .ForMember(dest => dest.DateVisualisation, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}