using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.PalletTransferRequests.Queries.GetPalletTransferRequest;

public class GetPalletTransferRequestQuery : IRequest<PalletTransferRequestDto>
{
    public string JobNo { get; set; } = null!;
}

public class GetPalletTransferRequestQueryHandler : IRequestHandler<GetPalletTransferRequestQuery, PalletTransferRequestDto>
{
    private readonly IPalletTransferRequestsRepository palletTransferRequests;
    private readonly IStorageDetailRepository storageDetails;

    public GetPalletTransferRequestQueryHandler(IPalletTransferRequestsRepository repository, IStorageDetailRepository storageDetails)
    {
        this.palletTransferRequests = repository;
        this.storageDetails = storageDetails;
    }

    public async Task<PalletTransferRequestDto> Handle(GetPalletTransferRequestQuery request, CancellationToken cancellationToken)
    {
        var ptr = await palletTransferRequests.Get(request.JobNo) ?? throw new UnknownJobNoException();
        var pallet = storageDetails.GetPalletDetail(ptr.PID) ?? throw new UnknownPIDException();
        return new PalletTransferRequestDto(ptr.JobNo, pallet.Product.CustomerSupplier.CustomerCode, pallet.WhsCode, pallet.Product.Code, pallet.Qty, pallet.Product.CustomerSupplier.SupplierId, pallet.Id);

    }
}
