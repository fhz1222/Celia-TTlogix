using Application.UseCases.CompanyProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class AddressContactMapperProfile : Profile
    {
        public AddressContactMapperProfile()
        {
            CreateMap<TtAddressContact, AddressContactDto>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<TtAddressContact, AddressContact>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<AddressContact, TtAddressContact>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtAddressContact, StatusCancel>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<StatusCancel, TtAddressContact>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtAddressContact, Metadata>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<Metadata, TtAddressContact>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<UpdateAddressContactDto, TtAddressContact>();
        }
    }
}
