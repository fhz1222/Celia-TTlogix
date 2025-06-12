using Application.UseCases.Registration.Commands.AddArea;
using Application.UseCases.Registration.Commands.UpdateArea;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.FilterHandlers.Area;
using Application.UseCases.Registration.Queries.GetActiveAreasCombo;

namespace Persistence.Mappings
{
    public class AreaMapperProfile : Profile
    {
        public AreaMapperProfile()
        {
            CreateMap<TtArea, AreasComboDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<AreaWithTypeName, AreaListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<TtArea, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtArea>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtArea, Area>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Area, TtArea>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<UpdateAreaDto, TtArea>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<AddAreaDto, TtArea>();
        }
    }
}
