using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Models.ModelEnums;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class EKanbanService : ServiceBase<EKanbanService>, IEKanbanService
    {
        public EKanbanService(ITTLogixRepository repository,
            IUtilityService utilityService,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            ILocker locker,
            ILogger<EKanbanService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.utilityService = utilityService;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
        }

        public async Task<EKanbanListDto> GetEKanbanListForEurope(EKanbanListQueryFilter filter)
        {
            if (appSettings.OwnerCode.StartsWith(UtilityService.TESA_CODE))
            {
                // for Europe
                var query = repository.GetEKanbanList<EKanbanListItemDto>(filter);

                var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
                var total = await query.CountAsync();
                var data = await pagedQuery.ToListAsync();

                return new EKanbanListDto
                {
                    Data = data,
                    PageSize = filter.PageSize,
                    PageNo = filter.PageNo,
                    Total = total
                };
            }
            else
            {
                throw new Exception("Invalid endpoint called for TESA (Europe)");
            }
        }

        public async Task<bool> HasEkanbanForJobNo(string jobNo)
        {
            return await (from header in repository.EKanbanHeaders()
                          join customer in repository.Customers() on header.FactoryID equals customer.Code
                          join outbound in repository.Outbounds() on header.OrderNo equals outbound.RefNo
                          where outbound.JobNo == header.OutJobNo
                          && outbound.JobNo == jobNo
                          && header.Status != (int)EKanbanStatus.Imported
                          && outbound.Status < OutboundStatus.InTransit
                          select header).AnyAsync();
        }

        // (oEKANBANHeader.Instructions == "EHP")
        //StorageController > GetEKanbanPartsStatusByOwnershipEHP(ref oFilter, ref m_dstDataSet);
        public async Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusByOwnershipEHP(string orderNo)
        {
            #region query
            var eKanbanPartsStatusByOwnershipEHP = @"SELECT        EH.OrderNo, ED.ProductCode, ED.SupplierID, ED.QtyOrdered, CAST(ED.PkgOrdered AS DECIMAL) AS PkgOrdered, 0.0 AS OnHandQty, 0.0 AS OnhandPkg, 0.0 AS AllocatedQty, 0.0 AS AllocatedPkg, 0.0 AS QuarantineQty, 0.0 AS QuarantinePkg, 0.0 AS StandByQty, 
                         0.0 AS StandbyPkg, 0.0 AS StockMaintenanceQty, 0.0 AS StockMaintenancePkg, 0.0 AS AvailablePkg, 0.0 AS AvailableQty, CAST(ISNULL(SD.OnhandQty, 0)AS DECIMAL) AS ELXOnHandQty, CAST(ISNULL(SD.OnhandPkg, 0)AS DECIMAL) AS ELXOnhandPkg, 
                         CAST(ISNULL(SD.AllocatedQty, 0)AS DECIMAL) AS ELXAllocatedQty, CAST(ISNULL(SD.AllocatedPkg, 0)AS DECIMAL) AS ELXAllocatedPkg, CAST(ISNULL(SD.QuarantineQty, 0)AS DECIMAL) AS ELXQuarantineQty, CAST(ISNULL(SD.QuarantinePkg, 0)AS DECIMAL) AS ELXQuarantinePkg, 
                         CAST(ISNULL(SD.StandbyQty, 0)AS DECIMAL) AS ELXStandByQty, CAST(ISNULL(SD.StandbyPkg, 0)AS DECIMAL) AS ELXStandbyPkg, CAST(ISNULL(SD.StockMaintenanceQty, 0)AS DECIMAL) AS ELXStockMaintenanceQty, CAST(ISNULL(SD.StockMaintenancePkg, 0)AS DECIMAL) 
                         AS ELXStockMaintenancePkg, CAST(ISNULL(SD.OnhandPkg - SD.AllocatedPkg - SD.QuarantinePkg - SD.StandbyPkg, 0)AS DECIMAL) AS ELXAvailablePkg, CAST(ISNULL(SD.OnhandQty - SD.AllocatedQty - SD.QuarantineQty - SD.StandbyQty, 0)AS DECIMAL) 
                         AS ELXAvailableQty
FROM            EKANBANHeader AS EH INNER JOIN
                             (SELECT        OrderNo, ProductCode, SupplierID, SUM(Quantity) AS QtyOrdered, COUNT(SerialNo) AS PkgOrdered
                               FROM            EKANBANDetail
                               GROUP BY OrderNo, ProductCode, SupplierID) AS ED ON ED.OrderNo = EH.OrderNo LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END) AS InComingPkg, 
                                                         SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END) AS OnhandPkg, SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN qty ELSE 0 END) AS AllocatedQty, 
                                                         SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty, 
                                                         SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END) AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty, 
                                                         SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty, 
                                                         SUM(CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END) AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty, 
                                                         SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END) AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.WHSCode, 
                                                         TT_StorageDetail.Ownership
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership = 1)
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.WHSCode, TT_StorageDetail.Ownership) AS SD ON EH.FactoryID = SD.CustomerCode AND 
                         ED.ProductCode = SD.ProductCode
                        WHERE        (EH.OrderNo = @orderNo)
                        ORDER BY ED.ProductCode, ED.SupplierID";
            #endregion
            IEnumerable<EKanbanPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eKanbanPartsStatusByOwnershipEHP, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EKanbanPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        // no matching instruction
        // StorageController > GetEKanbanPartsStatusForEKanbanCPart
        public async Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForEKanbanCPart(string orderNo)
        {
            #region query
            var eKanbanPartsStatusForEKanbanCPartSql = @"SELECT        EH.OrderNo, ED.ProductCode, ED.SupplierID, ED.QtyOrdered, CAST(ED.PkgOrdered AS DECIMAL) AS PkgOrdered, CAST(ISNULL(SD.OnhandQty, 0)AS DECIMAL) AS OnHandQty, 
                         CAST(ISNULL(SD.OnhandPkg, 0)AS DECIMAL) AS OnhandPkg, CAST(ISNULL(SD.AllocatedQty, 0)AS DECIMAL) AS AllocatedQty, 
                         CAST(ISNULL(SD.AllocatedPkg, 0)AS DECIMAL) AS AllocatedPkg, CAST(ISNULL(SD.QuarantineQty, 0)AS DECIMAL) AS QuarantineQty, 
                         CAST(ISNULL(SD.QuarantinePkg, 0)AS DECIMAL) AS QuarantinePkg, CAST(ISNULL(SD.StandbyQty, 0)AS DECIMAL) AS StandByQty, CAST(ISNULL(SD.StandbyPkg, 0)AS DECIMAL) 
                         AS StandbyPkg, CAST(ISNULL(SD.StockMaintenanceQty, 0)AS DECIMAL) AS StockMaintenanceQty, CAST(ISNULL(SD.StockMaintenancePkg, 0)AS DECIMAL) AS StockMaintenancePkg, CAST(ISNULL(SD.OnhandPkg - SD.AllocatedPkg - SD.QuarantinePkg - SD.StandbyPkg, 0)AS DECIMAL) 
                         AS AvailablePkg, CAST(ISNULL(SD.OnhandQty - SD.AllocatedQty - SD.QuarantineQty - SD.StandbyQty, 0)AS DECIMAL) AS AvailableQty, CAST(ISNULL(ESD.OnhandQty, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.OnhandQty, 0) AS DECIMAL) AS ELXOnHandQty, CAST(ISNULL(ESD.OnhandPkg, 0)AS DECIMAL) 
                         + CAST(ISNULL(ExtSD.OnhandPkg, 0)AS DECIMAL) AS ELXOnhandPkg, CAST(ISNULL(ESD.AllocatedQty, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.AllocatedQty, 0)AS DECIMAL) AS ELXAllocatedQty, CAST(ISNULL(ESD.AllocatedPkg, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.AllocatedPkg, 0)AS DECIMAL) AS ELXAllocatedPkg,
                         CAST(ISNULL(ESD.QuarantineQty, 0)AS DECIMAL) AS ELXQuarantineQty, CAST(ISNULL(ESD.QuarantinePkg, 0)AS DECIMAL) AS ELXQuarantinePkg, CAST(ISNULL(ESD.StandbyQty, 0)AS DECIMAL) AS ELXStandByQty, CAST(ISNULL(ESD.StandbyPkg, 0)AS DECIMAL) AS ELXStandbyPkg,
                         CAST(ISNULL(ESD.StockMaintenanceQty, 0)AS DECIMAL) AS ELXStockMaintenanceQty, CAST(ISNULL(ESD.StockMaintenancePkg, 0)AS DECIMAL) AS ELXStockMaintenancePkg, CAST(ISNULL(ESD.OnhandPkg - ESD.AllocatedPkg - ESD.QuarantinePkg - ESD.StandbyPkg, 0)AS DECIMAL) 
                         AS ELXAvailablePkg, CAST(ISNULL(ESD.OnhandQty - ESD.AllocatedQty - ESD.QuarantineQty - ESD.StandbyQty, 0)AS DECIMAL) AS ELXAvailableQty
FROM EKANBANHeader AS EH INNER JOIN
                  (SELECT EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, SUM(EKANBANDetail.Quantity) AS QtyOrdered, CASE WHEN ISNULL(IsCPart, 0) = 0 THEN COUNT(SerialNo)
                                                         ELSE CEILING(SUM(Quantity / CASE WHEN ISNULL(CPartSPQ, 0) = 0 THEN SPQ ELSE CPartSPQ END)) END AS PkgOrdered
                               FROM            EKANBANHeader INNER JOIN
                                                         EKANBANDetail ON EKANBANHeader.OrderNo = EKANBANDetail.OrderNo LEFT OUTER JOIN
                                                         TT_PartMaster ON EKANBANHeader.FactoryID = TT_PartMaster.CustomerCode AND EKANBANDetail.SupplierID = TT_PartMaster.SupplierID AND
                                                         EKANBANDetail.ProductCode = TT_PartMaster.ProductCode1
                               GROUP BY EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, TT_PartMaster.CPartSPQ, TT_PartMaster.IsCPart) AS ED ON ED.OrderNo = EH.OrderNo LEFT OUTER JOIN
                             (SELECT SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty,
                                                  SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status< 5) OR
                                                       TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND

                                                       TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty,
                                                       SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND

                                                       TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty,
                                                       SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ)
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND(TT_StorageDetail.Status< 90) AND(TT_StorageDetail.Ownership IS NULL OR

                                                       TT_StorageDetail.Ownership = 0)
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS SD ON EH.FactoryID = SD.CustomerCode AND ED.SupplierID = SD.SupplierID AND
                         ED.ProductCode = SD.ProductCode LEFT OUTER JOIN
                             (SELECT SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty,
                                                  SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status< 5) OR
                                                       TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND

                                                       TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty,
                                                       SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND

                                                       TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty,
                                                       SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ)
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty,
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND(TT_StorageDetail.Status< 90) AND(TT_StorageDetail.Ownership IS NULL OR

                                                       TT_StorageDetail.Ownership = 1) AND(TT_StorageDetail.LocationCode<> 'RETURN')
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS ESD ON EH.FactoryID = ESD.CustomerCode AND
                         ED.SupplierID = ESD.SupplierID AND ED.ProductCode = ESD.ProductCode LEFT OUTER JOIN
                             (SELECT SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status< 5) OR
                                                 TT_StorageDetail.Status = 9) THEN qty ELSE 0 END ELSE CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status< 5) OR
                                               TT_StorageDetail.Status = 9) THEN(FLOOR(qty / CPartSPQ) * CPartSPQ) ELSE 0 END END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN((TT_StorageDetail.Status > 0 AND

                                              TT_StorageDetail.Status < 5) OR

                                              TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status< 5) OR
                                            TT_StorageDetail.Status = 9) THEN FLOOR(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND

                                            TT_StorageDetail.Status < 5 THEN(FLOOR(qty / CPartSPQ) * CPartSPQ) ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) 
                                                         AS AllocatedQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND
                                                         TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN FLOOR(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg,
                                                         TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND(TT_StorageDetail.Status< 90) AND(TT_StorageDetail.Ownership IS NULL OR

                                                       TT_StorageDetail.Ownership = 1) AND(TT_StorageDetail.LocationCode = 'RETURN')
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS ExtSD ON EH.FactoryID = ExtSD.CustomerCode AND
                         ED.SupplierID = ExtSD.SupplierID AND ED.ProductCode = ExtSD.ProductCode
                        WHERE        (EH.OrderNo = @orderNo)
                        ORDER BY ED.ProductCode, ED.SupplierID";
            #endregion
            IEnumerable<EKanbanPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eKanbanPartsStatusForEKanbanCPartSql, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EKanbanPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        // (oEKANBANHeader.Instructions == "Supplier")
        // StorageController > GetEKanbanPartsStatusByOwnership
        public async Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusByOwnership(string orderNo)
        {
            #region query
            var eKanbanPartsStatusByOwnershipSql = @"SELECT        EH.OrderNo, ED.ProductCode, ED.SupplierID, ED.QtyOrdered, CAST(ED.PkgOrdered AS DECIMAL) AS PkgOrdered, CAST(ISNULL(SD.OnhandQty, 0)AS DECIMAL) AS OnHandQty, CAST(ISNULL(SD.OnhandPkg, 0)AS DECIMAL) AS OnhandPkg, CAST(ISNULL(SD.AllocatedQty, 0)AS DECIMAL) AS AllocatedQty, 
                         CAST(ISNULL(SD.AllocatedPkg, 0)AS DECIMAL) AS AllocatedPkg, CAST(ISNULL(SD.QuarantineQty, 0)AS DECIMAL) AS QuarantineQty, CAST(ISNULL(SD.QuarantinePkg, 0)AS DECIMAL) AS QuarantinePkg, CAST(ISNULL(SD.StandbyQty, 0)AS DECIMAL) AS StandByQty, CAST(ISNULL(SD.StandbyPkg, 0)AS DECIMAL) 
                         AS StandbyPkg, CAST(ISNULL(SD.StockMaintenanceQty, 0)AS DECIMAL) AS StockMaintenanceQty, CAST(ISNULL(SD.StockMaintenancePkg, 0)AS DECIMAL) AS StockMaintenancePkg, CAST(ISNULL(SD.OnhandPkg - SD.AllocatedPkg - SD.QuarantinePkg - SD.StandbyPkg, 0) AS DECIMAL)
                         AS AvailablePkg, CAST(ISNULL(SD.OnhandQty - SD.AllocatedQty - SD.QuarantineQty - SD.StandbyQty, 0)AS DECIMAL) AS AvailableQty, 0.0 AS ELXOnHandQty, 0.0 AS ELXOnhandPkg, 0.0 AS ELXAllocatedQty, 0.0 AS ELXAllocatedPkg,
                         0.0 AS ELXQuarantineQty, 0.0 AS ELXQuarantinePkg, 0.0 AS ELXStandByQty, 0.0 AS ELXStandbyPkg, 0.0 AS ELXStockMaintenanceQty, 0.0 AS ELXStockMaintenancePkg, 0.0 AS ELXAvailablePkg, 0.0 AS ELXAvailableQty
FROM EKANBANHeader AS EH INNER JOIN
                  (SELECT OrderNo, ProductCode, SupplierID, SUM(Quantity) AS QtyOrdered, COUNT(SerialNo) AS PkgOrdered
                    FROM            EKANBANDetail
                    GROUP BY OrderNo, ProductCode, SupplierID) AS ED ON ED.OrderNo = EH.OrderNo LEFT OUTER JOIN
                  (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END) AS InComingPkg,
                                              SUM(CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR

                                              TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR

                                              TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END) AS OnhandPkg, SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN qty ELSE 0 END) AS AllocatedQty,
                                              SUM(CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty,
                                              SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END) AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty,
                                              SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty,
                                              SUM(CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END) AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty,
                                              SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END) AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode, 
                                                         TT_StorageDetail.Ownership
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode
                               WHERE(TT_StorageDetail.Qty > 0) AND(TT_StorageDetail.Status < 90) AND(TT_StorageDetail.Ownership IS NULL OR

                                                TT_StorageDetail.Ownership = 0)
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode, TT_StorageDetail.Ownership) AS SD ON EH.FactoryID = SD.CustomerCode AND
                         ED.SupplierID = SD.SupplierID AND ED.ProductCode = SD.ProductCode
                        WHERE        (EH.OrderNo = @orderNo)
                        ORDER BY ED.ProductCode, ED.SupplierID";

            #endregion
            IEnumerable<EKanbanPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eKanbanPartsStatusByOwnershipSql, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EKanbanPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        // (oEKANBANHeader.Instructions == "DELJIT") or (oEKANBANHeader.FactoryID == "DGT")
        // StorageController > GetEKanbanPartsStatusForCPart
        public async Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForCPart(string orderNo)
        {
            #region query
            var eKanbanPartsStatusForCPartSql = @"SELECT        EH.OrderNo, ED.ProductCode, ED.SupplierID, ED.QtyOrdered, ED.PkgOrdered, CAST(ISNULL(SD.OnhandQty, 0)AS DECIMAL) AS OnHandQty, CAST(ISNULL(SD.OnhandPkg, 0)AS DECIMAL) AS OnhandPkg, CAST(ISNULL(SD.AllocatedQty, 0)AS DECIMAL) AS AllocatedQty, 
                         CAST(ISNULL(SD.AllocatedPkg, 0)AS DECIMAL) AS AllocatedPkg, CAST(ISNULL(SD.QuarantineQty, 0)AS DECIMAL) AS QuarantineQty, CAST(ISNULL(SD.QuarantinePkg, 0)AS DECIMAL) AS QuarantinePkg, CAST(ISNULL(SD.StandbyQty, 0)AS DECIMAL) AS StandByQty, CAST(ISNULL(SD.StandbyPkg, 0) AS DECIMAL)
                         AS StandbyPkg, CAST(ISNULL(SD.StockMaintenanceQty, 0)AS DECIMAL) AS StockMaintenanceQty, CAST(ISNULL(SD.StockMaintenancePkg, 0)AS DECIMAL) AS StockMaintenancePkg, CAST(ISNULL(SD.OnhandPkg - SD.AllocatedPkg - SD.QuarantinePkg - SD.StandbyPkg, 0) AS DECIMAL)
                         AS AvailablePkg, CAST(ISNULL(SD.OnhandQty - SD.AllocatedQty - SD.QuarantineQty - SD.StandbyQty, 0)AS DECIMAL) AS AvailableQty, CAST(ISNULL(ESD.OnhandQty, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.OnhandQty, 0)AS DECIMAL) AS ELXOnHandQty, CAST(ISNULL(ESD.OnhandPkg, 0) AS DECIMAL)
                         + CAST(ISNULL(ExtSD.OnhandPkg, 0)AS DECIMAL) AS ELXOnhandPkg, CAST(ISNULL(ESD.AllocatedQty, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.AllocatedQty, 0)AS DECIMAL) AS ELXAllocatedQty, CAST(ISNULL(ESD.AllocatedPkg, 0)AS DECIMAL) + CAST(ISNULL(ExtSD.AllocatedPkg, 0)AS DECIMAL) AS ELXAllocatedPkg, 
                         CAST(ISNULL(ESD.QuarantineQty, 0)AS DECIMAL) AS ELXQuarantineQty, CAST(ISNULL(ESD.QuarantinePkg, 0)AS DECIMAL) AS ELXQuarantinePkg, CAST(ISNULL(ESD.StandbyQty, 0)AS DECIMAL) AS ELXStandByQty, CAST(ISNULL(ESD.StandbyPkg, 0)AS DECIMAL) AS ELXStandbyPkg, 
                         CAST(ISNULL(ESD.StockMaintenanceQty, 0)AS DECIMAL) AS ELXStockMaintenanceQty, CAST(ISNULL(ESD.StockMaintenancePkg, 0)AS DECIMAL) AS ELXStockMaintenancePkg, CAST(ISNULL(ESD.OnhandPkg - ESD.AllocatedPkg - ESD.QuarantinePkg - ESD.StandbyPkg, 0) AS DECIMAL)
                         AS ELXAvailablePkg, CAST(ISNULL(ESD.OnhandQty - ESD.AllocatedQty - ESD.QuarantineQty - ESD.StandbyQty, 0)AS DECIMAL) AS ELXAvailableQty
FROM            EKANBANHeader AS EH INNER JOIN
                             (SELECT        EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, SUM(EKANBANDetail.Quantity) AS QtyOrdered, CASE WHEN ISNULL(IsCPart, 0) = 0 THEN COUNT(SerialNo) 
                                                         ELSE CEILING(SUM(Quantity / CASE WHEN ISNULL(CPartSPQ, 0) = 0 THEN SPQ ELSE CPartSPQ END)) END AS PkgOrdered
                               FROM            EKANBANHeader INNER JOIN
                                                         EKANBANDetail ON EKANBANHeader.OrderNo = EKANBANDetail.OrderNo LEFT OUTER JOIN
                                                         TT_PartMaster ON EKANBANHeader.FactoryID = TT_PartMaster.CustomerCode AND EKANBANDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         EKANBANDetail.ProductCode = TT_PartMaster.ProductCode1
                               GROUP BY EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, TT_PartMaster.CPartSPQ, TT_PartMaster.IsCPart) AS ED ON ED.OrderNo = EH.OrderNo LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ) 
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership IS NULL OR
                                                         TT_StorageDetail.Ownership = 0)
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS SD ON EH.FactoryID = SD.CustomerCode AND ED.SupplierID = SD.SupplierID AND 
                         ED.ProductCode = SD.ProductCode LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ) 
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership IS NULL OR
                                                         TT_StorageDetail.Ownership = 1) AND (TT_StorageDetail.LocationCode <> 'RETURN')
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS ESD ON EH.FactoryID = ESD.CustomerCode AND 
                         ED.SupplierID = ESD.SupplierID AND ED.ProductCode = ESD.ProductCode LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN (FLOOR(qty / CPartSPQ) * CPartSPQ) ELSE 0 END END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND 
                                                         TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN FLOOR(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN (FLOOR(qty / CPartSPQ) * CPartSPQ) ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) 
                                                         AS AllocatedQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN FLOOR(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, 
                                                         TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership IS NULL OR
                                                         TT_StorageDetail.Ownership = 1) AND (TT_StorageDetail.LocationCode = 'RETURN')
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS ExtSD ON EH.FactoryID = ExtSD.CustomerCode AND 
                         ED.SupplierID = ExtSD.SupplierID AND ED.ProductCode = ExtSD.ProductCode
                        WHERE        (EH.OrderNo = @orderNo)
                        ORDER BY ED.ProductCode, ED.SupplierID";
            #endregion
            IEnumerable<EKanbanPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eKanbanPartsStatusForCPartSql, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EKanbanPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        // (oEKANBANHeader.Instructions == "SAFETYSTOCK")
        // StorageController > GetEKanbanPartsStatusForCPartWithoutExt
        public async Task<IEnumerable<EKanbanPartsStatusDto>> GetEKanbanPartsStatusForCPartWithoutExt(string orderNo)
        {
            #region query
            var eKanbanPartsStatusForCPartWithoutExtSql = @"SELECT        EH.OrderNo, ED.ProductCode, ED.SupplierID, ED.QtyOrdered, ED.PkgOrdered, CAST(ISNULL(SD.OnhandQty, 0)AS DECIMAL) AS OnHandQty, CAST(ISNULL(SD.OnhandPkg, 0)AS DECIMAL) AS OnhandPkg, CAST(ISNULL(SD.AllocatedQty, 0)AS DECIMAL) AS AllocatedQty, 
                         CAST(ISNULL(SD.AllocatedPkg, 0)AS DECIMAL) AS AllocatedPkg, CAST(ISNULL(SD.QuarantineQty, 0)AS DECIMAL) AS QuarantineQty, CAST(ISNULL(SD.QuarantinePkg, 0)AS DECIMAL) AS QuarantinePkg, CAST(ISNULL(SD.StandbyQty, 0)AS DECIMAL) AS StandByQty, CAST(ISNULL(SD.StandbyPkg, 0) AS DECIMAL)
                         AS StandbyPkg, CAST(ISNULL(SD.StockMaintenanceQty, 0)AS DECIMAL) AS StockMaintenanceQty, CAST(ISNULL(SD.StockMaintenancePkg, 0)AS DECIMAL) AS StockMaintenancePkg, CAST(ISNULL(SD.OnhandPkg - SD.AllocatedPkg - SD.QuarantinePkg - SD.StandbyPkg, 0)AS DECIMAL) 
                         AS AvailablePkg, CAST(ISNULL(SD.OnhandQty - SD.AllocatedQty - SD.QuarantineQty - SD.StandbyQty, 0)AS DECIMAL) AS AvailableQty, CAST(ISNULL(ESD.OnhandQty, 0)AS DECIMAL) AS ELXOnHandQty, CAST(ISNULL(ESD.OnhandPkg, 0)AS DECIMAL) AS ELXOnhandPkg, 
                         CAST(ISNULL(ESD.AllocatedQty, 0)AS DECIMAL) AS ELXAllocatedQty, CAST(ISNULL(ESD.AllocatedPkg, 0)AS DECIMAL) AS ELXAllocatedPkg, CAST(ISNULL(ESD.QuarantineQty, 0)AS DECIMAL) AS ELXQuarantineQty, CAST(ISNULL(ESD.QuarantinePkg, 0)AS DECIMAL) AS ELXQuarantinePkg, 
                         CAST(ISNULL(ESD.StandbyQty, 0)AS DECIMAL) AS ELXStandByQty, CAST(ISNULL(ESD.StandbyPkg, 0)AS DECIMAL) AS ELXStandbyPkg, CAST(ISNULL(ESD.StockMaintenanceQty, 0)AS DECIMAL) AS ELXStockMaintenanceQty, CAST(ISNULL(ESD.StockMaintenancePkg, 0)AS DECIMAL) 
                         AS ELXStockMaintenancePkg, CAST(ISNULL(ESD.OnhandPkg - ESD.AllocatedPkg - ESD.QuarantinePkg - ESD.StandbyPkg, 0)AS DECIMAL) AS ELXAvailablePkg, CAST(ISNULL(ESD.OnhandQty - ESD.AllocatedQty - ESD.QuarantineQty - ESD.StandbyQty, 0)AS DECIMAL) 
                         AS ELXAvailableQty
FROM            EKANBANHeader AS EH INNER JOIN
                             (SELECT        EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, SUM(EKANBANDetail.Quantity) AS QtyOrdered, CASE WHEN ISNULL(IsCPart, 0) = 0 THEN COUNT(SerialNo) 
                                                         ELSE CEILING(SUM(Quantity / CASE WHEN ISNULL(CPartSPQ, 0) = 0 THEN SPQ ELSE CPartSPQ END)) END AS PkgOrdered
                               FROM            EKANBANHeader INNER JOIN
                                                         EKANBANDetail ON EKANBANHeader.OrderNo = EKANBANDetail.OrderNo LEFT OUTER JOIN
                                                         TT_PartMaster ON EKANBANHeader.FactoryID = TT_PartMaster.CustomerCode AND EKANBANDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         EKANBANDetail.ProductCode = TT_PartMaster.ProductCode1
                               GROUP BY EKANBANDetail.OrderNo, EKANBANDetail.ProductCode, EKANBANDetail.SupplierID, TT_PartMaster.CPartSPQ, TT_PartMaster.IsCPart) AS ED ON ED.OrderNo = EH.OrderNo LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ) 
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership IS NULL OR
                                                         TT_StorageDetail.Ownership = 0)
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS SD ON EH.FactoryID = SD.CustomerCode AND ED.SupplierID = SD.SupplierID AND 
                         ED.ProductCode = SD.ProductCode LEFT OUTER JOIN
                             (SELECT        SUM(CASE WHEN TT_StorageDetail.Status = 0 THEN qty ELSE 0 END) AS InComingQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 0 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 0 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS InComingPkg, SUM(CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN qty ELSE 0 END) AS OnhandQty, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN 1 ELSE 0 END ELSE CASE WHEN ((TT_StorageDetail.Status > 0 AND TT_StorageDetail.Status < 5) OR
                                                         TT_StorageDetail.Status = 9) THEN CEILING(qty / CPartSPQ) ELSE 0 END END) AS OnhandPkg, SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN qty ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN AllocatedQty ELSE 0 END END) AS AllocatedQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status > 1 AND TT_StorageDetail.Status < 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status > 1 AND 
                                                         TT_StorageDetail.Status < 5 THEN CEILING(AllocatedQty / CPartSPQ) ELSE 0 END END) AS AllocatedPkg, SUM(CASE WHEN TT_StorageDetail.Status = 9 THEN qty ELSE 0 END) AS QuarantineQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 9 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 9 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS QuarantinePkg, SUM(CASE WHEN TT_StorageDetail.Status = 21 THEN qty ELSE 0 END) AS StockMaintenanceQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 21 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 21 THEN CEILING(qty / CPartSPQ) 
                                                         ELSE 0 END END) AS StockMaintenancePkg, SUM(CASE WHEN TT_Location.Type = 3 THEN qty ELSE 0 END) AS StandbyQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_Location.Type = 3 THEN 1 ELSE 0 END ELSE CASE WHEN TT_Location.Type = 3 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS StandbyPkg, SUM(CASE WHEN TT_StorageDetail.Status = 5 THEN qty ELSE 0 END) AS TransitQty, 
                                                         SUM(CASE WHEN TT_PartMaster.IsCPart = 0 THEN CASE WHEN TT_StorageDetail.Status = 5 THEN 1 ELSE 0 END ELSE CASE WHEN TT_StorageDetail.Status = 5 THEN CEILING(qty / CPartSPQ) ELSE 0 END END) 
                                                         AS TransitPkg, TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode
                               FROM            TT_StorageDetail INNER JOIN
                                                         TT_Location ON TT_StorageDetail.LocationCode = TT_Location.Code AND TT_StorageDetail.WHSCode = TT_Location.WHSCode INNER JOIN
                                                         TT_PartMaster ON TT_StorageDetail.CustomerCode = TT_PartMaster.CustomerCode AND TT_StorageDetail.SupplierID = TT_PartMaster.SupplierID AND 
                                                         TT_StorageDetail.ProductCode = TT_PartMaster.ProductCode1
                               WHERE        (TT_StorageDetail.Qty > 0) AND (TT_StorageDetail.Status < 90) AND (TT_StorageDetail.Ownership IS NULL OR
                                                         TT_StorageDetail.Ownership = 1) AND (TT_StorageDetail.LocationCode <> 'RETURN')
                               GROUP BY TT_StorageDetail.CustomerCode, TT_StorageDetail.ProductCode, TT_StorageDetail.SupplierID, TT_StorageDetail.WHSCode) AS ESD ON EH.FactoryID = ESD.CustomerCode AND 
                         ED.SupplierID = ESD.SupplierID AND ED.ProductCode = ESD.ProductCode
                        WHERE        (EH.OrderNo = @orderNo)
                        ORDER BY ED.ProductCode, ED.SupplierID";

            #endregion
            IEnumerable<EKanbanPartsStatusDto> res = null;
            await repository.ExecuteQueryAsync(eKanbanPartsStatusForCPartWithoutExtSql, (r) =>
            {
                res = mapper.Map<IDataReader, IEnumerable<EKanbanPartsStatusDto>>(r);
            }, new KeyValuePair<string, object>("@orderNo", orderNo));
            return res;
        }

        public async Task<Result<string>> CreateEKanbanManual(string jobNo, string customerCode, ManualType manualType)
        {
            return await WithTransactionScope<string>(async () =>
            {
                var orderNo = await utilityService.GetNextOrderNo(UtilityService.MANUAL_ORDER_PREFIX, 3);
                var eKanbanHeader = new EKanbanHeader
                {
                    OrderNo = orderNo,
                    FactoryID = customerCode,
                    RunNo = "",
                    IssuedDate = DateTime.Now,
                    Instructions = manualType == ManualType.ManualEHP ? "EHP" : (manualType == ManualType.ManualSUP ? "Supplier" : ""),
                    Status = (int)EKanbanStatus.Imported,
                    OutJobNo = jobNo
                };
                await repository.AddEKanbanHeaderAsync(eKanbanHeader);
                return new SuccessResult<string>(orderNo);
            });
        }

        public async Task<IList<string>> GetEKanbanDataToDownload(string orderNumber)
        {
            var resultData = new List<string>();
            StringBuilder sbHeaderInfo = new StringBuilder();
            // get the data from StorageDetail based on JobNo
            var eKanbanData = await repository.EKanbanDetails().Where(e => e.OrderNo == orderNumber).ToListAsync();

            // OK, after we retrieve the data, we can generate the header information
            // Columns	Item Description
            // 1) 01-12	Filename								(12 char
            // 2) 13-17	Number of rows of data					(05 char
            // 3) 18-19	Total number of fields, m				(02 char
            // 4) 20-21	Length of datafield 1					(02 char
            // 5) 22-23	Length of datafield ...m				(02 char

            // Step1 : Construct the header information
            sbHeaderInfo.Append("TKANBAN.DAT ");
            sbHeaderInfo.Append(eKanbanData.Count.ToString("00000"));  //number of records
            sbHeaderInfo.Append("07");  // number of fields
            sbHeaderInfo.Append("15");  // Order No
            sbHeaderInfo.Append("30");  // Product Code
            sbHeaderInfo.Append("30");  // Serial No
            sbHeaderInfo.Append("10");  // Supplier ID
            sbHeaderInfo.Append("10");  // Supplied Qty
            sbHeaderInfo.Append("01");  // status
            sbHeaderInfo.Append("01");  // flag

            // dump Heade Information into the first column of the string array
            resultData.Add(sbHeaderInfo.ToString());

            StringBuilder sb = new StringBuilder();
            string str = "";
            int RowNumber = 1;
            foreach (var row in eKanbanData)
            {
                str = "";
                str += RowNumber.ToString("00000"); // line number for scanner
                str += FillString(row.OrderNo, 15);
                str += FillString(row.ProductCode, 30);
                int.TryParse(row.SerialNo, out int serialNumber);

                str += FillString(serialNumber.ToString("00000000"), 30);
                str += FillString(row.SupplierID, 10);
                str += FillString(row.QuantitySupplied.ToString("#"), 10);
                if (row.QuantitySupplied == 0)
                {
                    str += FillString("0", 1); // Not supplied
                }
                else if (int.Equals(row.QuantitySupplied, row.Quantity))
                {
                    str += FillString("1", 1);// Supplied with std qty
                }
                else
                {
                    str += FillString("2", 1);// Supplied with non-std qty
                }
                str += FillString("0", 1);// flag
                // add into string array
                resultData.Add(str);
                RowNumber++;
            }
            return resultData;
        }


        public async Task<Result<bool>> CheckEKanbanFulfillable(IEnumerable<string> orderNos)
        {
            foreach (var orderno in orderNos)
            {
                var header = await repository.GetEKanbanHeaderAsync(orderno);
                if (header == null)
                {
                    return new InvalidResult<bool>(new JsonResultError("ErrorRetrievingEKanban__", "orderNo", orderno).ToJson());
                }

                IEnumerable<EKanbanPartsStatusDto> data;
                if (header.Instructions == "EHP")
                {
                    data = await GetEKanbanPartsStatusByOwnershipEHP(orderno);
                }
                else if (header.Instructions == "Supplier")
                {
                    data = await GetEKanbanPartsStatusByOwnership(orderno);
                }
                else if (header.Instructions == "DELJIT")
                {
                    data = await GetEKanbanPartsStatusForCPart(orderno);
                }
                else if (header.Instructions == "SAFETYSTOCK")
                {
                    data = await GetEKanbanPartsStatusForCPartWithoutExt(orderno);
                }
                else
                {
                    data = await GetEKanbanPartsStatusForCPart(orderno);
                }

                if (data.Any(d => d.AvailablePkg + d.ELXAvailablePkg < d.PkgOrdered))
                {
                    return new SuccessResult<bool>(false);
                }
            }
            return new SuccessResult<bool>(true);
        }

        public async Task<Result<EKanbanHeader>> GetEKanbanHeaderForImport(string orderNo, string whsCode)
        {
            var eKanbanHeader = await repository.GetEKanbanHeaderAsync(orderNo);
            if (eKanbanHeader == null)
                return new NotFoundResult<EKanbanHeader>(new JsonResultError("RecordNotFound").ToJson());

            if (eKanbanHeader.Status != (int)EKanbanStatus.New || !String.IsNullOrWhiteSpace(eKanbanHeader.OutJobNo))
            {
                return new InvalidResult<EKanbanHeader>(new JsonResultError("UnableToCancelOrderImported__", "orderNo", orderNo).ToJson());
            }

            if (orderNo.StartsWith("TTK") && whsCode == "RR")//Add the second checking to allow user in Poland can import manual EKanban temporalily
            {
                var ekanbanForVMISuppliers = await GetEKanbanForVMISuppliers(orderNo);
                if (ekanbanForVMISuppliers.Any())
                {
                    return new InvalidResult<EKanbanHeader>(new JsonResultError
                    {
                        MessageKey = "UnableToImportOrderVMISuppliers__",
                        Arguments = new Dictionary<string, string>()
                        {
                            {"orderNo", orderNo },
                            { "suppliers", String.Join(", ", ekanbanForVMISuppliers)}
                        }
                    }.ToJson());
                }
            }
            return new SuccessResult<EKanbanHeader>(eKanbanHeader);
        }

        public async Task<Result<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>> GetSupplierMasterForEKanbanImport(string orderNo, string factoryId, string instruction)
        {
            var orderQuantities = await repository.GetEKanbanDetailDistinctProductCodeList<EKanbanDetailDistinctProductCodeQueryResult>(orderNo, null, null);
            if (!orderQuantities.Any())
            {
                return new InvalidResult<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>(new JsonResultError("NoPartNoFoundForOrder__", "orderNo", orderNo).ToJson());
            }

            foreach (var row in orderQuantities)
            {
                var hasAnyPartMasters = await (from pm in repository.PartMasters()
                                               where pm.CustomerCode == factoryId &&
                                               pm.ProductCode1 == row.ProductCode &&
                                               (instruction == "EHP" || pm.SupplierID == row.SupplierId)
                                               select pm).AnyAsync();

                if (!hasAnyPartMasters)
                    return new InvalidResult<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>(new JsonResultError("ProductCodeNotFoundInPartMaster__", "productCode", row.ProductCode).ToJson());
            }

            var supplierMaster = repository.SupplierMasters()
                .Where(sm => sm.SupplierID == orderQuantities.First().SupplierId && sm.FactoryID == factoryId)
                .FirstOrDefault();

            if (supplierMaster == null && instruction != "EHP")
            {
                return new InvalidResult<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>(new JsonResultError
                {
                    MessageKey = "SupplierMasterNotFound__",
                    Arguments = new Dictionary<string, string>
                    {
                        {"supplierID", orderQuantities.First().SupplierId },
                        {"factoryId",factoryId }
                    }
                }.ToJson());
            }
            return new SuccessResult<Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>>(new Tuple<SupplierMaster, IEnumerable<EKanbanDetailDistinctProductCodeQueryResult>>(supplierMaster, orderQuantities));
        }

        public async Task<Result<IEnumerable<EKanbanPartsStatusDto>>> EKanbanCheck(IEnumerable<string> orderNos)
        {
            IEnumerable<EKanbanPartsStatusDto> data = new EKanbanPartsStatusDto[0];
            foreach (var orderno in orderNos)
            {
                var header = await repository.GetEKanbanHeaderAsync(orderno);
                if (header == null)
                {
                    return new InvalidResult<IEnumerable<EKanbanPartsStatusDto>>(new JsonResultError("ErrorRetrievingEKanban__", "orderNo", orderno).ToJson());
                }

                IEnumerable<EKanbanPartsStatusDto> _data;
                if (header.Instructions == "EHP")
                {
                    _data = await GetEKanbanPartsStatusByOwnershipEHP(orderno);
                }
                else if (header.Instructions == "Supplier")
                {
                    _data = await GetEKanbanPartsStatusByOwnership(orderno);
                }
                else if (header.Instructions == "DELJIT")
                {
                    _data = await GetEKanbanPartsStatusForCPart(orderno);
                }
                else if (header.Instructions == "SAFETYSTOCK")
                {
                    _data = await GetEKanbanPartsStatusForCPartWithoutExt(orderno);
                }
                else
                {
                    _data = await GetEKanbanPartsStatusForCPart(orderno);
                }

                foreach (var item in _data)
                {
                    var availAndELXAvail = item.AvailablePkg + item.ELXAvailablePkg;
                    if (availAndELXAvail <= 0)
                    {
                        item.Status = "UNABLE TO SUPPLY";
                    }
                    else if (availAndELXAvail < item.PkgOrdered)
                    {
                        item.Status = availAndELXAvail == 0 ? "SUPPLY PARTIALLY" : "SUPPLY PARTIALLY (Split)";
                    }
                    else
                    {
                        item.Status = (item.ELXAvailablePkg > 0 && item.ELXAvailablePkg < item.PkgOrdered) ? "OK (Split)" : "OK";
                    }
                }

                data = data.Concat(_data);
            }
            return new SuccessResult<IEnumerable<EKanbanPartsStatusDto>>(data);
        }

        private async Task<IEnumerable<string>> GetEKanbanForVMISuppliers(string orderNo)
        {
            return await (from detail in repository.EKanbanDetails()
                          join sm in repository.SupplierMasters() on detail.SupplierID equals sm.SupplierID
                          where detail.OrderNo == orderNo && sm.SupplyParadigm != null && sm.SupplyParadigm.ToUpper().EndsWith("V")
                          select sm.SupplierID).Distinct().ToListAsync();
        }


        private string FillString(string p_str, int intWidth)
        {
            return p_str?.PadRight(intWidth, ' ');
        }

        private readonly ITTLogixRepository repository;
        private readonly IUtilityService utilityService;
        private readonly AppSettings appSettings;
        private readonly IMapper mapper;
    }
}
