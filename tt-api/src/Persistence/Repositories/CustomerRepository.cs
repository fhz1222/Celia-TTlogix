using Application.Interfaces.Repositories;
using Application.UseCases.Customer;
using Application.UseCases.Customer.Queries.GetCustomerClientList;
using Application.UseCases.Customer.Queries.GetCustomerList;
using Application.UseCases.Customer.Queries.GetUomDecimalList;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
namespace Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;
    public CustomerRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public IEnumerable<CustomerDto> GetCustomers()
    {
        var customers = context.TtCustomers;

        return mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public List<CustomerListItemDto> GetCustomerList(GetCustomerListDtoFilter filter, string? orderBy, bool orderByDescending)
    {
        var query = context.TtCustomers
            .AsNoTracking()
            .Where(x => x.Whscode == filter.WhsCode)
            .OptionalWhere((byte?)filter.Status, status => x => x.Status == status)
            .OptionalWhere(filter.Code, code => x => x.Code == code)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .GroupJoin(
                context.AddressContacts,
                c => c.Pic1,
                ac => ac.Code,
                (c, acs) => new { c, acs })
            .SelectMany(
                set => set.acs.DefaultIfEmpty(),
                (c, ac) => new CustomerListItemDto
                {
                    Code = c.c.Code,
                    Name = c.c.Name,
                    ContactPerson = ac != null ? ac.Name : null,
                    TelephoneNo = ac != null ? ac.TelNo : null,
                    FaxNo = ac != null ? ac.FaxNo : null,
                    Status = (CustomerStatus)c.c.Status
                })
            .OptionalWhere(filter.ContactPerson, person => x => x.ContactPerson == person)
            .OptionalWhere(filter.TelephoneNo, tel => x => x.TelephoneNo == tel)
            .OptionalWhere(filter.FaxNo, fax => x => x.FaxNo == fax);

        orderBy ??= "code";
        var querableResult = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "code" ? i.Code :
                orderBy.ToLower() == "name" ? i.Name :
                orderBy.ToLower() == "contactperson" ? i.ContactPerson :
                orderBy.ToLower() == "telephoneno" ? i.TelephoneNo :
                orderBy.ToLower() == "faxno" ? i.FaxNo :
                orderBy.ToLower() == "status" ? i.Status :
                i.Code);

        var items = querableResult
            .ToList();

        return items;
    }

    public Customer? TryGetCustomer(string code, string whsCode)
    {
        var obj = context.TtCustomers.Find(code, whsCode);
        if(obj is null)
            return null;
        return mapper.Map<Customer>(obj);
    }

    public void AddNew(Customer customer)
    {
        var ttcustomer = mapper.Map<TtCustomer>(customer);

        context.TtCustomers.Add(ttcustomer);
    }

    public void Update(Customer updated)
    {
        var existing = context.TtCustomers.Find(updated.Code, updated.Whscode)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public bool AddressBookDoesNotExistsOrIsInactive(string companyCode, string primaryAddress)
    {
        var activeStatusValue = (byte)Status.Active;
        var found = context.AddressBooks
            .AsNoTracking()
            .Where(x => x.Code == primaryAddress)
            .Where(x => x.Status == activeStatusValue)
            .Where(x => x.CompanyCode == companyCode)
            .Any();
        return !found;
    }

    public bool PICDoesNotExistsOrIsInactive(string primaryAddress, string pic)
    {
        var activeStatusValue = (byte)Status.Active;
        var found = context.AddressContacts
            .AsNoTracking()
            .Where(x => x.Code == pic)
            .Where(x => x.Status == activeStatusValue)
            .Where(x => x.AddressCode == primaryAddress)
            .Any();
        return !found;
    }

    public T GetInventoryControl<T>(string customerCode) where T : class
    {
        var obj = context.InventoryControls.Find(customerCode)
            ?? throw new EntityDoesNotExistException();
        return mapper.Map<T>(obj);
    }

    public T? TryGetInventoryControl<T>(string customerCode) where T : class
    {
        var obj = context.InventoryControls.Find(customerCode);
        return mapper.Map<T?>(obj);
    }

    public void AddNewInventoryControl(InventoryControl obj)
    {
        var ttinvctrl = mapper.Map<TtInventoryControl>(obj);

        context.InventoryControls.Add(ttinvctrl);
    }

    public void UpdateInventoryControl(InventoryControl updated)
    {
        var existing = context.InventoryControls.Find(updated.CustomerCode)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public List<UomDecimalListItemDto> GetUomDecimalList(GetUomDecimalListDtoFilter filter, string? orderBy, bool orderByDescending)
    {
        byte? statusValue = filter.Status != null ? ((byte)filter.Status) : null;

        var query = context.TtUOMDecimals
            .AsNoTracking()
            .Where(x => x.CustomerCode == filter.CustomerCode)
            .OptionalWhere(filter.DecimalNum, range => x => x.DecimalNum >= range.From && x.DecimalNum <= range.To)
            .OptionalWhere(statusValue, status => x => x.Status == status)
            .GroupJoin(
                context.TtUOM,
                dec => dec.UOM,
                uom => uom.Code,
                (dec, uoms) => new { dec, uoms })
            .SelectMany(
                set => set.uoms.DefaultIfEmpty(),
                (dec, uom) => new
                {
                    dec.dec.CustomerCode,
                    Uom = uom != null ? uom.Code : "",
                    Name = uom != null ? uom.Name : "",
                    dec.dec.DecimalNum,
                    dec.dec.Status,
                })
            .OptionalWhere(filter.Name, name => x => EF.Functions.Like(x.Name, name.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR));

        orderBy ??= "name";
        var querableResult = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "uom" ? i.Uom :
                orderBy.ToLower() == "name" ? i.Name :
                orderBy.ToLower() == "decimalnum" ? i.DecimalNum :
                orderBy.ToLower() == "status" ? i.Status :
                i.Name);

        var items = querableResult
            .Select(x => new UomDecimalListItemDto
            {
                CustomerCode = x.CustomerCode,
                Uom = x.Uom,
                Name = x.Name,
                DecimalNum = x.DecimalNum,
                Status = (UomDecimalStatus)x.Status,
            })
            .ToList();

        return items;
    }

    public UomDecimal? TryGetUomDecimal(string customerCode, string uom)
    {
        var obj = context.TtUOMDecimals.Find(customerCode, uom);
        return mapper.Map<UomDecimal?>(obj);
    }

    public void AddNewUomDecimal(UomDecimal obj)
    {
        var ttuomdecimal = mapper.Map<TtUOMDecimal>(obj);

        context.TtUOMDecimals.Add(ttuomdecimal);
    }

    public void UpdateUomDecimal(UomDecimal updated)
    {
        var existing = context.TtUOMDecimals.Find(updated.CustomerCode, updated.UOM)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public bool UomExists(string uom)
    {
        var entity = context.TtUOM.Find(uom);
        return entity != null;
    }

    public List<CustomerClientListItemDto> GetCustomerClientList(GetCustomerClientListDtoFilter filter, string? orderBy, bool orderByDescending)
    {
        byte? statusValue = filter.Status != null ? ((byte)filter.Status) : null;

        var query = context.CustomerClients
            .AsNoTracking()
            .Where(x => x.CustomerCode == filter.CustomerCode)
            .OptionalWhere(statusValue, status => x => x.Status == status)
            .OptionalWhere(filter.Code, code => x => x.Code == code)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .GroupJoin(
                context.AddressContacts,
                c => c.Pic1,
                ac => ac.Code,
                (c, acs) => new { c, acs })
            .SelectMany(
                set => set.acs.DefaultIfEmpty(),
                (c, ac) => new CustomerClientListItemDto
                {
                    Code = c.c.Code,
                    Name = c.c.Name,
                    ContactPerson = ac != null ? ac.Name : null,
                    TelephoneNo = ac != null ? ac.TelNo : null,
                    FaxNo = ac != null ? ac.FaxNo : null,
                    Status = (CustomerClientStatus)c.c.Status
                })
            .OptionalWhere(filter.ContactPerson, person => x => x.ContactPerson == person)
            .OptionalWhere(filter.TelephoneNo, tel => x => x.TelephoneNo == tel)
            .OptionalWhere(filter.FaxNo, fax => x => x.FaxNo == fax);

        orderBy ??= "code";
        var querableResult = query
            .OrderByDescOrAsc(orderByDescending, (i) =>
                orderBy.ToLower() == "code" ? i.Code :
                orderBy.ToLower() == "name" ? i.Name :
                orderBy.ToLower() == "contactperson" ? i.ContactPerson :
                orderBy.ToLower() == "telephoneno" ? i.TelephoneNo :
                orderBy.ToLower() == "faxno" ? i.FaxNo :
                orderBy.ToLower() == "status" ? i.Status :
                i.Code);

        var items = querableResult
            .ToList();

        return items;
    }

    public CustomerClient GetCustomerClient(string code)
    {
        var obj = context.CustomerClients.Find(code)
            ?? throw new EntityDoesNotExistException();
        return mapper.Map<CustomerClient>(obj);
    }

    public CustomerClient? TryGetCustomerClient(string code)
    {
        var obj = context.CustomerClients.Find(code);
        return mapper.Map<CustomerClient?>(obj);
    }

    public void AddNewCustomerClient(CustomerClient obj)
    {
        var ttcustomerclient = mapper.Map<TtCustomerClient>(obj);

        context.CustomerClients.Add(ttcustomerclient);
    }

    public void UpdateCustomerClient(CustomerClient updated)
    {
        var existing = context.CustomerClients.Find(updated.Code)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }
}
