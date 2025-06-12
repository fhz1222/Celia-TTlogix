using Application.UseCases.Registration;
using Application.UseCases.Registration.Commands.AddLocation;
using Application.UseCases.Registration.Commands.UpdateLocation;
using Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.FilterHandlers.Location;

namespace Persistence.Mappings;

internal class LocationMapperProfile : Profile
{
    public LocationMapperProfile()
    {
        CreateMap<TtLocation, StockTakeLocationDto>();

        CreateMap<TtLocation, LocationListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (LocationType)z.Type))
                .ForMember(x => x.IsPriority, y => y.MapFrom(z => (int?)z.IsPriority))
                .ForMember(x => x.WarehouseCode, y => y.MapFrom(z => z.Whscode));

        CreateMap<TtLocation, Metadata>()
            .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

        CreateMap<Metadata, TtLocation>()
            .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

        CreateMap<TtLocation, Location>()
            .ForMember(x => x.WarehouseCode, y => y.MapFrom(z => z.Whscode))
            .ForMember(x => x.Type, y => y.MapFrom(z => (LocationType)z.Type))
            .ForMember(x => x.IsActive, y => y.MapFrom(z => z.Status == 1));

        CreateMap<TtLocation, LocationDto>()
            .ForMember(x => x.WarehouseCode, y => y.MapFrom(z => z.Whscode))
            .ForMember(x => x.Type, y => y.MapFrom(z => (LocationType)z.Type))
            .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
            .ForMember(x => x.IsPriority, y => y.MapFrom(z => (int?)z.IsPriority));

        CreateMap<LocationDto, TtLocation>()
            .ForMember(x => x.Whscode, y => y.MapFrom(z => z.WarehouseCode))
            .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type))
            .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
            .ForMember(x => x.IsPriority, y => y.MapFrom(z => (byte?)z.IsPriority));

        CreateMap<UpdateLocationDto, TtLocation>()
                .ForMember(x => x.Whscode, y => y.MapFrom(z => z.WarehouseCode))
                .ForMember(x => x.IsPriority, y => y.MapFrom(z => z.IsPriority))
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

        CreateMap<AddLocationDto, TtLocation>()
                .ForMember(x => x.Whscode, y => y.MapFrom(z => z.WarehouseCode))
                .ForMember(x => x.IsPriority, y => y.MapFrom(z => (byte?)z.IsPriority))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));
    }
}
