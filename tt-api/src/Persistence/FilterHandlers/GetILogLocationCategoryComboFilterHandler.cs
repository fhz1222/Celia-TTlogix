using Application.UseCases.Registration.Queries.GetILogLocationCategoryCombo;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetILogLocationCategoryComboFilterHandler : IFilter<GetILogLocationCategoryComboFilter, ILogLocationCategory>
{
    public IQueryable<ILogLocationCategory> GetFilteredTable(AppDbContext context, GetILogLocationCategoryComboFilter? filter)
    {
        var table = context.ILogLocationCategories.AsNoTracking();

        return table;
    }
}
