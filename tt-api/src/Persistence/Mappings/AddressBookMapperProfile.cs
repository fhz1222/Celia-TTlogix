using Application.UseCases.CompanyProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class AddressBookMapperProfile : Profile
    {
        public AddressBookMapperProfile()
        {
            CreateMap<TtAddressBook, AddressBookDto>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<TtAddressBook, AddressBook>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<AddressBook, TtAddressBook>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtAddressBook, StatusCancel>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<StatusCancel, TtAddressBook>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtAddressBook, Metadata>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status))
                .ForMember(c => c.CreatedBy, db => db.MapFrom(c => c.CreateBy))
                .ForMember(c => c.CreatedDate, db => db.MapFrom(c => c.CreateDate));

            CreateMap<Metadata, TtAddressBook>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status))
                .ForMember(c => c.CreateBy, db => db.MapFrom(c => c.CreatedBy))
                .ForMember(c => c.CreateDate, db => db.MapFrom(c => c.CreatedDate));

            CreateMap<UpdateAddressBookDto, TtAddressBook>();
        }
    }
}
