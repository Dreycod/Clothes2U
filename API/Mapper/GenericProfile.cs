using AutoMapper;
using API.DTO;
using API.DTO.Categorie;
using API.DTO.Couleur;
using API.DTO.SousCategorie;
using API.DTO.StatutAnnonce;
using API.DTO.Taille;
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
    }
}