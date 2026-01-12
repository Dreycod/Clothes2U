using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.EtatArticle;
using Shared.DTO.Recense;
using Shared.DTO.SousCategorie;
using Shared.DTO.StatutAnnonce;
using Shared.DTO.Taille;
using Shared.DTO.Marque;
using Shared.DTO.Mesures;
using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.Tag;

namespace API.Mapper;

public class AnnonceMappingProfile : Profile
{
    public AnnonceMappingProfile()
    {
        CreateMap<Annonce, AnnonceDTO>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.Titre, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoId)))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
            .ForMember(dest => dest.NombreVues, opt => opt.MapFrom(src => src.LesVisualisations.Count))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.NomAuteur, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.IdAuteur, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.IdPhotoProfilAuteur,
                opt => opt.MapFrom(src => src.Utilisateur.PhotoProfil.PhotoId))
            .ReverseMap();
        
        
        
        CreateMap<Annonce, AnnonceDetailDTO>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque ?? "Inconnue"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat ?? "Inconnu"))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille ?? "Inconnue"))
            .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => src.Categorie.LibelleCategorie ?? "Inconnue"))
            .ForMember(dest => dest.SousCategorie, opt => opt.MapFrom(src => src.SousCategorie.LibelleSousCategorie ?? "Inconnue"))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? "Sans titre"))
            .ForMember(dest => dest.DateAnnonce, opt => opt.MapFrom(src => src.DateAnnonce))
            .ForMember(dest => dest.Negociable, opt => opt.MapFrom(src => src.Negociable))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
            .ForMember(dest => dest.NombreVues, opt => opt.MapFrom(src => src.LesVisualisations.Count))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoId).ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag.LibelleTag).ToList()))
            .ForMember(dest => dest.Couleurs, opt => opt.MapFrom(src => src.Couleurs.Select(t => t.Couleur.Nom).ToList()))
            .ForMember(dest => dest.StatutAnnonce, opt => opt.MapFrom(src => src.Statut.StatutLibelle))
            .ForMember(dest => dest.StatutAnnonceId, opt => opt.MapFrom(src => src.StatutAnnonceId))
            .ReverseMap();

        CreateMap<CreateAnnonceDTO, Annonce>()
                .ForMember(dest => dest.AnnonceId, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titre))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DateAnnonce, opt => opt.MapFrom(src => src.DateAnnonce))
                .ForMember(dest => dest.Negociable, opt => opt.MapFrom(src => src.EstNegociable))
                .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
                .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
                .ForMember(dest => dest.EtatId, opt => opt.MapFrom(src => src.EtatId))
                .ForMember(dest => dest.MarqueId, opt => opt.MapFrom(src => src.MarqueId))
                .ForMember(dest => dest.TailleId, opt => opt.MapFrom(src => src.TailleId))
                .ForMember(dest => dest.SousCategorieId, opt => opt.MapFrom(src => src.SousCategorieId))
                .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.CategorieId))
                .ForMember(dest => dest.StatutAnnonceId, opt => opt.MapFrom(src => src.StatutAnnonceId))
                .ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.GenreId))
                .ForMember(dest => dest.Marque, opt => opt.Ignore())
                .ForMember(dest => dest.Taille, opt => opt.Ignore())
                .ForMember(dest => dest.Etat, opt => opt.Ignore())
                .ForMember(dest => dest.SousCategorie, opt => opt.Ignore())
                .ForMember(dest => dest.Categorie, opt => opt.Ignore())
                .ForMember(dest => dest.Statut, opt => opt.Ignore())
                .ForMember(dest => dest.GenreAnnonce, opt => opt.Ignore())
                .ForMember(dest => dest.Utilisateur, opt => opt.Ignore())
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Couleurs, opt => opt.Ignore())
                .ForMember(dest => dest.UtilisateursFavoris, opt => opt.Ignore())
                .ForMember(dest => dest.LesVisualisations, opt => opt.Ignore());

        CreateMap<PutAnnonceDTO, Annonce>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Titre))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DateAnnonce, opt => opt.MapFrom(src => src.DateAnnonce))
            .ForMember(dest => dest.Negociable, opt => opt.MapFrom(src => src.EstNegociable))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.EtatId, opt => opt.MapFrom(src => src.EtatId))
            .ForMember(dest => dest.MarqueId, opt => opt.MapFrom(src => src.MarqueId))
            .ForMember(dest => dest.TailleId, opt => opt.MapFrom(src => src.TailleId))
            .ForMember(dest => dest.SousCategorieId, opt => opt.MapFrom(src => src.SousCategorieId))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.CategorieId))
            .ForMember(dest => dest.StatutAnnonceId, opt => opt.MapFrom(src => src.StatutAnnonceId))
            .ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.GenreId))
            // Les relations (Marque, Taille, etc.) seront charg�es par EF Core
            .ForMember(dest => dest.Marque, opt => opt.Ignore())
            .ForMember(dest => dest.Taille, opt => opt.Ignore())
            .ForMember(dest => dest.Etat, opt => opt.Ignore())
            .ForMember(dest => dest.SousCategorie, opt => opt.Ignore())
            .ForMember(dest => dest.Categorie, opt => opt.Ignore())
            .ForMember(dest => dest.Statut, opt => opt.Ignore())
            .ForMember(dest => dest.GenreAnnonce, opt => opt.Ignore())
            .ForMember(dest => dest.Utilisateur, opt => opt.Ignore())
            // Les collections seront g�r�es s�par�ment
            .ForMember(dest => dest.Photos, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Couleurs, opt => opt.Ignore())
            .ForMember(dest => dest.UtilisateursFavoris, opt => opt.Ignore())
            .ForMember(dest => dest.LesVisualisations, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateRecenseDTO, Recense>()
            .ForMember(dest => dest.RecenseId, opt => opt.Ignore())
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.Annonce, opt => opt.Ignore())
            .ForMember(dest => dest.Tag, opt => opt.Ignore())
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
        
        CreateMap<Tag, TagDTO>()
            .ForMember(dest => dest.IdTag, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.LibelleTag, opt => opt.MapFrom(src => src.LibelleTag))
            .ReverseMap();

        CreateMap<CreateTagDTO, Tag>()
            .ForMember(dest => dest.TagId, opt => opt.Ignore())
            .ForMember(dest => dest.LibelleTag, opt => opt.MapFrom(src => src.Libelle))
            .ReverseMap();

        CreateMap<StatutAnnonce, StatutAnnonceDTO>();
        
        CreateMap<Taille, TailleDTO>()
            .ForMember(dest => dest.NombreProduits, opt => opt.MapFrom(src => src.Annonces.Count))
            .ForMember(dest => dest.Mesures, opt => opt.MapFrom(src => src.Mesures))
            .ReverseMap()
            .ForMember(dest => dest.Annonces, opt => opt.Ignore());


        CreateMap<Mesure, MesureDTO>()
            .ReverseMap();

        CreateMap<EtatArticle, EtatArticleDTO>().ReverseMap();

        CreateMap<Marque, MarqueDTO>()
            .ForMember(dest => dest.MarqueID, opt => opt.MapFrom(src => src.MarqueId))
            .ForMember(dest => dest.NombreProduits, opt => opt.MapFrom(src => src.Annonces.Count))
            .ReverseMap()
            .ForMember(dest => dest.Annonces, opt => opt.Ignore());

        CreateMap<SousCategorie, SousCategorieDTO>()
            .ForMember(dest => dest.SousCategorieId, opt => opt.MapFrom(src => src.SousCategorieId))
            .ForMember(dest => dest.LibelleSousCategorie, opt => opt.MapFrom(src => src.LibelleSousCategorie))
            .ForMember(dest => dest.CategorieId, opt => opt.MapFrom(src => src.CategorieId))
            .ReverseMap()
            ;

        CreateMap<SousCategoriePostDTO, SousCategorie>()
            .ForMember(d => d.Categorie, o => o.Ignore());

        CreateMap<Couleur, CouleurDTO>()
            .ForMember(dest => dest.CouleurId, opt => opt.MapFrom(src => src.CouleurId))
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Nom))
            .ForMember(dest => dest.NombreProduits, opt => opt.MapFrom(src => src.Annonces.Count))
             .ReverseMap()
            .ForMember(dest => dest.Annonces, opt => opt.Ignore());

        CreateMap<CreateEstDeCouleurDTO, Est_De_Couleur>()
            .ForMember(dest => dest.CouleurId, opt => opt.MapFrom(src => src.CouleurId))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.Couleur, opt => opt.Ignore())
            .ForMember(dest => dest.Annonce, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Est_De_Couleur, EstDeCouleurDTO>().ReverseMap();

        CreateMap<Categorie, CategorieDTO>()
            .ForMember(dest => dest.IdCategorie, opt => opt.MapFrom(src => src.CategorieId))
            .ForMember(dest => dest.LibelleCategorie, opt => opt.MapFrom(src => src.LibelleCategorie))
            .ForMember(dest => dest.SousCategories, opt => opt.MapFrom(src => src.SousCategories))
            .ForMember(dest => dest.NombreProduits, opt => opt.MapFrom(src => src.Annonces.Count))
             .ReverseMap()
            .ForMember(dest => dest.Annonces, opt => opt.Ignore());
        CreateMap<Genre, GenreDTO>().ReverseMap();
        CreateMap<Annonce, AnnonceSuggestionDTO>()
            .ForMember(dest => dest.Categorie, opt => opt.MapFrom(src => src.Categorie.LibelleCategorie))
            .ForMember(dest => dest.SousCategorie, opt => opt.MapFrom(src => src.SousCategorie.LibelleSousCategorie))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat))
            .ForMember(dest => dest.Couleurs,
                opt => opt.MapFrom(src => src.Couleurs.Select(t => t.Couleur.Nom).ToList()));
    }
}