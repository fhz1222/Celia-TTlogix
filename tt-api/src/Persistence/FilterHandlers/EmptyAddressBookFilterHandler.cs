using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class EmptyAddressBookFilterHandler : IFilter<object, TtAddressBook>
{
    public IQueryable<TtAddressBook> GetFilteredTable(AppDbContext context, object? filter)
    {
        var table = context.AddressBooks.AsNoTracking();

        return table;
    }
}
