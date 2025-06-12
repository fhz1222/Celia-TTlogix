using Application.UseCases.Registration.Queries.GetLabelPrinterList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetLabelPrinterListDtoFilterHandler : IFilter<GetLabelPrinterListDtoFilter, TtLabelPrinter>
{
    public IQueryable<TtLabelPrinter> GetFilteredTable(AppDbContext context, GetLabelPrinterListDtoFilter? filter)
    {
        var table = context.LabelPrinters.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Ip, ip => x => x.Ip == ip)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .OptionalWhere(filter.Type, type => x => x.Type == type)
            .OptionalWhere(filter.Description, desc => x => x.Description == desc);

        return query;
    }
}
