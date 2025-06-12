using Application.Common.Models;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.RelocationLogs;
using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using Domain.Entities;
using Persistence.Entities;
using Persistence.Extensions;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Extensions;
using Persistence.PetaPoco.Models;
using PetaPoco;
using SqlKata;

namespace Persistence.Repositories
{
    public class RelocationRepository : IRelocationRepository
    {
        private readonly Database dbContext;
        private readonly AppDbContext context;
        private readonly AutoMapper.IMapper mapper;
        public RelocationRepository(IPPDbContextFactory factory, AutoMapper.IMapper mapper, AppDbContext context)
        {
            dbContext = factory.GetInstance();
            this.mapper = mapper;
            this.context = context;
        }

        public async Task AddRelocationLog(RelocationLog relocationLog)
        {
            var item = mapper.Map<TtRelocationLog>(relocationLog);
            context.TtRelocationLog.Add(item);
            await context.SaveChangesAsync();
        }

        public PaginatedList<RelocationLogDto> GetRelocationLogs(PaginationQuery pagination,
            RelocationLogDtoFilter filter, string? orderBy, bool orderByDescending)
        {

            var query = new Query(TT_RelocationLog.SqlTableName)
                .Join(TT_StorageDetail.SqlTableName, j => j.On($"{TT_RelocationLog.SqlTableName}.PID", $"{TT_StorageDetail.SqlTableName}.PID"))
                .Join(TT_PartMaster.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.ProductCode", $"{TT_PartMaster.SqlTableName}.ProductCode1")
                    .On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_PartMaster.SqlTableName}.CustomerCode")
                    .On($"{TT_StorageDetail.SqlTableName}.SupplierId", $"{TT_PartMaster.SqlTableName}.SupplierId"))
                .Join(TT_Customer.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_Customer.SqlTableName}.Code"));

            filter.ApplyFilter(query); 

            query.Select(TT_RelocationLog.SqlTableName + ".{PID, ExternalPID, OldWHSCode, OldLocationCode, NewWHSCode, NewLocationCode, ScannerType, RelocatedBy, RelocatedDate}",
                TT_StorageDetail.SqlTableName + ".{SupplierId, ProductCode, Qty}", 
                TT_Customer.SqlTableName + ".{Code, Name}");

            string orderByColumnName = PrepareOrderByClause(orderBy);

            if (!orderByDescending)
                query.OrderBy(orderByColumnName);
            else
                query.OrderByDesc(orderByColumnName);

            return dbContext.PagedFetch<TT_RelocationLog, TT_StorageDetail, TT_Customer, (TT_RelocationLog, TT_StorageDetail, TT_Customer)>(pagination.PageNumber, pagination.ItemsPerPage
                , (log, storageDetail, customer) => new(log, storageDetail, customer), query)
                .ToPaginatedList(q => mapper.Map<RelocationLogDto>(q.Item1)
                .SetCustomerCode(q.Item3)
                .SetPalletDetails(q.Item2.SupplierId, q.Item2.ProductCode, q.Item2.Qty));
        }

        private string PrepareOrderByClause(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return $"{TT_RelocationLog.SqlTableName}.PID";
            }
            bool addMainTableName = true;
            string orderByResult = orderBy;
            switch (orderBy.ToLower())
            {
                case "pid":
                case "externalpid":
                case "oldwhscode":
                case "newwhscode":
                case "relocatedby":
                case "relocateddate":
                case "scannertype":  break;
                case "oldlocation": orderByResult = "OldLocationCode"; break;
                case "newlocation": orderByResult = "NewLocationCode"; break;
                case "relocationdate": orderByResult = "RelocatedDate"; break;
                case "supplierid":
                case "productcode":
                case "qty": orderByResult = $"{TT_StorageDetail.SqlTableName}.{orderBy}"; addMainTableName = false; break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addMainTableName)
                return orderByResult;
            return $"{TT_RelocationLog.SqlTableName}.{orderByResult}";
        }
    }
}
