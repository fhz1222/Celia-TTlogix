using AutoMapper;
using Domain.Entities;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class ProductMapperProfile: Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<TT_PartMaster, Product>()
                .ForMember(s => s.Code, db => db.MapFrom(i => i.ProductCode1))
                .ForPath(s => s.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId));
            CreateMap<TtPartMaster, Product>()
                .ForMember(s => s.Code, db => db.MapFrom(i => i.ProductCode1))
                .ForPath(s => s.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.SPQ, db => db.MapFrom(i => (int)i.Spq))
                .ForMember(s => s.IsCPart, db => db.MapFrom(i => i.IsCpart))
                .ForMember(s => s.CPartBoxQty, db => db.MapFrom(i => (int)i.CpartSpq));
        }
    }
}
