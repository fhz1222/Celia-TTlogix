using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using Persistence.PetaPoco.Models;
using SqlKata;

namespace Persistence.Extensions
{
    internal static class RelocationLogDtoFilterExtensions
    {
        internal static Query ApplyFilter(this RelocationLogDtoFilter filter, Query query)
        {

            filter.ApplyRelocationDateFilterIfExists(query);
            filter.ApplyPIDFilterIfExists(query);
            filter.ApplyExternalPIDFilterIfExists(query);
            filter.ApplyProductCodeFilterIfExists(query);
            filter.ApplySupplierIdFilterIfExists(query);
            filter.ApplyQtyFilterIfExists(query);
            filter.ApplyOldWhsCodeFilterIfExists(query);
            filter.ApplyOldLocationFilterIfExists(query);
            filter.ApplyNewWhsCodeFilterIfExists(query);
            filter.ApplyNewLocationFilterIfExists(query);
            filter.ApplyRelocatedByFilterIfExists(query);
            filter.ApplyScannerTypeFilterIfExists(query);
            filter.ApplyCustomerCodeFilterIfExists(query);
            return query;
        }

        internal static Query ApplyRelocationDateFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.RelocationDate?.From != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.RelocatedDate", ">=", filter.RelocationDate.From.Value.Date);
            if (filter.RelocationDate?.To != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.RelocatedDate", "<", filter.RelocationDate.To.Value.Date.AddDays(1));
            return query;

        }
        internal static Query ApplyPIDFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.PID != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.PID", filter.PID);
            return query;
        }
        internal static Query ApplyExternalPIDFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (!string.IsNullOrEmpty(filter.ExternalPID))
                query.WhereLike($"{TT_RelocationLog.SqlTableName}.ExternalPID", $"%{filter.ExternalPID}%");
            return query;
        }

        internal static Query ApplyProductCodeFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.ProductCode != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.ProductCode", $"%{filter.ProductCode}%");
            return query;
        }
        internal static Query ApplySupplierIdFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.SupplierId != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.SupplierId", $"%{filter.SupplierId}%");
            return query;
        }
        internal static Query ApplyQtyFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.Qty?.From != null)
                query.Where($"{TT_StorageDetail.SqlTableName}.Qty", ">=", filter.Qty.From);
            if (filter.Qty?.To != null)
                query.Where($"{TT_StorageDetail.SqlTableName}.Qty", "<=", filter.Qty.To);
            return query;
        }
        internal static Query ApplyOldWhsCodeFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.OldWhsCode != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.OldWHSCode", $"{filter.OldWhsCode}");
            return query;
        }
        internal static Query ApplyOldLocationFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.OldLocation != null)
                query.WhereLike($"{TT_RelocationLog.SqlTableName}.OldLocationCode", $"%{filter.OldLocation}%");
            return query;
        }
        internal static Query ApplyNewWhsCodeFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.NewWhsCode != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.NewWHSCode", $"{filter.NewWhsCode}");
            return query;
        }
        internal static Query ApplyNewLocationFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.NewLocation != null)
                query.WhereLike($"{TT_RelocationLog.SqlTableName}.NewLocationCode", $"%{filter.NewLocation}%");
            return query;
        }
        internal static Query ApplyRelocatedByFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.RelocatedBy != null)
                query.WhereLike($"{TT_RelocationLog.SqlTableName}.RelocatedBy", $"%{filter.RelocatedBy}%");
            return query;
        }
        internal static Query ApplyScannerTypeFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (filter.ScannerType != null)
                query.Where($"{TT_RelocationLog.SqlTableName}.ScannerType", (int) filter.ScannerType);
            return query;
        }
        internal static Query ApplyCustomerCodeFilterIfExists(this RelocationLogDtoFilter filter, Query query)
        {
            if (!String.IsNullOrWhiteSpace(filter.CustomerCode))
                query.Where($"{TT_Customer.SqlTableName}.Code", filter.CustomerCode);
            return query;
        }
    }
}
