using Application.Common.Models;
using Application.Interfaces.Utils;
using Application.UseCases.Decants;
using Application.UseCases.Decants.Queries.GetDecants;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDecantRepository : IJobNumberSource
{
    PaginatedList<DecantDto> GetDecants(DecantDtoFilter filter, PaginationQuery pagination, string? orderBy, bool orderByDescending);
    Task<Decant?> GetDecant(string jobNo);
    Task UpdateDecant(Decant updated);
    Task AddNewDecant(Decant newObject);
    Task DeleteDecantItem(string jobNo, DecantItem decantItem);
    Task AddDecantItem(string jobNo, string userCode, DecantItem decantItem);
    Task UpdateDecantItemOnComplete(string jobNo, DecantItem decantItem);
}

