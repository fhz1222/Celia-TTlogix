using AutoMapper;
using Domain.Entities;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class OwnerMapperProfile: Profile
    {
        public OwnerMapperProfile()
        {
            CreateMap<TT_Owner, Owner>()
                .ForMember(s => s.Code, db => db.MapFrom(i => i.Code))
                .ForMember(s => s.Site, db => db.MapFrom(i => i.Site))
                .ForMember(s => s.CompanyCode, db => db.MapFrom(i => i.CompanyCode));
        }
    }
}
