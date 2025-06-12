using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class AccessLockMapperProfile: Profile
    {
        public AccessLockMapperProfile()
        {
            CreateMap<TT_AccessLock, AccessLock>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.UserCode, db => db.MapFrom(i => i.UserCode))
                .ForMember(s => s.ComputerName, db => db.MapFrom(i => i.ComputerName))
                .ForMember(s => s.ModuleName, db => db.MapFrom(i => i.ModuleName));
        }
    }
}
