using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Storage.Queries.GetPidsInILog;

public class GetPidsInILogQuery : IRequest<List<string>>
{
    public string[] Pids { get; set; } = default!;
}

public class GetPidsInILogQueryHandler : IRequestHandler<GetPidsInILogQuery, List<string>>
{
    private readonly IRepository repository;

    public GetPidsInILogQueryHandler(IRepository repository) 
        => this.repository = repository;

    public async Task<List<string>> Handle(GetPidsInILogQuery r, CancellationToken cancellationToken)
    {
        var warehouses = repository.ILogIntegrationRepository.GetWarehouses();
        var categoryId = repository.Locations.GetILogStorageLocationCategoryId();
        var results = repository.StorageDetails.GetPidsInILog(r.Pids, warehouses, categoryId);
        return await Task.FromResult(results);
    }
}
