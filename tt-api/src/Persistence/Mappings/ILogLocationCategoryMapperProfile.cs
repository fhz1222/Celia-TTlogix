using Application.UseCases.Registration.Queries.GetILogLocationCategoryCombo;
using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings;

public class ILogLocationCategoryMapperProfile : Profile
{
    public ILogLocationCategoryMapperProfile()
    {
        CreateMap<ILogLocationCategory, ILogLocationCategoryComboDto>()
            .ForMember(x => x.Code, y => y.MapFrom(z => z.Id))
            .ForMember(x => x.Label, y => y.MapFrom(z => z.Name));
    }
}