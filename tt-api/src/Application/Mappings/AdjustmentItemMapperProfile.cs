using Application.UseCases.AdjustmentItems;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class AdjustmentItemMapperProfile : Profile
{
    public AdjustmentItemMapperProfile()
    {

        CreateMap<AdjustmentItemDto, AdjustmentItem>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.LineItem, db => db.MapFrom(i => i.LineItem ?? 0))
                .ForMember(s => s.PID, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.NewQty, db => db.MapFrom(i => i.NewQty))
                .ForMember(s => s.NewQtyPerPkg, db => db.MapFrom(i => i.NewQtyPerPkg))
                .ForMember(s => s.Remarks, db => db.MapFrom(i => i.Remarks));

        CreateMap<Pallet, AdjustmentItem>()
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.Product.Code))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.Product.CustomerSupplier.SupplierId))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.Product.CustomerSupplier.SupplierId))
                .ForMember(s => s.InitialQty, db => db.MapFrom(i => i.Qty))
                .ForMember(s => s.InitialQtyPerPkg, db => db.MapFrom(i => i.QtyPerPkg));
    }
}

