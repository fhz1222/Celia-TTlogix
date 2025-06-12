using Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;
using Microsoft.EntityFrameworkCore;

namespace Persistence.FilterHandlers.Delivery;

class GetDeliveryCustomerClientListFilterHandler : IFilter<GetDeliveryCustomerClientListFilter, DeliveryCustomerClient>
{
    public IQueryable<DeliveryCustomerClient> GetFilteredTable(AppDbContext context, GetDeliveryCustomerClientListFilter? filter)
    {
        var table = context.CustomerClients.AsNoTracking()
            .Join(
                context.AddressBooks,
                cc => cc.ShippingAddress,
                ab => ab.Code,
                (cc, ab) => new DeliveryCustomerClient
                {
                    Code = cc.Code,
                    CustomerCode = cc.CustomerCode,
                    Name = cc.Name,
                    Status = ab.Status,
                    Address1 = ab.Address1,
                    Address2 = ab.Address2,
                    Address3 = ab.Address3,
                    Address4 = ab.Address4,
                    PostCode = ab.PostCode,
                    Country = ab.Country
                }
            );

        if (filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.CustomerCode, customerCode => x => x.CustomerCode == customerCode)
            .OptionalWhere((byte)filter.Status, status => x => x.Status == status);

        return query;
    }
}
