using Application.UseCases.Inventory;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class InventoryMapperProfile : Profile
    {
        public InventoryMapperProfile()
        {
            CreateMap<TtInventory, InventoryItemDto>()
                .ForMember(s => s.OnHandQty, db => db.MapFrom(i => (int)i.OnHandQty))
                .ForMember(s => s.OnHandPkg, db => db.MapFrom(i => (i.OnHandPkg ?? 0)))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
                .ForMember(s => s.Ownership, db => db.MapFrom(i => (Ownership)i.Ownership))
                .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int)i.AllocatedQty))
                .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int)i.AllocatedQty))
                .ForMember(s => s.QuarantineQty, db => db.MapFrom(i => (int)i.QuarantineQty))
                .ForMember(s => s.QuarantinePkg, db => db.MapFrom(i => (i.QuarantinePkg?? 0)))
                .ForPath(s => s.Product.Code, db => db.MapFrom(i => i.ProductCode1))
                .ForPath(s => s.Product.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.Product.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId));

            CreateMap<TtInventory, InventoryItem>()
               .ForMember(s => s.OnHandQty, db => db.MapFrom(i => (int)i.OnHandQty))
               .ForMember(s => s.OnHandPkg, db => db.MapFrom(i => (i.OnHandPkg ?? 0)))
               .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
               .ForMember(s => s.Ownership, db => db.MapFrom(i => (Ownership)i.Ownership))
               .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int)i.AllocatedQty))
               .ForMember(s => s.AllocatedPkg, db => db.MapFrom(i => i.AllocatedPkg ?? 0))
               .ForMember(s => s.QuarantineQty, db => db.MapFrom(i => (int)i.QuarantineQty))
               .ForMember(s => s.QuarantinePkg, db => db.MapFrom(i => (i.QuarantinePkg ?? 0)))
               .ForPath(s => s.Product.Code, db => db.MapFrom(i => i.ProductCode1))
               .ForPath(s => s.Product.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
               .ForPath(s => s.Product.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId));
        }
    }
}
