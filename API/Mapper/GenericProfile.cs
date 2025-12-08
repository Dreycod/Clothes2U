using AutoMapper;
using API.DTO;
using API.DTO.Abonnement;
using API.DTO.Annonce;
using API.DTO.Bloque;
using API.DTO.Categorie;
using API.DTO.Conversation;
using API.DTO.Couleur;
using API.DTO.Decision_suspension;
using API.DTO.Favoris;
using API.DTO.Message;
using API.DTO.Signalement;
using API.DTO.SousCategorie;
using API.DTO.StatutAnnonce;
using API.DTO.Taille;
using API.DTO.Utilisateur;
using API.Models;
using API.Models.EntityFramework;
using API.DTO.NoteUtilisateur;
using API.DTO.Notification;
using API.DTO.DemandeRestauration;
using API.DTO.Recense;
using API.DTO.Visualisation;


namespace API.Mapper;

public class GenericProfile : Profile
{
    public GenericProfile()
    {
        CreateMap<SousCategorie, SousCategorieDTO>()
            .ForMember(dest => dest.SousCategorieId, opt => opt.MapFrom(src => src.SousCategorieId))
            .ForMember(dest => dest.LibelleSousCategorie, opt => opt.MapFrom(src => src.LibelleSousCategorie))
            .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => src.Categorie.LibelleCategorie))
            .ReverseMap();
        
        CreateMap<Categorie, CategorieDTO>()
            .ForMember(dest => dest.IdCategorie, opt => opt.MapFrom(src => src.CategorieId))
            .ForMember(dest => dest.LibelleCategorie, opt => opt.MapFrom(src => src.LibelleCategorie))
            .ForMember(dest => dest.SousCategories, opt => opt.MapFrom(src => src.SousCategories));

        CreateMap<Utilisateur, UtilisateurViewDTO>()
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.DateInscription, opt => opt.MapFrom(src => src.Dateinscription))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ValidEmail, opt => opt.MapFrom(src => src.ValidEmail))
            .ForMember(dest => dest.ValidTelephone, opt => opt.MapFrom(src => src.ValidTelephone))
            .ForMember(dest => dest.Statut, opt => opt.MapFrom(src => src.Statut.StatutLibelle))
            .ForMember(dest => dest.PhotoProfilId, opt => opt.MapFrom(src => src.PhotoProfil != null ? src.PhotoProfil.PhotoId : 0))
            .ForMember(dest => dest.Abonnes, opt => opt.MapFrom(src => src.Abonnes.Count))
            .ForMember(dest => dest.Abonnements, opt => opt.MapFrom(src => src.Abonnements.Count))
            .ForMember(dest => dest.NombreAvis, opt => opt.MapFrom(src => src.NotesCible.Count))
            .ForMember(dest => dest.MoyenneAvis, opt => opt.MapFrom(src => 
                src.NotesCible.Any() 
                    ? src.NotesCible.Average(n => (double)n.Note) 
                    : 0.0));

        CreateMap<UtilisateurPutDTO, Utilisateur>()
            // Ignorer la clé primaire
            .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
    
            // Mapper les propriétés explicitement
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.AdresseId, opt => opt.MapFrom(src => src.AdresseId))
            .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(src => src.PhotoProfilId))
    
            // Ne mapper que les propriétés non nulles (à la fin)
            .ForAllMembers(opt => opt.Condition((src, dest, srcValue) => srcValue != null));

        
        CreateMap<StatutAnnonce, StatutAnnonceDTO>();
        CreateMap<Couleur, CouleurDTO>();
        CreateMap<Taille, TailleDTO>();
        CreateMap<Utilisateur, UtilisateurDTO>().ReverseMap();

        CreateMap<Annonce, AnnonceDTO>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque))
            .ForMember(dest => dest.DateAnnonce, opt => opt.MapFrom(src => src.DateAnnonce))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoId)))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.NomAuteur, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.UriPhotoProfilAuteur, opt => opt.MapFrom(src => src.Utilisateur.PhotoProfil.PhotoId))
            .ReverseMap();
        CreateMap<Annonce, AnnonceDetailDTO>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))

            // Champs liés aux marques, état, taille
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque ?? "Inconnue"))
            .ForMember(dest => dest.MarqueId, opt => opt.MapFrom(src => src.MarqueId))
            
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat ?? "Inconnu"))
            .ForMember(dest => dest.EtatId, opt => opt.MapFrom(src => src.EtatId))
    
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille ?? "Inconnue"))
            .ForMember(dest => dest.TailleId, opt => opt.MapFrom(src => src.TailleId))
    
            // Champs liés aux catégories
            .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => src.Categorie.LibelleCategorie ?? "Inconnue"))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.CategorieId))
    
            .ForMember(dest => dest.SousCategorie, opt => opt.MapFrom(src => src.SousCategorie.LibelleSousCategorie ?? "Inconnue"))
            .ForMember(dest => dest.SousCategorieId, opt => opt.MapFrom(src => src.SousCategorieId))
    
            // Champs liés aux autres relations
            .ForMember(dest => dest.StatutAnnonceId, opt => opt.MapFrom(src => src.StatutAnnonceId))
    
            // Autres champs simples
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? "Sans titre"))
            .ForMember(dest => dest.DateAnnonce, opt => opt.MapFrom(src => src.DateAnnonce))
            .ForMember(dest => dest.Negociable, opt => opt.MapFrom(src => src.Negociable))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
    
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoId).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.LibelleTag).ToList()))
    
            .ReverseMap();


        CreateMap<Marque, MarqueDTO>().ReverseMap();
        CreateMap<FavorisDTO, Favoris>().ReverseMap();
        
        CreateMap<Conversation, ConversationDTO>()
            .ForMember(dest => dest.ConversationId,
                opt => opt.MapFrom(src => src.ConversationId))

            .ForMember(dest => dest.LastMessage,
                opt => opt.MapFrom(src => src.Messages
                    .OrderByDescending(m => m.MessageDate)
                    .Select(m => m.MessageTexte)
                    .FirstOrDefault()))

            .ForMember(dest => dest.LastMessageDate,
                opt => opt.MapFrom(src => src.Messages
                    .OrderByDescending(m => m.MessageDate)
                    .Select(m => m.MessageDate)
                    .FirstOrDefault()))

            .ForMember(dest => dest.Interlocuteur, 
                opt => opt.MapFrom((src, dest, _, context) =>
                    (int)context.Items["CurrentUserId"] == src.Acheteur.UtilisateurAcheteurId
                    ? src.Vendeur.UtilisateurVendeur.Login
                    : src.Acheteur.UtilisateurAcheteur.Login
                    ))
            .ForMember(dest => dest.PhotoInterlocuteurId,
                opt => opt.MapFrom((src, dest, _, context) =>
                    (int)context.Items["CurrentUserId"] == src.Acheteur.UtilisateurAcheteurId
                    ? src.Vendeur.UtilisateurVendeur.PhotoProfil.PhotoId
                    : src.Acheteur.UtilisateurAcheteur.PhotoProfil.PhotoId));
        

        CreateMap<Conversation, ConversationDetailDTO>()
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.ListMessages, opt => opt.MapFrom(src => src.Messages.OrderBy(m => m.MessageDate)))
            // .ForMember(dest => dest.Vendeur, opt => opt.MapFrom(src => src.Vendeur.UtilisateurVendeur.Login))
            // .ForMember(dest => dest.Acheteur, opt => opt.MapFrom(src => src.Acheteur.UtilisateurAcheteur.Login))
            .ForMember(dest => dest.TitreAnnonce, opt => opt.MapFrom(src => src.LAnnonce.Title))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.LAnnonce.Prix))
            .ForMember(dest => dest.AnnonceId,
                opt => opt.MapFrom(src => src.LAnnonce.AnnonceId))
            .ForMember(dest => dest.PhotoAnnonceId,
                opt => opt.MapFrom(src => src.LAnnonce.Photos.First().Photo.PhotoId))
            .ForMember(dest => dest.Interlocuteur,
                opt => opt.MapFrom((src, dest, _, context) =>
                    (int)context.Items["CurrentUserId"] == src.Acheteur.UtilisateurAcheteurId
                    ? src.Vendeur.UtilisateurVendeur.Login
                    : src.Acheteur.UtilisateurAcheteur.Login));
        
        CreateMap<Message, MessageDTO>()
            .ForMember(dest=> dest.MessageId, opt=> opt.MapFrom(src=>src.MessageId))
            .ForMember(dest=> dest.Date, opt=> opt.MapFrom(src=>src.MessageDate))
            .ForMember(dest=>dest.Lu, opt=>opt.MapFrom(src=> src.MessageLu))
            .ForMember(dest=> dest.Contenu, opt=> opt.MapFrom(src=>src.MessageTexte.ContenuMessage.ToString()))
            .ForMember(dest=> dest.Utilisateur,opt=> opt.MapFrom(src=>src.Utilisateur.Login));

 
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

        
        CreateMap<Notification, NotificationDTO>()
            // Type de notification => libellé
            .ForMember(dest => dest.LibelleType,
                opt => opt.MapFrom(src => src.NotificationType.LibelleType))

            
            // --- Est Lu ---
            .ForMember(dest => dest.EstLu, 
                opt => opt.MapFrom(src => src.EstLu))
            // --- Notification Administrateur ---
            .ForMember(dest => dest.AdminText,
                opt => opt.MapFrom(src => src.NotificationAdmins != null
                    ? src.NotificationAdmins.AdminText
                    : null))

            // --- Notification Avertissement ---
            .ForMember(dest => dest.MessageAvertissement,
                opt => opt.MapFrom(src => src.NotificationAvertissements != null
                    ? src.NotificationAvertissements.MessageAvertissement
                    : null))

            // --- Notification Nouveau Message ---
            .ForMember(dest => dest.ConversationId,
                opt => opt.MapFrom(src => src.NotificationMessages != null
                    ? src.NotificationMessages.Message.ConversationId
                    : (int?)null))

            .ForMember(dest => dest.MessagePreview,
                opt => opt.MapFrom(src => src.NotificationMessages != null
                    ? src.NotificationMessages.MessagePreview
                    : null))
            
            // --- Notification Modification Annonce ---
            .ForMember(dest => dest.ModificationAnnonceId,
                opt => opt.MapFrom(src => src.NotificationModifications != null
                    ? src.NotificationModifications.AnnonceId
                    : (int?)null))

            .ForMember(dest => dest.NomAuteur,
                opt => opt.MapFrom(src => src.NotificationMessages != null 
                    ? src.NotificationMessages.Message.Utilisateur.Login      // auteur du message
                    : src.NotificationModifications != null
                        ? src.NotificationModifications.Annonce.Utilisateur.Login  // auteur modif annonce
                        : src.NotificationNouvellesAnnonces != null
                            ? src.NotificationNouvellesAnnonces.Annonce.Utilisateur.Login  // auteur nouvelle annonce
                            : null))


            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.NotificationModifications != null
                    ? src.NotificationModifications.Annonce.Title
                    : null))

            // --- Notification Nouvelle Annonce ---
            .ForMember(dest => dest.NouvelleAnnonceId,
                opt => opt.MapFrom(src => src.NotificationNouvellesAnnonces != null
                    ? src.NotificationNouvellesAnnonces.AnnonceId
                    : (int?)null));

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
            .ForMember(dest => dest.UtilisateurNom, opt => opt.MapFrom(src => src.UtilisateurSuspendu != null ? src.UtilisateurSuspendu.Login : null))

            .ForMember(dest => dest.UtilisateurAdminId, opt => opt.MapFrom(src => src.UtilisateurAdminId))
            .ForMember(dest => dest.AdminNom, opt => opt.MapFrom(src => src.Decisionnaire != null ? src.Decisionnaire.Login : null))

            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.AnnonceTitre, opt => opt.MapFrom(src => src.AnnonceSuspendu != null ? src.AnnonceSuspendu.Title : null));

        CreateMap<DecisionSuspensionCreateDTO, Decision_suspension>()
            .ForMember(dest => dest.Decision_suspensionId, opt => opt.Ignore()) // la DB gère l'ID
            .ForMember(dest => dest.DateDebutSuspension, opt => opt.MapFrom(src => src.DateDebut))
            .ForMember(dest => dest.DateFinSuspension, opt => opt.MapFrom(src => src.DateFin))
            .ForMember(dest => dest.MotifSuspension, opt => opt.MapFrom(src => src.Raison))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.UtilisateurAdminId, opt => opt.MapFrom(src => src.UtilisateurAdminId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId));

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

        CreateMap<Recense, RecenseDTO>()
            .ForMember(dest => dest.RecenseId, opt => opt.MapFrom(src => src.RecenseId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ReverseMap();

        CreateMap<Recense, RecenseDetailDTO>()
            .ForMember(dest => dest.RecenseId, opt => opt.MapFrom(src => src.RecenseId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.AnnonceTitre, opt => opt.MapFrom(src => src.Annonce.Title))
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.LibelleTag, opt => opt.MapFrom(src => src.Tag.LibelleTag))
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