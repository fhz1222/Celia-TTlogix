using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class DecantItemMapperProfile: Profile
    {
        public DecantItemMapperProfile()
        {
            CreateMap<TtDecantPkg, DecantItem>()
                .ForMember(s => s.SourcePalletId, db => db.MapFrom(i => i.Pid))
                .ForMember(s => s.SourceQty, db => db.MapFrom(i => i.Qty));
        }
    }
}
