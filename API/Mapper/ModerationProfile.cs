using API.DTO.MotInterdit;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper;

public class ModerationProfile : Profile
{
    public ModerationProfile()
    {
        CreateMap<MotInterdit, MotInterditDTO>().ReverseMap();
    }
}