using Application.UseCases.Customer;
using Application.UseCases.Registration.Commands.UpdateControlCode;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class ControlCodeMapperProfile : Profile
    {
        public ControlCodeMapperProfile()
        {
            CreateMap<TtControlCode, ControlCodeDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<TtControlCode, ControlCodeListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<TtControlCode, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtControlCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtControlCode, ControlCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<ControlCode, TtControlCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

            CreateMap<UpdateControlCodeDto, TtControlCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));
        }
    }
}
