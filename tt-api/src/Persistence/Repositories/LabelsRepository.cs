using Application.Interfaces.Repositories;
using Application.UseCases.Labels;
using Persistence.PetaPoco;
using PetaPoco;

namespace Persistence.Repositories
{
    public class LabelsRepository : ILabelRepository
    {
        private readonly Database dbContext;
        private readonly AutoMapper.IMapper mapper;
        public LabelsRepository(IPPDbContextFactory factory, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.mapper = mapper;
        }

        public VmiLabelDto GetVmiLabel(string Pid)
        {
            var result = new VmiLabelDto();

            result.Receiver = "ELECTROLUX SUSEGANA";
            result.AdviceNoteNo = "4618138-043610125489554-4618138";
            result.SupNameAndAddress = "TECHNO PLAST CUGIR SRL, -";
            result.GrossWeight = "700";
            result.PartNo = "A11905207";
            result.Qty = "88";
            result.Description = "Can Assy 050 wTrim Metal-look";
            result.SupplierCode = "04361025654";
            result.SerialNo = "060700049";
            result.Date = "P000101";
            result.Pid = "TESAG201507000B1E";

            return result;
        /*var query = new Query(TT_RelocationLog.SqlTableName)
            .Join(TT_StorageDetail.SqlTableName, j => j.On($"{TT_RelocationLog.SqlTableName}.PID", $"{TT_StorageDetail.SqlTableName}.PID"))
            .Join(TT_PartMaster.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.ProductCode", $"{TT_PartMaster.SqlTableName}.ProductCode1")
                .On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_PartMaster.SqlTableName}.CustomerCode")
                .On($"{TT_StorageDetail.SqlTableName}.SupplierId", $"{TT_PartMaster.SqlTableName}.SupplierId"))
            .Join(TT_Customer.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_Customer.SqlTableName}.Code"));

        //filter.ApplyFilter(query); 

        query.Select(TT_RelocationLog.SqlTableName + ".{PID, ExternalPID, OldWHSCode, OldLocationCode, NewWHSCode, NewLocationCode, ScannerType, RelocatedBy, RelocatedDate}",
            TT_StorageDetail.SqlTableName + ".{SupplierId, ProductCode, Qty}", 
            TT_Customer.SqlTableName + ".{Code, Name}");*/

        //string orderByColumnName = PrepareOrderByClause(orderBy);

        /*if (!orderByDescending)
            query.OrderBy(orderByColumnName);
        else
            query.OrderByDesc(orderByColumnName);*/

        /*return dbContext.PagedFetch<TT_RelocationLog, TT_StorageDetail, TT_Customer, (TT_RelocationLog, TT_StorageDetail, TT_Customer)>(pagination.PageNumber, pagination.ItemsPerPage
            , (log, storageDetail, customer) => new(log, storageDetail, customer), query)
            .ToPaginatedList(q => mapper.Map<RelocationLogDto>(q.Item1)
            .SetCustomerCode(q.Item3)
            .SetPalletDetails(q.Item2.SupplierId, q.Item2.ProductCode, q.Item2.Qty));*/
    }

        /*private string PrepareOrderByClause(string? orderBy)
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
                case "supplierid":
                case "productcode":
                case "qty": orderByResult = $"{TT_StorageDetail.SqlTableName}.{orderBy}"; addMainTableName = false; break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addMainTableName)
                return orderByResult;
            return $"{TT_RelocationLog.SqlTableName}.{orderByResult}";
        }*/
    }
}
