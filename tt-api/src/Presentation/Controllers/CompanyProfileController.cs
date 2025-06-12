using Application.UseCases.CompanyProfiles;
using Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Presentation.Common;
using Presentation.Configuration;
using Application.UseCases.Common.Commands.ToggleActiveCommand;
using Application.UseCases.Common.Commands.UpsertCommand;
using Application.UseCases.Common;
using Application.UseCases.Common.Queries.GetListQuery;
using Application.UseCases.CompanyProfiles.Queries.GetAddressTree;
using Application.UseCases.CompanyProfiles.Commands.AddAddressBook;
using Application.UseCases.CompanyProfiles.Commands.AddAddressContact;
using Application.UseCases.Common.Commands.UpdateCommand;

namespace Presentation.Controllers;
/// <summary>
/// CompanyProfileController provides method to get the CompanyProfile, AddressBook, AddressContact
/// </summary>
public partial class CompanyProfileController : ApiControllerBase
{
    private readonly IMapper mapper;
    private readonly IFeatureManager manager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="manager"></param>
    public CompanyProfileController(IMapper mapper, IFeatureManager manager)
    {
        this.mapper = mapper;
        this.manager = manager;
    }

    [HttpGet("isActive")]
    public async Task<bool> IsActive()
        => await manager.IsEnabledAsync(FeatureFlags.CompanyProfile);

    /// <summary>
    /// Get profiles
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpGet]
    public async Task<IEnumerable<CompanyProfileDto>> GetCompanyProfiles([FromQuery] CompanyProfilesParameters parameters)
    {
        var gridFilter = mapper.Map<GetCompanyProfilesDtoFilter>(parameters);
        var result = await Mediator.Send(new GetListQuery<GetCompanyProfilesDtoFilter, CompanyProfileDto>()
        {
            Filter = gridFilter,
            EntityType = EntityType.CompanyProfile,
            Sorting = parameters.Sorting,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Create new Company Profile or update existing
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("update")]
    public async Task<CompanyProfile> UpdateCompanyProfile(UpsertCompanyProfileDto companyProfile, string userCode)
    {
        return await Mediator.Send(new UpsertCommand<UpsertCompanyProfileDto, CompanyProfile>()
        {
            Key = new string[] { companyProfile.Code },
            Updated = companyProfile,
            EntityType = EntityType.CompanyProfile,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate Company Profile
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("toggleActiveCompanyProfile")]
    public async Task<CompanyProfile> ToggleActiveCompanyProfile(string code, string userCode)
    {
        return await Mediator.Send(new ToggleActiveCommand<CompanyProfile>()
        {
            Key = new string[] { code },
            EntityType = EntityType.CompanyProfile,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Get address books
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpGet("getAddressBooks")]
    public async Task<IEnumerable<AddressBookDto>> GetAddressBooks()
    {
        var result = await Mediator.Send(new GetListQuery<object, AddressBookDto>()
        {
            Filter = null,
            EntityType = EntityType.AddressBook,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Update existing Address Book
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("updateAddressBook")]
    public async Task<AddressBook> UpdateAddressBook(UpdateAddressBookDto addressBook, string userCode)
    {
        return await Mediator.Send(new UpdateCommand<UpdateAddressBookDto, AddressBook>()
        {
            Key = new string[] { addressBook.Code },
            Updated = addressBook,
            EntityType = EntityType.AddressBook,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Create new Address Book
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("addAddressBook")]
    public async Task<AddressBook> AddAddressBook(AddAddressBookDto addressBook, string userCode)
    {
        return await Mediator.Send(new AddAddressBookCommand()
        {
            AddressBook = addressBook,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate Address Book
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("toggleActiveAddressBook")]
    public async Task<AddressBook> ToggleActiveAddressBook(string code, string userCode)
    {
        return await Mediator.Send(new ToggleActiveCommand<AddressBook>()
        {
            Key = new string[] { code },
            EntityType = EntityType.AddressBook,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Get Address Contacts
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpGet("getAddressContacts")]
    public async Task<IEnumerable<AddressContactDto>> GetAddressContacts()
    {
        var result = await Mediator.Send(new GetListQuery<object, AddressContactDto>()
        {
            Filter = null,
            EntityType = EntityType.AddressContact,
            Sorting = null,
            Pagination = null,
        });
        return result.Items;
    }

    /// <summary>
    /// Update existing Address Contact
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("updateAddressContact")]
    public async Task<AddressContact> UpdateAddressContact(UpdateAddressContactDto addressContact, string userCode)
    {
        return await Mediator.Send(new UpdateCommand<UpdateAddressContactDto, AddressContact>()
        {
            Key = new string[] { addressContact.Code },
            Updated = addressContact,
            EntityType = EntityType.AddressContact,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Create new Address Contact
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("addAddressContact")]
    public async Task<AddressContact> AddAddressContact(AddAddressContactDto addressContact, string userCode)
    {
        return await Mediator.Send(new AddAddressContactCommand()
        {
            AddressContact = addressContact,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Activate or deactivate Address Contact
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpPost("toggleActiveAddressContact")]
    public async Task<AddressContact> ToggleActivAddressContact(string code, string userCode)
    {
        return await Mediator.Send(new ToggleActiveCommand<AddressContact>()
        {
            Key = new string[] { code },
            EntityType = EntityType.AddressContact,
            UserCode = userCode
        });
    }

    /// <summary>
    /// Get Address Tree
    /// </summary>
    [FeatureGate(FeatureFlags.CompanyProfile)]
    [HttpGet("getAddressTree")]
    public async Task<AddressTreeDto> GetAddressTree()
    {
        return await Mediator.Send(new GetAddressTreeQuery()
        {
        });
    }
}


