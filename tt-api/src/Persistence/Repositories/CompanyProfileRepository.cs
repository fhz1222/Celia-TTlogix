using Application.Interfaces.Repositories;
using Application.UseCases.CompanyProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class CompanyProfileRepository : ICompanyProfileRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;

    public CompanyProfileRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public bool CompanyProfileDoesNotExistsOrIsInactive(string companyCode)
    {
        var obj = context.CompanyProfiles.Find(companyCode);
        if(obj is null)
            return true;
        return obj.Status == Status.Inactive;
    }

    public void AddNewAddressBook(AddressBook obj)
    {
        var ttbook = mapper.Map<TtAddressBook>(obj);

        context.AddressBooks.Add(ttbook);
    }

    public AddressBook? GetAddressBook(string code)
    {
        var obj = context.AddressBooks.Find(code)
            ?? throw new EntityDoesNotExistException();
        return mapper.Map<AddressBook>(obj);
    }

    public void UpdateAddressBook(AddressBook updated)
    {
        var existing = context.AddressBooks.Find(updated.Code)
            ?? throw new EntityDoesNotExistException();

        mapper.Map(updated, existing);
    }

    public void AddNewAddressContact(AddressContact obj)
    {
        var ttcontact = mapper.Map<TtAddressContact>(obj);

        context.AddressContacts.Add(ttcontact);
    }

    public AddressTreeDto GetAddressTreeDto()
    {
        var cps = context.CompanyProfiles
            .AsNoTracking()
            .ToList()
            .GroupJoin(
                context.AddressBooks.ToList()
                    .GroupJoin(
                        context.AddressContacts.ToList(),
                        ab => ab.Code,
                        ac => ac.AddressCode,
                        (ab, acs) => new AddressTreeDto.AddressTreeBookDto
                        {
                            Code = ab.Code,
                            CompanyCode = ab.CompanyCode,
                            Address1 = ab.Address1,
                            Address2 = ab.Address2,
                            Address3 = ab.Address3,
                            Address4 = ab.Address4,
                            Country = ab.Country,
                            TelNo = ab.TelNo,
                            FaxNo = ab.FaxNo,
                            PostCode = ab.PostCode,
                            Status = (Status)ab.Status,
                            AddressContacts = acs.Select(ac => new AddressTreeDto.AddressTreeContactDto
                            {
                                Code = ac.Code,
                                AddressCode = ac.AddressCode,
                                Name = ac.Name,
                                Email = ac.Email,
                                TelNo = ac.TelNo,
                                FaxNo = ac.FaxNo,
                                Status = (Status)ac.Status
                            })
                        }
                    ),
                cp => cp.Code,
                ab => ab.CompanyCode,
                (cp, abs) => new AddressTreeDto.AddressTreeCompanyProfileDto
                {
                    Code = cp.Code,
                    Name = cp.Name,
                    Status = (Status)cp.Status,
                    AddressBooks = abs
                }
            )
            .ToList();

        return new AddressTreeDto
        {
            CompanyProfiles = cps
        };
    }
}
