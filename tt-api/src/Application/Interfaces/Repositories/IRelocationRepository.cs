using Application.Common.Models;
using Application.UseCases.RelocationLogs;
using Application.UseCases.RelocationLogs.Queries.GetRelocationLogs;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IRelocationRepository
{
    PaginatedList<RelocationLogDto> GetRelocationLogs(PaginationQuery pagination, RelocationLogDtoFilter filter, string? orderBy, bool orderByDescending);
    Task AddRelocationLog(RelocationLog relocationLog);
}
