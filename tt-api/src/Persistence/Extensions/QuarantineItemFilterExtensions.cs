using Application.Common.Enums;
using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using Application.UseCases.Quarantine.Queries.GetQuarantineItems;
using Domain.ValueObjects;
using Persistence.PetaPoco.Models;
using SqlKata;

namespace Persistence.Extensions
{
    internal static class QuarantineItemFilterExtensions
    {
        internal static Query ApplyFilter(this QuarantineItemDtoFilter filter, Query query)
        {

            query.Where($"{TT_StorageDetail.SqlTableName}.WHSCode", filter.WhsCode);

            filter.ApplyCustomerCodeFilterIfExists(query);
            filter.ApplySupplierCodeFilterIfExists(query);
            filter.ApplyProductCodeFilterIfExists(query);
            filter.ApplyPIDFilterIfExists(query);
            filter.ApplyReasonFilterIfExists(query);
            filter.ApplyLocationFilterIfExists(query);
            filter.ApplyCreatedByFilterIfExists(query);
            filter.ApplyQuarantineDateFilterIfExists(query);
            filter.ApplyQtyFilterIfExists(query);
            filter.ApplyDecimalNumFilterIfExists(query);
            return query;
        }

        internal static Query ApplyCustomerCodeFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.CustomerCode != null)
                query.Where($"{TT_StorageDetail.SqlTableName}.CustomerCode", filter.CustomerCode);
            return query;
        }

        internal static Query ApplySupplierCodeFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.SupplierId != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.SupplierId", $"%{filter.SupplierId}%");
            return query;
        }

        internal static Query ApplyProductCodeFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.ProductCode != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.ProductCode", $"%{filter.ProductCode}%");
            return query;
        }

        internal static Query ApplyLocationFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.Location != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.LocationCode", $"%{filter.Location}%");
            return query;
        }

        internal static Query ApplyPIDFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.PID != null)
                query.WhereLike($"{TT_StorageDetail.SqlTableName}.PID", $"%{filter.PID}%");
            return query;
        }

        internal static Query ApplyReasonFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.Reason != null)
                query.WhereLike($"{TT_QuarantineReason.SqlTableName}.Reason", $"%{filter.Reason}%");
            return query;
        }

        internal static Query ApplyCreatedByFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.CreatedBy != null)
                query.WhereLike("Quarantine.CreatedBy", $"%{filter.CreatedBy}%");
            return query;
        }

        internal static Query ApplyQuarantineDateFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.QuarantineDate?.From != null)
            {
                query.Where("Quarantine.CreatedDate", ">=", filter.QuarantineDate.From.Value.Date);
            }
            if (filter.QuarantineDate?.To != null)
            {
                query.Where("Quarantine.CreatedDate", "<", filter.QuarantineDate.To.Value.Date.AddDays(1));
            }
            return query;
        }

        internal static Query ApplyQtyFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.Qty?.From != null)
            {
                query.Where($"{TT_StorageDetail.SqlTableName}.Qty", ">=", filter.Qty.From);
            }
            if (filter.Qty?.To != null)
            {
                query.Where($"{TT_StorageDetail.SqlTableName}.Qty", "<=", filter.Qty.To);
            }
            return query;
        }
        internal static Query ApplyDecimalNumFilterIfExists(this QuarantineItemDtoFilter filter, Query query)
        {
            if (filter.DecimalNum?.From != null)
            {
                query.WhereRaw($"ISNULl(DecimalNum, 0) >= {filter.DecimalNum.From}");
                
            }
            if (filter.DecimalNum?.To != null)
            {
                query.WhereRaw($"ISNULL(DecimalNum, 0) <= {filter.DecimalNum.To}");
            }
            return query;
        }
    }
}
