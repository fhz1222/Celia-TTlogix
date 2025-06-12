namespace Persistence.FilterHandlers;

interface IFilter<F, T>
{
    public IQueryable<T> GetFilteredTable(AppDbContext context, F? filter);
}
