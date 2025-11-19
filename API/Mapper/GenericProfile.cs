using AutoMapper;
using API.DTO;
using API.DTO.Annonce;
using API.DTO.Categorie;
using API.DTO.Conversation;
using API.DTO.Couleur;
using API.DTO.Favoris;
using API.DTO.Message;
using API.DTO.SousCategorie;
using API.DTO.StatutAnnonce;
using API.DTO.Taille;
using API.DTO.Utilisateur;
using API.Models;
using API.Models.EntityFramework;

namespace API.Mapper;

public class GenericProfile : Profile
{
    public GenericProfile()
    {
        CreateMap<SousCategorie, SousCategorieDTO>();
        
        CreateMap<Categorie, CategorieDTO>()
            .ForMember(dest => dest.IdCategorie, opt => opt.MapFrom(src => src.CategorieId))
            .ForMember(dest => dest.LibelleCategorie, opt => opt.MapFrom(src => src.LibelleCategorie))
            .ForMember(dest => dest.SousCategories, opt => opt.MapFrom(src => src.SousCategories));

        CreateMap<StatutAnnonce, StatutAnnonceDTO>();
        CreateMap<Couleur, CouleurDTO>();
        CreateMap<Taille, TailleDTO>();
        CreateMap<Utilisateur, UtilisateurDTO>().ReverseMap();

        CreateMap<Annonce, AnnonceDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoUri)))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix))
            .ForMember(dest => dest.NomAuteur, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.UriPhotoProfilAuteur, opt => opt.MapFrom(src => src.Utilisateur.PhotoProfil.PhotoUri));
        CreateMap<Annonce,AnnonceDetailDTO >()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.Marque.NomMarque))
            .ForMember(dest => dest.EtatArticle, opt => opt.MapFrom(src => src.Etat.NomEtat))
            .ForMember(dest => dest.Taille, opt => opt.MapFrom(src => src.Taille.Libelletaille)) 
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Photo.PhotoUri)))
            .ForMember(dest => dest.NombreLikes, opt => opt.MapFrom(src => src.UtilisateursFavoris.Count))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix));

        CreateMap<FavorisDTO, Favoris>();
        
        CreateMap<Conversation, ConversationDTO>()
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Messages.OrderByDescending(m => m.MessageTexte).FirstOrDefault()))
            .ForMember(dest => dest.LastMessageDate, opt=> opt.MapFrom(src => src.Messages.OrderByDescending(m => m.MessageDate).FirstOrDefault()))
            .ForMember(dest => dest.Acheteur, opt => opt.MapFrom(src => src.Acheteur.UtilisateurAcheteur.Login))
            .ForMember(dest => dest.Vendeur, opt => opt.MapFrom(src => src.Vendeur.UtilisateurVendeur.Login))
            .ForMember(dest => dest.Annonce, opt => opt.MapFrom(src => src.LAnnonce.Title));
        
        CreateMap<Conversation, ConversationDetailDTO>()
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.ListMessages, opt => opt.MapFrom(src => src.Messages))
            .ForMember(dest => dest.Vendeur, opt => opt.MapFrom(src => src.Vendeur.UtilisateurVendeur.Login))
            .ForMember(dest => dest.Acheteur, opt => opt.MapFrom(src => src.Acheteur.UtilisateurAcheteur.Login))
            .ForMember(dest => dest.Annonce, opt => opt.MapFrom(src => src.LAnnonce.Title))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.LAnnonce.Prix));
        CreateMap<Message, MessageDTO>()
            .ForMember(dest=> dest.MessageId, opt=> opt.MapFrom(src=>src.MessageId))
            .ForMember(dest=> dest.Date, opt=> opt.MapFrom(src=>src.MessageDate))
            .ForMember(dest=>dest.Lu, opt=>opt.MapFrom(src=> src.MessageLu))
            .ForMember(dest=> dest.Contenu, opt=> opt.MapFrom(src=>src.MessageTexte))
            .ForMember(dest=> dest.Utilisateur,opt=> opt.MapFrom(src=>src.Utilisateur.Login));
    }
}