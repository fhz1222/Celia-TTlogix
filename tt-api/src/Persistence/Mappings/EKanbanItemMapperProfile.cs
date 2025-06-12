using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class EKanbanItemMapperProfile : Profile
{
    public EKanbanItemMapperProfile()
    {
        CreateMap<EKanbanDetail, EKanbanItem>()
            .ForMember(i => i.SuppliedQty, db => db.MapFrom(i => (int)i.QuantitySupplied));
    }
}