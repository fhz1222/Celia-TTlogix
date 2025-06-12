using Application.UseCases.Inventory.Queries.GetInventoryItems;
using AutoMapper;
using Domain.ValueObjects;
using Presentation.Common;

namespace Persistence.Mappings
{
    /// <summary>
    /// Automapper for inventory item parameters; maps InventoryItemParameters object to InventoryItemDtoFilter object
    /// </summary>
    public class InventoryParametersMapperProfile : Profile
    {
        /// <summary>
        /// default constructor - creates the map
        /// </summary>
        public InventoryParametersMapperProfile()
        {
            CreateMap<InventoryItemParameters, InventoryItemDtoFilter>()
                .ForMember(s => s.ProductCode, d => d.MapFrom(i => i.ProductCode))
                .ForMember(s => s.CustomerCode, d => d.MapFrom(i => i.CustomerCode))
                .ForMember(s => s.SupplierId, d => d.MapFrom(i => i.SupplierId))
                .ForMember(s => s.Ownership, d => d.MapFrom(i => i.Ownership == null ? null : Ownership.From(i.Ownership.Value)))
                .ForMember(s => s.OnHandQty, d => d.MapFrom(i => i.OnHandQty))
                .ForMember(s => s.AllocatedQty, d => d.MapFrom(i => i.AllocatedQty))
                .ForMember(s => s.QuarantineQty, d => d.MapFrom(i => i.QuarantineQty))
                .ForMember(s => s.IncomingQty, d => d.MapFrom(i => i.IncomingQty))
                .ForMember(s => s.PickableQty, d => d.MapFrom(i => i.PickableQty))
                .ForMember(s => s.BondedQty, d => d.MapFrom(i => i.BondedQty))
                .ForMember(s => s.NonBondedQty, d => d.MapFrom(i => i.NonBondedQty));
        }
    }
}
