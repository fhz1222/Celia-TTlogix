using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;

namespace TT.Core.Interfaces
{
    public interface ITTLogixRepositoryForOutboundImportEKanban
    {
        #region Queryable collections
        IQueryable<PickingList> PickingLists();
        IQueryable<EKanbanDetail> EKanbanDetails();
        IQueryable<StorageDetail> StorageDetails();
        IQueryable<EOrder> EOrders();

        #endregion

        #region queries
        Task<EOrder> GetFirstEOrder(string orderNo, string productCode);
        Task<IList<StorageDetailExtendedQueryResult>> GetStorageDetailListEuro(StorageDetailExtendedQueryFilter filter);
        Task<IList<EKanbanDetail>> GetEKanbanDetailForPicking(EKanbanForPickingQueryFilter filter);
        Task<IEnumerable<OutboundDetailPickingQueryResult>> GetOutboundDetailPickingResultList(string jobNo);

        #endregion

        #region get entity
        Task<Outbound> GetOutboundAsync(string jobNo);
        Task<OutboundDetail> GetOutboundDetailAsync(string jobNo, int lineItem);
        Task<PickingListEKanban> GetPickingListEKanbanAsync(string jobNo, int lineItem, int seqNo);
        Task<PickingAllocatedPID> GetPickingAllocatedPIDAsync(string jobNo, int lineItem, int seqNo);
        Task<PartMaster> GetPartMasterAsync(string customerCode, string supplierId, string productCode);
        Task<Inventory> GetInventoryAsync(string customerCode, string supplierID, string productCode, string wHSCode, Ownership ownership);
        Task<EKanbanHeader> GetEKanbanHeaderAsync(string orderNo);
        Task<bool> EKanbanDetailExistsAsync(string orderNo, string productCode, string serialNo);
        Task<SupplierMaster> GetSupplierMasterAsync(string factoryId, string supplierId);

        #endregion

        #region add entity
        Task AddOutboundAsync(Outbound entity);
        Task AddOutboundDetailAsync(OutboundDetail entity);
        Task AddEKanbanHeaderAsync(EKanbanHeader entity);
        Task AddEKanbanDetailAsync(EKanbanDetail entity);
        Task AddPickingListEKanbanAsync(PickingListEKanban entity);
        Task AddEOrderAsync(EOrder entity);
        Task AddPickingListAsync(PickingList entity);
        Task AddPickingAllocatedPIDAsync(PickingAllocatedPID entity);

        #endregion

        #region delete entity
        Task DeleteOutboundDetailAsync(OutboundDetail entity);
        Task DeleteEKanbanDetailAsync(EKanbanDetail entity);
        Task DeleteEOrderAsync(EOrder entity);
        Task DeletePickingListAsync(PickingList entity);
        Task DeletePickingAllocatedPIDAsync(PickingAllocatedPID entity);
        Task DeletePickingListEKanbanAsync(PickingListEKanban entity);

        #endregion
        void ChangeTrackingOff();
        void ChangeTrackingOn();
        Task SaveChangesAsyncFinal();
        void PreloadDataToLocal(string jobNo, string orderNo, string factoryId, string supplierId, bool isCPart, string whsCode);
    }

}
