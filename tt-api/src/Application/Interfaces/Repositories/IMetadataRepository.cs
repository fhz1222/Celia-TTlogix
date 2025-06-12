using Application.Common.Models;
using Application.UseCases.Common;
using Domain.Metadata;

namespace Application.Interfaces.Repositories
{
    public interface IMetadataRepository
    {
        T? Get<T>(EntityType entityType, params string[] key);
        void Remove(EntityType entityType, string[] key);
        void AddNew<T>(EntityType entityType, T obj) where T : class;
        void Update<T>(EntityType entityType, T updated, params string[] key);
        void AddNewWithMetadata<T>(EntityType entityType, T obj, Metadata metadata) where T : class;
        void UpdateWithMetadata<T>(EntityType entityType, T updated, Metadata metadata, string[] key) where T : class;
        PaginatedList<T> GetPaginatedList<F, T>(EntityType entityType, F? filter, OrderBy? sorting, PaginationQuery? pagination);
    }
}