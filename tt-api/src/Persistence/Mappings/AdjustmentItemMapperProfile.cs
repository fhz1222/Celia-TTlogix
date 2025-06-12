using Application.UseCases.AdjustmentItems;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class AdjustmentItemMapperProfile: Profile
    {
        public AdjustmentItemMapperProfile()
        {
            CreateMap<TT_InvAdjustmentDetail, AdjustmentItem>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.LineItem, db => db.MapFrom(i => i.LineItem))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.PID, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.InitialQty, db => db.MapFrom(i => i.OldQty))
                .ForMember(s => s.NewQty, db => db.MapFrom(i => i.NewQty))
                .ForMember(s => s.InitialQtyPerPkg, db => db.MapFrom(i => i.OldQtyPerPkg))
                .ForMember(s => s.NewQtyPerPkg, db => db.MapFrom(i => i.NewQtyPerPkg))
                .ForMember(s => s.Remarks, db => db.MapFrom(i => i.Remark))
                .ForMember(s => s.IsPositive, db => db.MapFrom(i => i.Act == (byte) InventoryAdjustmentType.Positive));

            CreateMap<AdjustmentItem, TtInvAdjustmentDetail>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.LineItem, db => db.MapFrom(i => i.LineItem))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.Pid, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.OldQty, db => db.MapFrom(i => i.InitialQty))
                .ForMember(s => s.NewQty, db => db.MapFrom(i => i.NewQty))
                .ForMember(s => s.OldQtyPerPkg, db => db.MapFrom(i => i.InitialQtyPerPkg))
                .ForMember(s => s.NewQtyPerPkg, db => db.MapFrom(i => i.NewQtyPerPkg))
                .ForMember(s => s.Remark, db => db.MapFrom(i => i.Remarks))
                .ForMember(s => s.Act, db => db.MapFrom(i => (byte) (i.IsPositive ? InventoryAdjustmentType.Positive: InventoryAdjustmentType.Negative)));

            CreateMap<TT_InvAdjustmentDetail, AdjustmentItemWithPalletDto>()
            .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
            .ForMember(s => s.LineItem, db => db.MapFrom(i => i.LineItem))
            .ForMember(s => s.NewQty, db => db.MapFrom(i => i.NewQty))
            .ForMember(s => s.NewQtyPerPkg, db => db.MapFrom(i => i.NewQtyPerPkg))
            .ForMember(s => s.Remarks, db => db.MapFrom(i => i.Remark));
        }
    }
}
