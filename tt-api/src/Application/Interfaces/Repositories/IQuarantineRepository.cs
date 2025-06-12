using Application.Common.Models;
using Application.UseCases.Quarantine;
using Application.UseCases.Quarantine.Queries.GetQuarantineItems;
using Application.UseCases.Quarantine.Queries.GetQuarantinePalletsInILog;

namespace Application.Interfaces.Repositories;

public interface IQuarantineRepository
{
    PaginatedList<QuarantineItemDto> GetQuarantineItems(PaginationQuery pagination, QuarantineItemDtoFilter filter, string? orderBy, bool orderByDescending);
    IEnumerable<QuarantinePalletDto> GetQuarantinePalletsOnLocationCategory(string jobNo, int locationCategoryId);
    string? GetQuarantineReason(string pid);
    Task SetQuarantineReason(string pid, string reason, DateTime createdDate);
}
