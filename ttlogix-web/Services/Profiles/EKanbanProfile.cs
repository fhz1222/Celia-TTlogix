using AutoMapper;
using System.Data;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class EKanbanProfile : Profile
    {
        public EKanbanProfile()
        {
            CreateMap<IDataRecord, EKanbanPartsStatusDto>(MemberList.Source)
                .ForSourceMember(src => src.FieldCount, d => d.DoNotValidate());

            CreateMap<EKanbanHeader, EKanbanHeader>();
            CreateMap<EKanbanDetail, EKanbanDetail>();
        }
    }
}

