using AutoMapper;
using captly.Models;
using captly.ViewModels;

namespace captly.Profiles;
public class WhisperOptionProfile : Profile
{
    public WhisperOptionProfile()
    {
        CreateMap<WhisperOption, WhisperOptionViewModel>()
            .ForMember(dest => dest.Option, opt => opt.MapFrom(src => src.Option))
            .ForMember(dest => dest.Original, opt => opt.MapFrom(src => src.Original))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DefaultValue, opt => opt.MapFrom(src => src.DefaultValue))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.IsChecked, opt => opt.MapFrom(src => src.IsChecked));
    }
}
