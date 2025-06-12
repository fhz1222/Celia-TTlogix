using AutoMapper;
using System.Data;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class EStockTransferProfile : Profile
    {
        public EStockTransferProfile()
        {
            CreateMap<IDataRecord, EStockTransferPartsStatusDto>(MemberList.Source)
                .ForSourceMember(src => src.FieldCount, d => d.DoNotValidate());
        }
    }
}

