using AutoMapper;
using MrPorker.Data.Dtos;
using MrPorker.Data.Models;

namespace MrPorker
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MeasurementDto, MeasurementModel>().ReverseMap();
        }
    }
}
