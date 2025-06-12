using Application.UseCases.StorageDetails;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    internal class StorageDetailMapperProfile : Profile
    {
        public StorageDetailMapperProfile()
        {
            CreateMap<TtStorageDetail, StorageDetailItemDto>()
                .ForPath(s => s.Product.Code, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.Product.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.Product.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
                .ForMember(s => s.Ownership, db => db.MapFrom(i => (Ownership)i.Ownership))
                .ForMember(s => s.PID, db => db.MapFrom(i => i.Pid))
                .ForMember(s => s.InboundJobNo, db => db.MapFrom(i => i.InJobNo))
                .ForMember(s => s.OutboundJobNo, db => db.MapFrom(i => i.OutJobNo))
                .ForMember(s => s.LineItem, db => db.MapFrom(i => i.LineItem))
                .ForMember(s => s.SequenceNo, db => db.MapFrom(i => i.SeqNo))
                .ForMember(s => s.Status, db => db.MapFrom(i => (StorageStatus)i.Status))
                .ForMember(s => s.ControlCode1, db => db.MapFrom(i => i.ControlCode1))
                .ForMember(s => s.ControlCode2, db => db.MapFrom(i => i.ControlCode2))
                .ForMember(s => s.ParentId, db => db.MapFrom(i => i.ParentId))
                .ForMember(s => s.InboundDate, db => db.MapFrom(i => i.InboundDate))
                .ForMember(s => s.BondedStatus, db => db.MapFrom(i => i.BondedStatus))
                .ForMember(s => s.Qty, db => db.MapFrom(i => (int)i.Qty))
                .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int)i.AllocatedQty));

            CreateMap<TT_StorageDetail, Pallet>()
                .ForPath(s => s.Product.Code, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.Product.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.Product.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.WHSCode))
                .ForMember(s => s.Ownership, db => db.MapFrom(i => (Ownership) i.Ownership))
                .ForMember(s => s.Id, db => db.MapFrom(i => i.PID))
                .ForMember(s => s.InboundDate, db => db.MapFrom(i => i.InboundDate))
                .ForMember(s => s.Location, db => db.MapFrom(i => i.LocationCode))
                .ForMember(s => s.Qty, db => db.MapFrom(i => (int)i.Qty))
                .ForMember(s => s.QtyPerPkg, db => db.MapFrom(i => (int)i.QtyPerPkg))
                .ForMember(s => s.OriginalQty, db => db.MapFrom(i => (int)i.OriginalQty))
                .ForMember(s => s.Status, db => db.MapFrom(i => (StorageStatus)i.Status))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight));

            CreateMap<TtStorageDetail, Pallet>()
                .ForPath(s => s.Product.Code, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.Product.CustomerSupplier.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.Product.CustomerSupplier.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
                .ForMember(s => s.Ownership, db => db.MapFrom(i => (Ownership) i.Ownership))
                .ForMember(s => s.Id, db => db.MapFrom(i => i.Pid))
                .ForMember(s => s.InboundDate, db => db.MapFrom(i => i.InboundDate))
                .ForMember(s => s.Location, db => db.MapFrom(i => i.LocationCode))
                .ForMember(s => s.Qty, db => db.MapFrom(i => (int)i.Qty))
                .ForMember(s => s.QtyPerPkg, db => db.MapFrom(i => (int)i.QtyPerPkg))
                .ForMember(s => s.OriginalQty, db => db.MapFrom(i => (int)i.OriginalQty))
                .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => (int)i.AllocatedQty))
                .ForMember(s => s.Status, db => db.MapFrom(i => (StorageStatus)i.Status))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight))
                .ForMember(s => s.InboundJobNo, db => db.MapFrom(i => i.InJobNo))
                .ForMember(s => s.OutboundJobNo, db => db.MapFrom(i => i.OutJobNo))
                .ForMember(s => s.IsVmi, db => db.MapFrom(i => i.IsVmi == 1));

            CreateMap<Pallet, TtStorageDetail>()
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.Product.Code))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.Product.CustomerSupplier.CustomerCode))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.Product.CustomerSupplier.SupplierId))
                .ForMember(s => s.Whscode, db => db.MapFrom(i => i.WhsCode))
                .ForMember(s => s.Ownership, db => db.MapFrom(i => (byte)(i.Ownership??0)))
                .ForMember(s => s.Pid, db => db.MapFrom(i => i.Id))
                .ForMember(s => s.InboundDate, db => db.MapFrom(i => i.InboundDate))
                .ForMember(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForMember(s => s.OriginalQty, db => db.MapFrom(i => i.Qty))
                .ForMember(s => s.QtyPerPkg, db => db.MapFrom(i => (int)i.QtyPerPkg))
                .ForMember(s => s.AllocatedQty, db => db.MapFrom(i => i.AllocatedQty))
                .ForMember(s => s.Status, db => db.MapFrom(i => (byte)i.Status))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight))
                .ForMember(s => s.InJobNo, db => db.MapFrom(i => i.InboundJobNo))
                .ForMember(s => s.OutJobNo, db => db.MapFrom(i => i.OutboundJobNo))
                .ForMember(s => s.IsVmi, db => db.MapFrom(i => i.IsVmi ? 1 : 0));
        } 
    }
}
