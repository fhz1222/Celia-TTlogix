using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class EStockTransferService : ServiceBase<EStockTransferService>, IEStockTransferService
    {
        public EStockTransferService(ITTLogixRepository repository,
            IMapper mapper,
            ILocker locker,
            ILogger<EStockTransferService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<EStockTransferListDto> GetEStockTransferList(EStockTransferListQueryFilter filter)
        {
            var query = repository.GetEStockTransferList<EStockTransferListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new EStockTransferListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<Result<EStockTransferHeader>> GetEStockTransferHeaderForImport(string orderNo, string whsCode)
        {
            var eStockTransferHeader = await repository.GetEStockTransferHeaderAsync(orderNo);
            if (eStockTransferHeader == null)
                return new NotFoundResult<EStockTransferHeader>(new JsonResultError("RecordNotFound").ToJson());

            if (eStockTransferHeader.Status != Core.Enums.EStockTransferStatus.New || !String.IsNullOrWhiteSpace(eStockTransferHeader.StockTransferJobNo))
            {
                return new InvalidResult<EStockTransferHeader>(new JsonResultError("UnableToCancelOrderImported__", "orderNo", orderNo).ToJson());
            }
            return new SuccessResult<EStockTransferHeader>(eStockTransferHeader);
        }

        // StorageController > GetEStockTransferPartsStatusByOwnership
        public async Task<IEnumerable<EStockTransferPartsStatusDto>> GetEStockTransferPartsStatusByOwnership(string orderNo)
        {
            #region query
            var eStockTransferPartsStatusForCPartSql = @"SELECT 
EH.OrderNo, ED.ProductCode, ED.SupplierID, 
       QtyOrdered, PkgOrdered, 
       IsNull(OnHandQty, 0) as OnHandQty, 
       IsNull(OnhandPkg, 0) as OnhandPkg, 
       IsNull(AllocatedQty, 0) as AllocatedQty, 
       IsNull(AllocatedPkg, 0) as AllocatedPkg, 
       IsNull(QuarantineQty, 0) as QuarantineQty, 
       IsNull(QuarantinePkg, 0) as QuarantinePkg, 
       IsNull(StandbyQty, 0) as StandByQty, 
       IsNull(StandbyPkg, 0) as StandbyPkg, 
       IsNull(StockMaintenanceQty, 0) as StockMaintenanceQty, 
       IsNull(StockMaintenancePkg, 0) as StockMaintenancePkg, 
       IsNull(OnHandQty - AllocatedQty - QuarantineQty - StandbyQty, 0) AS AvailableQty, 
       IsNull(OnHandPkg - AllocatedPkg - QuarantinePkg - StandbyPkg, 0) AS AvailablePkg 
FROM EStockTransferHeader EH 

     Inner Join 
     (Select OrderNo, ProductCode, SupplierID, Sum(Quantity) as QtyOrdered, Count(SerialNo) as PkgOrdered 
      FROM EStockTransferDetail 
      Group by OrderNo, ProductCode, SupplierID) ED ON 
      ED.OrderNo = EH.OrderNo 

     Left Outer Join 
(SELECT SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS [InComingQty], 
     SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END) AS [InComingPkg], 
     SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR  TT_StorageDetail.Status = 9 )THEN qty ELSE 0 END) AS [OnhandQty], 
     SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR  TT_StorageDetail.Status = 9 ) THEN 1 ELSE 0 END) AS [OnhandPkg], 
     SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN qty ELSE 0 END) AS [AllocatedQty], 
     SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END) AS [AllocatedPkg], 
     SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS [QuarantineQty], 
     SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END) AS [QuarantinePkg], 
     SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS [StockMaintenanceQty], 
     SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END) AS [StockMaintenancePkg], 
     SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS [StandbyQty], 
     SUM(CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END) AS [StandbyPkg], 
     SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS [TransitQty], 
     SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END) AS [TransitPkg], 
     TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode, TT_StorageDetail.Ownership
 FROM TT_StorageDetail INNER JOIN 
     TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode 
 LEFT JOIN SunsetExpiredAlert 
     ON TT_StorageDetail.CustomerCode = SunsetExpiredAlert.FactoryID 
     AND TT_StorageDetail.SupplierID = SunsetExpiredAlert.SupplierID 
     AND TT_StorageDetail.ProductCode = SunsetExpiredAlert.ProductCode 
 WHERE (TT_StorageDetail.Qty > 0)AND TT_StorageDetail.Status < 90  AND (TT_StorageDetail.Ownership is null OR TT_StorageDetail.Ownership = 0) 
 AND TT_StorageDetail.PutawayDate <=  DATEADD(DAY, (-1 * (SunsetExpiredAlert.SunsetPeriod - 0)), getDate()) 
 GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode, TT_StorageDetail.Ownership ) SD

    ON 
     EH.FactoryID = SD.CustomerCode and 
     ED.SupplierID = SD.SupplierID and 
     ED.ProductCode = SD.ProductCode 
     WHERE        (EH.OrderNo = @orderNo)
     ORDER BY ED.ProductCode, ED.SupplierID";
            #endregion
            IEnumerable<EStockTransferPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eStockTransferPartsStatusForCPartSql, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EStockTransferPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        public async Task<bool> HasAnyEStockTransferDiscrepancy(string jobNo) 
            => await repository.HasAnyEStockTransferDiscrepancy(jobNo);

        public async Task<Result<IEnumerable<EStockTransferPartsStatusDto>>> EStockTransferCheck(IEnumerable<string> orderNos)
        {
            IEnumerable<EStockTransferPartsStatusDto> data = Array.Empty<EStockTransferPartsStatusDto>();
            foreach (var orderno in orderNos)
            {
                var _data = await GetEStockTransferPartsStatusByOwnership(orderno);

                foreach (var item in _data)
                {

                    if (item.AvailablePkg <= 0)
                    {
                        item.Status = "UNABLE TO SUPPLY";
                    }
                    else if (item.AvailablePkg < item.PkgOrdered)
                    {
                        item.Status = "SUPPLY PARTIALLY";
                    }
                    else
                        item.Status = "OK";
                }

                data = data.Concat(_data);
            }
            return new SuccessResult<IEnumerable<EStockTransferPartsStatusDto>>(data);

        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
    }
}
