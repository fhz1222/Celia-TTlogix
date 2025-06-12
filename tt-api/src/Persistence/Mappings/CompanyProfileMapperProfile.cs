using Application.UseCases.CompanyProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class CompanyProfileMapperProfile : Profile
    {
        public CompanyProfileMapperProfile()
        {
            CreateMap<TtCompanyProfile, CompanyProfileDto>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<TtCompanyProfile, CompanyProfile>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<CompanyProfile, TtCompanyProfile>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtCompanyProfile, StatusCancel>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<StatusCancel, TtCompanyProfile>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<TtCompanyProfile, Metadata>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (Status)c.Status));

            CreateMap<Metadata, TtCompanyProfile>()
                .ForMember(c => c.Status, db => db.MapFrom(c => (byte)c.Status));

            CreateMap<UpsertCompanyProfileDto, TtCompanyProfile>();
        }
    }
}
