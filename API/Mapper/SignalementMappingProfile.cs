using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Signalement;
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
                            ? src.SignalementsAvis.Avis.Auteur.Login
                            : "Inconnu"
            ))
            .ForMember(dest => dest.PhotoProfilUtilisateurId, opt => opt.MapFrom(src =>
                src.SignalementsUtilisateur != null
                    ? src.SignalementsUtilisateur.UtilisateurSignale.PhotoId
                    : src.SignalementsAnnonce != null
                        ? src.SignalementsAnnonce.Annonce.Utilisateur.PhotoId
                        : src.SignalementsAvis != null
                            ? src.SignalementsAvis.Avis.Auteur.PhotoId
                            : null
            ));

        CreateMap<Signalement, SignalementDetailsDTO>()
            .ConvertUsing((src, dest, context) =>
            {
                if (src.SignalementsAnnonce != null)
                {
                    return new SignalementAnnonceDTO
                    {
                        SignalementId = src.SignalementId,
                        SignalementDate = src.SignalementDate,
                        SignalementMotif = src.SignalementMotif,
                        UtilisateurSignaleId = src.SignalementsAnnonce.Annonce.Utilisateur.UtilisateurId,
                        AnnonceSignaleeId = src.SignalementsAnnonce.AnnonceSignaleeId
                    };
                }
                else if (src.SignalementsAvis != null)
                {
                    return new SignalementAvisDTO
                    {
                        SignalementId = src.SignalementId,
                        SignalementDate = src.SignalementDate,
                        SignalementMotif = src.SignalementMotif,
                        UtilisateurSignaleId = src.SignalementsAvis.Avis.Auteur.UtilisateurId,
                        AvisId = src.SignalementsAvis.AvisId
                    };
                }
                else if (src.SignalementsUtilisateur != null)
                {
                    return new SignalementUtilisateurDTO
                    {
                        SignalementId = src.SignalementId,
                        SignalementDate = src.SignalementDate,
                        SignalementMotif = src.SignalementMotif,
                        UtilisateurSignaleId = src.SignalementsUtilisateur.UtilisateurSignale.UtilisateurId
                    };
                }

                return null;
            });
        
        CreateMap<SignalementAnnonceCreateDTO, Signalement>()
            .ForMember(dest => dest.SignalementId, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementMotif, opt => opt.MapFrom(src => src.SignalementMotif))
            .ForMember(dest => dest.SignalementDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.SignalementTypeId, opt => opt.MapFrom(src => src.TypeSignalementId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
            .ForMember(dest => dest.TypeSignalement, opt => opt.Ignore())
            .ForMember(dest => dest.Utilisateur, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAnnonce, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAvis, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsUtilisateur, opt => opt.Ignore());

        CreateMap<SignalementAvisCreateDTO, Signalement>()
            .ForMember(dest => dest.SignalementId, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementMotif, opt => opt.MapFrom(src => src.SignalementMotif))
            .ForMember(dest => dest.SignalementDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.SignalementTypeId, opt => opt.MapFrom(src => src.TypeSignalementId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
            .ForMember(dest => dest.TypeSignalement, opt => opt.Ignore())
            .ForMember(dest => dest.Utilisateur, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAnnonce, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAvis, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsUtilisateur, opt => opt.Ignore());

        CreateMap<SignalementUtilisateurCreateDTO, Signalement>()
            .ForMember(dest => dest.SignalementId, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementMotif, opt => opt.MapFrom(src => src.SignalementMotif))
            .ForMember(dest => dest.SignalementDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.SignalementTypeId, opt => opt.MapFrom(src => src.TypeSignalementId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
            .ForMember(dest => dest.TypeSignalement, opt => opt.Ignore())
            .ForMember(dest => dest.Utilisateur, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAnnonce, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsAvis, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsUtilisateur, opt => opt.Ignore());
               
    }
}