using Application.UseCases.Customer;
using Application.UseCases.Registration.Commands.UpdateUom;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class UomMapperProfile : Profile
    {
        public UomMapperProfile()
        {
            CreateMap<TtUOM, UomDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<TtUOM, UomListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<TtUOM, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtUOM>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtUOM, Uom>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<Uom, TtUOM>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

            CreateMap<UpdateUomDto, TtUOM>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));
        }
    }
}
