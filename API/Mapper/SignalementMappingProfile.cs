using API.DTO.NoteUtilisateur;
using API.DTO.Signalement;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class SignalementMappingProfile : Profile
{
    public SignalementMappingProfile()
    {
        
        CreateMap<NoteUtilisateurCreateDTO, NoteUtilisateur>()
            .ForMember(dest => dest.CibleId, opt => opt.MapFrom(src => src.CibleId))
            .ForMember(dest => dest.DatePublication, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.NoteUtilisateurId, opt => opt.Ignore());

        CreateMap<Signalement, SignalementDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeSignalement.SignalementTypeLibelle))
            .ForMember(dest => dest.LoginUtilisateurSignale, opt => opt.MapFrom(src => 
                src.SignalementsUtilisateur != null 
                    ? src.SignalementsUtilisateur.UtilisateurSignale.Login
                    : src.SignalementsAnnonce != null 
                        ? src.SignalementsAnnonce.Annonce.Utilisateur.Login
                        : src.SignalementsAvis != null 
                            ? src.SignalementsAvis.Avis.Cible.Login
                            : "Inconnu"
            ))
            .ForMember(dest => dest.PhotoProfilUtilisateurId, opt => opt.MapFrom(src => 
                src.SignalementsUtilisateur != null 
                    ? src.SignalementsUtilisateur.UtilisateurSignale.PhotoId
                    : src.SignalementsAnnonce != null 
                        ? src.SignalementsAnnonce.Annonce.Utilisateur.PhotoId
                        : src.SignalementsAvis != null 
                            ? src.SignalementsAvis.Avis.Cible.PhotoId
                            : null
            ));
        CreateMap<Signalement, SignalementDetailDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeSignalement.SignalementTypeLibelle))
            .ForMember(dest => dest.LoginAuteur, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.Annonce, opt => opt.MapFrom(src => src.SignalementsAnnonce))
            .ForMember(dest => dest.Avis, opt => opt.MapFrom(src => src.SignalementsAvis))
            .ForMember(dest => dest.Utilisateur, opt => opt.MapFrom(src => src.SignalementsUtilisateur));

        CreateMap<SignalementCreateDTO, Signalement>()
            .ForMember(dest => dest.SignalementDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<SignalementAnnonce, SignalementAnnonceDTO>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceSignaleeId))
            .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Annonce.Title))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Annonce.Prix));

        CreateMap<SignalementAvis, SignalementAvisDTO>()
            .ForMember(dest => dest.AvisId, opt => opt.MapFrom(src => src.AvisId))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Avis.Note))
            .ForMember(dest => dest.Commentaire, opt => opt.MapFrom(src => src.Avis.Commentaire))
            .ForMember(dest => dest.Auteur, opt => opt.MapFrom(src => src.Avis.Auteur.Login));

        CreateMap<SignalementUtilisateur, SignalementUtilisateurDTO>()
            .ForMember(dest => dest.UtilisateurSignaleId, opt => opt.MapFrom(src => src.UtilisateurSignaleId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.UtilisateurSignale.Login));


    }
}