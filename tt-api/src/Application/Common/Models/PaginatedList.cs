namespace Application.Common.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public Pagination Pagination { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        Pagination = new Pagination()
        {
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            TotalCount = count,
            ItemsPerPage = pageSize
        };
    }
}