using AutoMapper;
using MrPorker.Data.Dtos;
using MrPorker.Data.Models;
using MrPorker.Data.Models.SubModels;

namespace MrPorker
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MeasurementDto, MeasurementModel>().ReverseMap();
            CreateMap<MeasurementDto, AddymerMeasurementModel>().ReverseMap();
            CreateMap<MeasurementDto, PaulMeasurementModel>().ReverseMap();
            CreateMap<MeasurementDto, AlexMeasurementModel>().ReverseMap();
            CreateMap<MeasurementDto, EunoraMeasurementModel>().ReverseMap();
            CreateMap<MeasurementDto, BluMeasurementModel>().ReverseMap();
        }
    }
}
