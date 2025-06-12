using Application.UseCases.Decants.Commands.CompleteDecantCommand;
using AutoMapper;
using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Mappings
{
    public class DecantItemPalletMapperProfile: Profile
    {
        public DecantItemPalletMapperProfile()
        {
            CreateMap<TtDecantDetail, DecantItemPallet>()
                .ForMember(s => s.PalletId, db => db.MapFrom(i => i.Pid))
                .ForMember(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForMember(s => s.SequenceNo, db => db.MapFrom(i => i.SeqNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight));

            CreateMap<DecantItemPallet, TtDecantDetail>()
                .ForMember(s => s.Pid, db => db.MapFrom(i => i.PalletId))
                .ForMember(s => s.Qty, db => db.MapFrom(i => i.Qty))
                .ForMember(s => s.SeqNo, db => db.MapFrom(i => i.SequenceNo))
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight));

            CreateMap<DecantItemPallet, TtDecantPkg>()
                .ForMember(s => s.Pid, db => db.Ignore())
                .ForMember(s => s.Qty, db => db.Ignore())
                .ForMember(s => s.ProductCode, db => db.MapFrom(i => i.ProductCode))
                .ForMember(s => s.SupplierId, db => db.MapFrom(i => i.SupplierId))
                .ForMember(s => s.Length, db => db.MapFrom(i => i.Length))
                .ForMember(s => s.Height, db => db.MapFrom(i => i.Height))
                .ForMember(s => s.Width, db => db.MapFrom(i => i.Width))
                .ForMember(s => s.NetWeight, db => db.MapFrom(i => i.NetWeight))
                .ForMember(s => s.GrossWeight, db => db.MapFrom(i => i.GrossWeight));
        }
    }
}
