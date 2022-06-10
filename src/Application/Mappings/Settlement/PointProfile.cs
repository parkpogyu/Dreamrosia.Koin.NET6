using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Domain.Entities;

namespace Dreamrosia.Koin.Application.Mappings
{
    public class PointProfile : Profile
    {
        public PointProfile()
        {
            CreateMap<Point, PointDto>();
        }
    }
}