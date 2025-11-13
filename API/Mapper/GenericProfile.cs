using AutoMapper;
using API.DTO;
using API.DTO.Annonce;
using API.DTO.Categorie;
using API.DTO.Couleur;
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
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix));
        CreateMap<Annonce,AnnonceDetailDTO >();
    }
}