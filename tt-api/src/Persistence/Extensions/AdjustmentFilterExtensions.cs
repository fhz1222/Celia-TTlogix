using Application.Common.Enums;
using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using Domain.ValueObjects;
using Persistence.PetaPoco.Models;
using SqlKata;

namespace Persistence.Extensions
{
    internal static class AdjustmentFilterExtensions
    {
        internal static Query ApplyFilter(this AdjustmentFilter filter, Query query)
        {

            query.Where($"{TT_InvAdjustmentMaster.SqlTableName}.WHSCode", filter.WhsCode);

            filter.ApplyCustomerCodeFilterIfExists(query);
            filter.ApplyStatusFilter(query);
            filter.ApplyJobNoFilterIfExists(query);
            filter.ApplyReferenceNoFilterIfExists(query);
            filter.ApplyReasonFilterIfExists(query);
            filter.ApplyJobTypeFilterIfExists(query);
            filter.ApplyCreatedDateFilterIfExists(query);
            return query;
        }

        internal static Query ApplyCustomerCodeFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.CustomerCode != null)
                query.Where($"{TT_InvAdjustmentMaster.SqlTableName}.CustomerCode", filter.CustomerCode);
            return query;

        }

        internal static Query ApplyStatusFilter(this AdjustmentFilter filter, Query query)
        {
            var statusColumnCond = $"{TT_InvAdjustmentMaster.SqlTableName}.Status";
            switch (filter.Status)
            {
                case AdjustmentFilterStatus.New:
                case AdjustmentFilterStatus.Processing:
                case AdjustmentFilterStatus.Completed:
                case AdjustmentFilterStatus.Cancelled: query.Where(statusColumnCond, (byte)filter.Status); break;
                case AdjustmentFilterStatus.Outstanding: query.Where(statusColumnCond, "<", (byte)InventoryAdjustmentStatus.Completed); break;
                case AdjustmentFilterStatus.All: query.Where(statusColumnCond, "<=", (byte)InventoryAdjustmentStatus.Cancelled); break;
            }
            return query;
        }
        internal static Query ApplyJobNoFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.JobNo != null)
                query.WhereLike("JobNo", $"%{filter.JobNo}%");
            return query;
        }

        internal static Query ApplyReferenceNoFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.ReferenceNo != null)
                query.WhereLike("RefNo", $"%{filter.ReferenceNo}%");
            return query;
        }

        internal static Query ApplyReasonFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.Reason != null)
                query.WhereLike("Reason", $"%{filter.Reason}%");
            return query;
        }
        internal static Query ApplyJobTypeFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.JobType != null)
                query.Where("JobType", (byte) filter.JobType);
            return query;
        }
        internal static Query ApplyCreatedDateFilterIfExists(this AdjustmentFilter filter, Query query)
        {
            if (filter.CreatedDate?.From != null)
            {
                query.Where($"{TT_InvAdjustmentMaster.SqlTableName}.CreatedDate", ">=", filter.CreatedDate.From.Value.Date);
            }
            if (filter.CreatedDate?.To != null)
            {
                query.Where($"{TT_InvAdjustmentMaster.SqlTableName}.CreatedDate", "<", filter.CreatedDate.To.Value.Date.AddDays(1));
            }

            return query;
        }
    }
}
