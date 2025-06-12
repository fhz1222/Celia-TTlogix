using Application.UseCases.StockTake;
using Application.UseCases.StockTake.Commands.UpdateStockTake;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class StockTakeMapperProfile : Profile
    {
        public StockTakeMapperProfile()
        {
            CreateMap<TtStockTakeByLoc, StockTakeDto>()
                .ForMember(x => x.Status, db => db.MapFrom(x => (JobStatus)x.Status));

            CreateMap<TtStockTakeByLoc, StockTake>()
                .ForMember(x => x.Status, db => db.MapFrom(x => (JobStatus)x.Status));

            CreateMap<StockTake, TtStockTakeByLoc>()
                .ForMember(x => x.Status, db => db.MapFrom(x => (byte)x.Status));

            CreateMap<UpdateStockTakeDto, TtStockTakeByLoc>();

            CreateMap<TtStockTakeByLoc, JobMetadata>()
                .ForMember(x => x.Status, db => db.MapFrom(x => (JobStatus)x.Status));
            CreateMap<JobMetadata, TtStockTakeByLoc>()
                .ForMember(x => x.Status, db => db.MapFrom(x => (byte)x.Status));
        }
    }
}
