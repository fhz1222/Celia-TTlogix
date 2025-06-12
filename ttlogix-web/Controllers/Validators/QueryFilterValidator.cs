using FluentValidation;
using TT.Core.QueryFilters;

namespace TT.Controllers.Validators
{
    public class QueryFilterBaseValidator<T> : AbstractValidator<T> where T : QueryFilterBase
    {
        public QueryFilterBaseValidator()
        {
            RuleFor(f => f.PageNo)
                .GreaterThan(0).WithMessage("PageNoMustBePositive");
            RuleFor(f => f.PageSize)
                .GreaterThan(0).WithMessage("PageSizeMustBePositive");
        }
    }
    public class ASNListQueryFilterValidator : QueryFilterBaseValidator<ASNListQueryFilter> { }
    public class EKanbanListQueryFilterValidator : QueryFilterBaseValidator<EKanbanListQueryFilter> { }
    public class EStockTransferListQueryFilterValidator : QueryFilterBaseValidator<EStockTransferListQueryFilter> { }
    public class InboundListQueryFilterValidator : QueryFilterBaseValidator<InboundListQueryFilter> { }
    public class LoadingListQueryFilterValidator : QueryFilterBaseValidator<LoadingListQueryFilter> { }
    public class OutboundListQueryFilterValidator : QueryFilterBaseValidator<OutboundListQueryFilter> { }
    public class PartMasterListQueryFilterValidator : QueryFilterBaseValidator<PartMasterListQueryFilter> { }
    public class StockTransferListQueryFilterValidator : QueryFilterBaseValidator<StockTransferListQueryFilter> { }
    public class StorageGroupListQueryFilterValidator : QueryFilterBaseValidator<StorageGroupListQueryFilter> { }
    public class UserListQueryFilterValidator : QueryFilterBaseValidator<UserListQueryFilter> { }

}
