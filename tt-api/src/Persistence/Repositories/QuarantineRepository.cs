using Application.Common.Models;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Quarantine;
using Application.UseCases.Quarantine.Queries.GetQuarantineItems;
using Application.UseCases.Quarantine.Queries.GetQuarantinePalletsInILog;
using Domain.ValueObjects;
using Persistence.Entities;
using Persistence.Extensions;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Extensions;
using Persistence.PetaPoco.Models;
using PetaPoco;
using PetaPoco.Extensions;
using PetaPoco.SqlKata;
using SqlKata;

namespace Persistence.Repositories
{
    public class QuarantineRepository : IQuarantineRepository
    {
        private readonly Database dbContext;
        private readonly AppDbContext appDbContext;
        private readonly AutoMapper.IMapper mapper;
        public QuarantineRepository(IPPDbContextFactory factory, AppDbContext appDbContext, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.appDbContext = appDbContext;
            this.mapper = mapper;
        }

        public PaginatedList<QuarantineItemDto> GetQuarantineItems(PaginationQuery pagination, QuarantineItemDtoFilter filter, string? orderBy, bool orderByDescending)
        {
            var latestQuarantineLogQuery = new Query(TT_QuarantineLog.SqlTableName)
                .Where("Flag", 1)
                .Where("Act", 0)
                .GroupBy("PID")
                .Select("PID")
                .SelectRaw("Max(JobNo) as JobNo");

            var quarantineLogQuery = new Query(TT_QuarantineLog.SqlTableName)
                .Join(latestQuarantineLogQuery.As("Latest")
                    , j => j.On($"{TT_QuarantineLog.SqlTableName}.JobNo", "Latest.JobNo")
                    .On($"{TT_QuarantineLog.SqlTableName}.PID", "Latest.PID"))
                .Select(TT_QuarantineLog.SqlTableName + ".{*}");

            dbContext.Fetch<TT_QuarantineLog>(quarantineLogQuery.ToSql());

            var query = new Query(TT_StorageDetail.SqlTableName)
                .Join(TT_Customer.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.CustomerCode", "Code")
                    .On($"{TT_StorageDetail.SqlTableName}.WHSCode", $"{TT_Customer.SqlTableName}.WHSCode"))
                .Join(TT_PartMaster.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.ProductCode", $"{TT_PartMaster.SqlTableName}.ProductCode1")
                    .On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_PartMaster.SqlTableName}.CustomerCode")
                    .On($"{TT_StorageDetail.SqlTableName}.SupplierId", $"{TT_PartMaster.SqlTableName}.SupplierId"))
                .LeftJoin(TT_UOMDecimal.SqlTableName, j => j.On($"{TT_PartMaster.SqlTableName}.CustomerCode", $"{TT_UOMDecimal.SqlTableName}.CustomerCode")
                    .On($"{TT_PartMaster.SqlTableName}.UOM", $"{TT_UOMDecimal.SqlTableName}.UOM")
                    .Where($"{TT_UOMDecimal.SqlTableName}.Status", 1))
                .LeftJoin(TT_QuarantineReason.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.PID", $"{TT_QuarantineReason.SqlTableName}.PID"))
                .LeftJoin(quarantineLogQuery.As("Quarantine")
                    , j => j.On($"{TT_StorageDetail.SqlTableName}.PID", "Quarantine.PID"))
                .Where($"{TT_StorageDetail.SqlTableName}.Status", (byte)StorageStatus.Quarantine);

            filter.ApplyFilter(query);

            query.Select(TT_StorageDetail.SqlTableName + ".{PID, CustomerCode, SupplierId, WhsCode, ProductCode, LocationCode, Qty}",
                   TT_Customer.SqlTableName + ".{Name}",
                   TT_UOMDecimal.SqlTableName + ".{DecimalNum}",
                   TT_QuarantineReason.SqlTableName + ".{Reason}",
                   "Quarantine.{CreatedBy, CreatedDate}");

            string orderByColumnName = PrepareOrderByClause(orderBy);

            if (!orderByDescending)
                query.OrderBy(orderByColumnName);
            else
                query.OrderByDesc(orderByColumnName);

            return dbContext.PagedFetch<TT_StorageDetail, TT_Customer, TT_UOMDecimal, TT_QuarantineReason, TT_QuarantineLog
                ,(TT_StorageDetail, TT_Customer, TT_UOMDecimal, TT_QuarantineReason, TT_QuarantineLog)>(pagination.PageNumber, pagination.ItemsPerPage
                ,(storageDetail, customer, uomDecimal, reason, qLog) => new(storageDetail, customer, uomDecimal, reason, qLog), query)
                .ToPaginatedList(q => mapper.Map(q.Item5, mapper.Map<QuarantineItemDto>(q.Item1))
                    .SetAdditionalFields(q.Item2.Name, q.Item4.Reason, q.Item3.DecimalNum));
        }

        public string? GetQuarantineReason(string pid)
        {
            var query = new Query(TT_StorageDetail.SqlTableName)
                .LeftJoin(TT_QuarantineReason.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.PID", $"{TT_QuarantineReason.SqlTableName}.PID"))
                .Where($"{TT_StorageDetail.SqlTableName}.Status", (byte)StorageStatus.Quarantine)
                .Where($"{TT_StorageDetail.SqlTableName}.PID", pid)
                .Select("Reason");

            return dbContext.FirstOrDefault<string?>(query.ToSql());
        }

        public async Task SetQuarantineReason(string pid, string reason, DateTime createdDate)
        {
            var currentReason = await appDbContext.FindAsync<TtQuarantineReason>(pid);
            if (currentReason != null)
            {
                currentReason.Reason = reason;
                currentReason.CreatedDate = createdDate;
            }
            else
            {
                appDbContext.TtQuarantineReasons.Add(new TtQuarantineReason
                {
                    Pid = pid,
                    Reason = reason,
                    CreatedDate = createdDate
                });
            }
        }

        public IEnumerable<QuarantinePalletDto> GetQuarantinePalletsOnLocationCategory(string jobNo, int locationCategoryId)
        {
            var query = new Query(TT_QuarantineLog.SqlTableName)
                .Join(TT_StorageDetail.SqlTableName, j => j.On($"{TT_QuarantineLog.SqlTableName}.PID", $"{TT_StorageDetail.SqlTableName}.PID"))
                .Join("TT_Location", j => j.On($"{TT_StorageDetail.SqlTableName}.LocationCode", "TT_Location.Code").On($"{TT_StorageDetail.SqlTableName}.WHSCode", "TT_Location.WHSCode"))
                .Where($"{TT_QuarantineLog.SqlTableName}.JobNo", jobNo)
                .Where("TT_Location.ILogLocationCategoryId", locationCategoryId)
                .Select($"{TT_QuarantineLog.SqlTableName}.PID", $"{TT_QuarantineLog.SqlTableName}.Act")
                .ToSql();

            return dbContext.Query<TT_QuarantineLog>(query).Select(q => new QuarantinePalletDto() { Pid = q.PID, IsOnHold = q.Act == 0 });
        }

        private string PrepareOrderByClause(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return $"{TT_StorageDetail.SqlTableName}.PID";
            }
            bool addStorageDetail = true;
            string orderByResult = orderBy;
            switch (orderBy.ToLower())
            {
                case "customername": orderByResult = $"{TT_Customer.SqlTableName}.Name"; addStorageDetail = false; break;
                case "whscode":
                case "pid":
                case "customercode":
                case "supplierid":
                case "productcode":
                case "qty": break;
                case "location": orderByResult = "LocationCode"; break;
                case "decimalnum": addStorageDetail = false; break;
                case "reason": orderByResult = $"{TT_QuarantineReason.SqlTableName}.Reason"; addStorageDetail = false; break;
                case "createdby": orderByResult = $"Quarantine.CreatedBy"; addStorageDetail = false; break;
                case "quarantinedate": orderByResult = $"Quarantine.CreatedDate"; addStorageDetail = false; break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addStorageDetail)
                return orderByResult;
            return $"{TT_StorageDetail.SqlTableName}.{orderByResult}";
        }
    }
}
