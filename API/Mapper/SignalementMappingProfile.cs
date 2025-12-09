using API.DTO.NoteUtilisateur;
using API.DTO.Signalement;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class SignalementMappingProfile : Profile
{
    public SignalementMappingProfile()
    {
        CreateMap<NoteUtilisateur, NoteUtilisateurDTO>()
            .ForMember(dest => dest.NoteurId, opt => opt.MapFrom(src => src.AuteurId))
            .ForMember(dest => dest.NomAuteur, opt => opt.MapFrom(src => src.Auteur.Login))
            .ForMember(dest => dest.PhotoProfilAuteurId, opt => opt.MapFrom(src => src.Auteur.PhotoId))
            .ForMember(dest => dest.NoteId, opt => opt.MapFrom(src => src.CibleId));

        CreateMap<NoteUtilisateur, NoteUtilisateurDetailDTO>()
            .ForMember(dest => dest.LoginAuteur, opt => opt.MapFrom(src => src.Auteur.Login))
            .ForMember(dest => dest.LoginCible, opt => opt.MapFrom(src => src.Cible.Login));

        CreateMap<NoteUtilisateurCreateDTO, NoteUtilisateur>()
            .ForMember(dest => dest.CibleId, opt => opt.MapFrom(src => src.CibleId))
            .ForMember(dest => dest.DatePublication, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.NoteUtilisateurId, opt => opt.Ignore());

        CreateMap<Signalement, SignalementDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeSignalement.SignalementTypeLibelle))
            .ForMember(dest => dest.LoginAuteur, opt => opt.MapFrom(src => src.Utilisateur.Login));

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