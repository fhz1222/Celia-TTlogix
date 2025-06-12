using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class SupplierMasterProfile : Profile
    {
        public SupplierMasterProfile()
        {
            CreateMap<SupplierMaster, SupplierMasterBasicDto>();
        }
    }
}
