using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class EmptyAddressContactFilterHandler : IFilter<object, TtAddressContact>
{
    public IQueryable<TtAddressContact> GetFilteredTable(AppDbContext context, object? filter)
    {
        var table = context.AddressContacts.AsNoTracking();

        return table;
    }
}
