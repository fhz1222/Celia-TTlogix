using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class AdjustmentMapperProfile: Profile
    {
        public AdjustmentMapperProfile()
        {
            // map PetaPoco object to Domain object
            CreateMap<TT_InvAdjustmentMaster, Adjustment>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.WHSCode))
                .ForMember(s => s.ReferenceNo, db => db.MapFrom(i => i.RefNo))
                .ForMember(s => s.JobType, db => db.MapFrom(i => (InventoryAdjustmentJobType) i.JobType))
                .ForMember(s => s.CreatedDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.CreatedBy, db => db.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.CompletedDate, db => db.MapFrom(i => i.ConfirmedDate))
                .ForMember(s => s.CompletedBy, db => db.MapFrom(i => i.ConfirmedBy))
                .ForMember(s => s.CancelledDate, db => db.MapFrom(i => i.CancelledDate))
                .ForMember(s => s.CancelledBy, db => db.MapFrom(i => i.CancelledBy))
                .ForMember(s => s.Status, db => db.MapFrom(i => InventoryAdjustmentStatus.From(i.Status)))
                .ForMember(s => s.Reason, db => db.MapFrom(i => i.Reason));

            // map Domain object entity framework object
            CreateMap<Adjustment, TtInvAdjustmentMaster>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.Whscode, db => db.MapFrom(i => i.WhsCode))
                .ForMember(s => s.RefNo, db => db.MapFrom(i => i.ReferenceNo))
                .ForMember(s => s.JobType, db => db.MapFrom(i => (byte)i.JobType))
                .ForMember(s => s.CreatedDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.CreatedBy, db => db.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.ConfirmedDate, db => db.MapFrom(i => i.CompletedDate))
                .ForMember(s => s.ConfirmedBy, db => db.MapFrom(i => i.CompletedBy))
                .ForMember(s => s.CancelledDate, db => db.MapFrom(i => i.CancelledDate))
                .ForMember(s => s.CancelledBy, db => db.MapFrom(i => i.CancelledBy))
                .ForMember(s => s.Status, db => db.MapFrom(i => (byte)i.Status))
                .ForMember(s => s.Reason, db => db.MapFrom(i => i.Reason))
                .ForMember(s => s.RevisedBy, db => db.Ignore())
                .ForMember(s => s.RevisedDate, db => db.Ignore());
        }
    }
}
