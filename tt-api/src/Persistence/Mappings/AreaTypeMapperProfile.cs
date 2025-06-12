using Application.UseCases.Registration.Commands.AddAreaType;
using Application.UseCases.Registration.Commands.UpdateAreaType;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;
using Application.UseCases.Registration.Queries.GetAreaTypesCombo;

namespace Persistence.Mappings
{
    public class AreaTypeMapperProfile : Profile
    {
        public AreaTypeMapperProfile()
        {
            CreateMap<TtAreaType, AreaTypesComboDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<TtAreaType, AreaTypeListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<TtAreaType, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtAreaType>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtAreaType, AreaType>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<AreaType, TtAreaType>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

            CreateMap<UpdateAreaTypeDto, TtAreaType>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<AddAreaTypeDto, TtAreaType>()
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));
        }
    }
}
