using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class TransactionMapperProfile: Profile
    {
        public TransactionMapperProfile()
        {
            CreateMap<TtInvTransaction, InventoryTransaction>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (InventoryTransactionType) i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.Pkg, db => db.MapFrom(i => i.Pkg))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty))
                .ForPath(s => s.BalancePkg, db => db.MapFrom(i => i.BalancePkg));

            CreateMap<InventoryTransaction, TtInvTransaction>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (byte)i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.Pkg, db => db.MapFrom(i => i.Pkg))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty))
                .ForPath(s => s.BalancePkg, db => db.MapFrom(i => i.BalancePkg));

            CreateMap<TtInvTransactionPerSupplier, InventoryTransactionPerSupplier>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForPath(s => s.Ownership, db => db.MapFrom(i => (Ownership) i.Ownership))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (InventoryTransactionType)i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty));

            CreateMap<InventoryTransactionPerSupplier, TtInvTransactionPerSupplier>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForPath(s => s.Ownership, db => db.MapFrom(i => (byte) i.Ownership))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (byte) i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty));

            CreateMap<TtInvTransactionPerWh, InventoryTransactionPerWhsCode>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (InventoryTransactionType)i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.Pkg, db => db.MapFrom(i => i.Pkg))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty))
                .ForPath(s => s.BalancePkg, db => db.MapFrom(i => i.BalancePkg));

            CreateMap<InventoryTransactionPerWhsCode, TtInvTransactionPerWh>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForPath(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForPath(s => s.Whscode, db => db.MapFrom(i => i.WhsCode))
                .ForPath(s => s.JobDate, db => db.MapFrom(i => i.JobDate))
                .ForPath(s => s.Act, db => db.MapFrom(i => (byte)i.Act))
                .ForPath(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForPath(s => s.Pkg, db => db.MapFrom(i => i.Pkg))
                .ForPath(s => s.BalanceQty, db => db.MapFrom(i => i.BalanceQty))
                .ForPath(s => s.BalancePkg, db => db.MapFrom(i => i.BalancePkg));
        }
    }
}
