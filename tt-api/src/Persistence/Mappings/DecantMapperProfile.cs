using Application.UseCases.Decants;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.PetaPoco.Models;

namespace Persistence.Mappings
{
    public class DecantMapperProfile: Profile
    {
        public DecantMapperProfile()
        {
            // map PetaPoco object to dto object
            CreateMap<TT_Decant, DecantDto>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.WHSCode))
                .ForMember(s => s.ReferenceNo, db => db.MapFrom(i => i.RefNo))
                .ForMember(s => s.CreatedDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.Status, db => db.MapFrom(i => DecantStatus.From(i.Status)))
                .ForMember(s => s.Remark, db => db.MapFrom(i => i.Remark));

            CreateMap<TtDecant, Decant>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.WhsCode, db => db.MapFrom(i => i.Whscode))
                .ForMember(s => s.ReferenceNo, db => db.MapFrom(i => i.RefNo))
                .ForMember(s => s.Status, db => db.MapFrom(i => DecantStatus.From(i.Status)))
                .ForMember(s => s.Remark, db => db.MapFrom(i => i.Remark))
                .ForMember(s => s.CreatedBy, db => db.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.CreatedDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.CompletedBy, db => db.MapFrom(i => i.ConfirmedBy))
                .ForMember(s => s.CompletedDate, db => db.MapFrom(i => i.ConfirmedDate))
                .ForMember(s => s.CancelledBy, db => db.MapFrom(i => i.CancelledBy))
                .ForMember(s => s.CancelledDate, db => db.MapFrom(i => i.CancelledDate));

            CreateMap<Decant, TtDecant>()
                .ForMember(s => s.JobNo, db => db.MapFrom(i => i.JobNo))
                .ForMember(s => s.CustomerCode, db => db.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.Whscode, db => db.MapFrom(i => i.WhsCode))
                .ForMember(s => s.RefNo, db => db.MapFrom(i => i.ReferenceNo))
                .ForMember(s => s.Status, db => db.MapFrom(i => (byte) i.Status))
                .ForMember(s => s.Remark, db => db.MapFrom(i => i.Remark))
                .ForMember(s => s.CreatedBy, db => db.MapFrom(i => i.CreatedBy))
                .ForMember(s => s.CreatedDate, db => db.MapFrom(i => i.CreatedDate))
                .ForMember(s => s.ConfirmedBy, db => db.MapFrom(i => i.CompletedBy))
                .ForMember(s => s.ConfirmedDate, db => db.MapFrom(i => i.CompletedDate))
                .ForMember(s => s.CancelledBy, db => db.MapFrom(i => i.CancelledBy))
                .ForMember(s => s.CancelledDate, db => db.MapFrom(i => i.CancelledDate));
        }
    }
}
