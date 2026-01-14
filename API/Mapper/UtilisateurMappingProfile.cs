using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Utilisateur;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class UtilisateurMappingProfile : Profile
{
    public UtilisateurMappingProfile()
    {
        CreateMap<NoteUtilisateur, NoteUtilisateurDTO>()
            .ForMember(dest => dest.NoteurId, opt => opt.MapFrom(src => src.AuteurId))
            .ForMember(dest => dest.NomAuteur, opt => opt.MapFrom(src => src.Auteur.Login))
            .ForMember(dest => dest.PhotoProfilAuteurId, opt => opt.MapFrom(src => src.Auteur.PhotoId))
            .ForMember(dest => dest.NoteId, opt => opt.MapFrom(src => src.CibleId));

        CreateMap<NoteUtilisateur, NoteUtilisateurDetailDTO>()
            .ForMember(dest => dest.LoginAuteur, opt => opt.MapFrom(src => src.Auteur.Login))
            .ForMember(dest => dest.LoginCible, opt => opt.MapFrom(src => src.Cible.Login));

        CreateMap<Utilisateur, UtilisateurCardDTO>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.NomUtilisateur, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(src => src.PhotoId));
        CreateMap<Utilisateur, UtilisateurViewDTO>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.DateInscription, opt => opt.MapFrom(src => src.Dateinscription))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ValidEmail, opt => opt.MapFrom(src => src.ValidEmail))
            .ForMember(dest => dest.ValidTelephone, opt => opt.MapFrom(src => src.ValidTelephone))
            .ForMember(dest => dest.Statut, opt => opt.MapFrom(src => src.Statut.StatutLibelle))
            .ForMember(dest => dest.PhotoProfilId,
                opt => opt.MapFrom(src => src.PhotoProfil != null ? src.PhotoProfil.PhotoId : 0))
            .ForMember(dest => dest.RoleUtilisateur, opt => opt.MapFrom(src => src.Role.RoleUtilisateurLibelle))
            .ForMember(dest => dest.Abonnes, opt => opt.MapFrom(src => src.Abonnes.Count))
            .ForMember(dest => dest.Abonnements, opt => opt.MapFrom(src => src.Abonnements.Count))
            .ForMember(dest => dest.NombreAvis, opt => opt.MapFrom(src => src.NotesCible.Count))
            .ForMember(dest => dest.MoyenneAvis, opt => opt.MapFrom(src =>
                src.NotesCible.Any()
                    ? src.NotesCible.Average(n => (double)n.Note)
                    : 0.0));

        CreateMap<Utilisateur, CurrentUtilisateurDTO>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.Statut, opt => opt.MapFrom(src => src.Statut.StatutLibelle))
            .ForMember(dest => dest.RoleUtilisateur, opt => opt.MapFrom(src => src.Role.RoleUtilisateurLibelle));

        CreateMap<Utilisateur, UtilisateurSettingsDTO>()
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
            .ForMember(dest => dest.PhotoProfilId, opt => opt.MapFrom(src => src.PhotoId))
            .ForMember(dest => dest.PreferenceNotifMail, opt => opt.MapFrom(src => src.PreferenceNotifMail));
                CreateMap<UtilisateurSettingsDTO, Utilisateur>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(src => src.PhotoProfilId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcValue) => srcValue != null));
        CreateMap<UtilisateurPutDTO, Utilisateur>()
            // Ignorer toutes les propriétés de navigation
            .ForMember(dest => dest.Adresses, opt => opt.Ignore())
            .ForMember(dest => dest.Annonces, opt => opt.Ignore())
            .ForMember(dest => dest.Messages, opt => opt.Ignore())
            .ForMember(dest => dest.Abonnements, opt => opt.Ignore())
            .ForMember(dest => dest.Abonnes, opt => opt.Ignore())
            .ForMember(dest => dest.UtilisateursBloques, opt => opt.Ignore())
            .ForMember(dest => dest.BloqueParUtilisateurs, opt => opt.Ignore())
            .ForMember(dest => dest.NotesAuteur, opt => opt.Ignore())
            .ForMember(dest => dest.NotesCible, opt => opt.Ignore())
            .ForMember(dest => dest.AnnoncesFavorites, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.Signalements, opt => opt.Ignore())
            .ForMember(dest => dest.SignalementsUtilisateurs, opt => opt.Ignore())
            .ForMember(dest => dest.Achats, opt => opt.Ignore())
            .ForMember(dest => dest.Ventes, opt => opt.Ignore())
            .ForMember(dest => dest.Visualisations, opt => opt.Ignore())
            .ForMember(dest => dest.VerificationCodes, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetTokens, opt => opt.Ignore())
            .ForMember(dest => dest.DecisionsModerateur, opt => opt.Ignore())
            .ForMember(dest => dest.DecisionsUtilisateurSanctionne, opt => opt.Ignore())
            .ForMember(dest => dest.CommandesAchetees, opt => opt.Ignore())
            .ForMember(dest => dest.CommandesVendues, opt => opt.Ignore())
            .ForMember(dest => dest.AnnoncesPreferences, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Statut, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoProfil, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Dateinscription, opt => opt.Ignore())
            .ForMember(dest => dest.ValidEmail, opt => opt.Ignore())
            .ForMember(dest => dest.ValidTelephone, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore());
        CreateMap<Utilisateur, UtilisateurDTO>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.DateInscription, opt => opt.MapFrom(src => src.Dateinscription))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.AdresseId, opt => opt.MapFrom(src => src.Adresses.FirstOrDefault(a => a.AdressePays == "France").AdresseId))
            .ForMember(dest => dest.ValidEmail, opt => opt.MapFrom(src => src.ValidEmail))
            .ForMember(dest => dest.ValidTelephone, opt => opt.MapFrom(src => src.ValidTelephone))
            .ForMember(dest => dest.StatutId, opt => opt.MapFrom(src => src.StatutId))
            .ForMember(dest => dest.PreferenceNotifMail, opt => opt.MapFrom(src => src.PreferenceNotifMail))
            .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt))
            .ForMember(dest => dest.DeletedByAdminId, opt => opt.MapFrom(src => src.DeletedByAdminId))
            .ReverseMap();
            
    }
}