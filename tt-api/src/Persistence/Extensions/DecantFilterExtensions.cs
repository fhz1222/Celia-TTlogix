using Application.Common.Enums;
using Application.UseCases.Decants.Queries.GetDecants;
using Domain.ValueObjects;
using Persistence.PetaPoco.Models;
using SqlKata;

namespace Persistence.Extensions
{
    internal static class DecantFilterExtensions
    {
        internal static Query ApplyFilter(this DecantDtoFilter filter, Query query)
        {

            query.Where($"{TT_Decant.SqlTableName}.WHSCode", filter.WhsCode);

            filter.ApplyCustomerCodeFilterIfExists(query);
            filter.ApplyStatusFilter(query);
            filter.ApplyJobNoFilterIfExists(query);
            filter.ApplyReferenceNoFilterIfExists(query);
            filter.ApplyRemarkFilterIfExists(query);
            filter.ApplyCreatedDateFilterIfExists(query);
            return query;
        }

        internal static Query ApplyCustomerCodeFilterIfExists(this DecantDtoFilter filter, Query query)
        {
            if (filter.CustomerCode != null)
                query.Where($"{TT_Decant.SqlTableName}.CustomerCode", filter.CustomerCode);
            return query;

        }

        internal static Query ApplyStatusFilter(this DecantDtoFilter filter, Query query)
        {
            var statusColumnCond = $"{TT_Decant.SqlTableName}.Status";
            switch (filter.Status)
            {
                case DecantFilterStatus.New:
                case DecantFilterStatus.Processing:
                case DecantFilterStatus.Completed:
                case DecantFilterStatus.Cancelled: query.Where(statusColumnCond, (byte)filter.Status); break;
                case DecantFilterStatus.Outstanding: query.Where(statusColumnCond, "<", (byte)DecantStatus.Completed); break;
                case DecantFilterStatus.All: query.Where(statusColumnCond, "<=", (byte)DecantStatus.Cancelled); break;
            }
            return query;
        }
        internal static Query ApplyJobNoFilterIfExists(this DecantDtoFilter filter, Query query)
        {
            if (filter.JobNo != null)
                query.WhereLike("JobNo", $"%{filter.JobNo}%");
            return query;
        }

        internal static Query ApplyReferenceNoFilterIfExists(this DecantDtoFilter filter, Query query)
        {
            if (filter.ReferenceNo != null)
                query.WhereLike("RefNo", $"%{filter.ReferenceNo}%");
            return query;
        }

        internal static Query ApplyRemarkFilterIfExists(this DecantDtoFilter filter, Query query)
        {
            if (filter.Remark != null)
                query.WhereLike("Remark", $"%{filter.Remark}%");
            return query;
        }

        internal static Query ApplyCreatedDateFilterIfExists(this DecantDtoFilter filter, Query query)
        {
            if (filter.CreatedDate?.From != null)
            {
                query.Where($"{TT_Decant.SqlTableName}.CreatedDate", ">=", filter.CreatedDate.From.Value.Date);
            }
            if (filter.CreatedDate?.To != null)
            {
                query.Where($"{TT_Decant.SqlTableName}.CreatedDate", "<", filter.CreatedDate.To.Value.Date.AddDays(1));
            }

            return query;
        }
    }
}
