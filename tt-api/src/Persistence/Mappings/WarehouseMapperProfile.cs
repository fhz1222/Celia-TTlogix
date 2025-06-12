using Application.UseCases.Registration.Commands.UpdateWarehouse;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;
using Application.UseCases.Registration.Commands.AddWarehouse;
using Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;

namespace Persistence.Mappings
{
    public class WarehouseMapperProfile : Profile
    {
        public WarehouseMapperProfile()
        {
            CreateMap<TtWarehouse, WarehouseComboDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<TtWarehouse, WarehouseListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<TtWarehouse, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtWarehouse>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtWarehouse, Warehouse>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<Warehouse, TtWarehouse>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

            CreateMap<UpdateWarehouseDto, TtWarehouse>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<AddWarehouseDto, TtWarehouse>()
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));
        }
    }
}
