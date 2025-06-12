using Application.UseCases.Customer;
using Application.UseCases.Registration.Commands.UpdateProductCode;
using Application.UseCases.Registration;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class ProductCodeMapperProfile : Profile
    {
        public ProductCodeMapperProfile()
        {
            CreateMap<TtProductCode, ProductCodeDto>()
                .ForMember(c => c.Label, db => db.MapFrom(c => c.Name));

            CreateMap<TtProductCode, ProductCodeListItemDto>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<TtProductCode, Metadata>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status));

            CreateMap<Metadata, TtProductCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));

            CreateMap<TtProductCode, ProductCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (Status)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (DefinedType)z.Type));

            CreateMap<ProductCode, TtProductCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status))
                .ForMember(x => x.Type, y => y.MapFrom(z => (byte)z.Type));

            CreateMap<UpdateProductCodeDto, TtProductCode>()
                .ForMember(x => x.Status, y => y.MapFrom(z => (byte)z.Status));
        }
    }
}
