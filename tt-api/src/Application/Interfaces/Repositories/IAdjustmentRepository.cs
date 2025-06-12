using Application.Common.Models;
using Application.Interfaces.Utils;
using Application.UseCases.Adjustments.Queries.GetAdjustmentsQuery;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface IAdjustmentRepository : IJobNumberSource
{
    PaginatedList<Adjustment> GetAdjustments(AdjustmentFilter filter, PaginationQuery pagination, string? orderBy, bool orderByDescending);
    Adjustment? GetAdjustmentDetails(string jobNo);
    int GetLastJobNumber(string prefix);
    Task<string> AddNewAdjustment(string whsCode, string customerCode, string userCode, InventoryAdjustmentJobType jobType, InventoryAdjustmentStatus status, string jobNo, CancellationToken cancellationToken);
    Task Update(Adjustment adjustment);
}
