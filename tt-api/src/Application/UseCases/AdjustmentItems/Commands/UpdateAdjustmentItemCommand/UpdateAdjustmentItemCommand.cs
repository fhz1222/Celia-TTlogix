using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.AdjustmentItems.Commands.UpdateAdjustmentItemCommand;

public class UpdateAdjustmentItemCommand : IRequest
{
    public AdjustmentItemDto AdjustmentItem { get; set; }
    public string UserCode { get; set; }
}

public class UpdateAdjustmentItemCommandHandler : IRequestHandler<UpdateAdjustmentItemCommand>
{
    private readonly IRepository repository;
    private readonly IILogIntegrationRepository iLogIntegrationRepository;
    private readonly IMapper mapper;
    private readonly IDateTime dateTimeService;

    public UpdateAdjustmentItemCommandHandler(IRepository repository, IMapper mapper, IDateTime dateTimeService, IILogIntegrationRepository iLogIntegrationRepository)
    {
        this.repository = repository;
        this.iLogIntegrationRepository = iLogIntegrationRepository;
        this.mapper = mapper;
        this.dateTimeService = dateTimeService;
    }

    public async Task<Unit> Handle(UpdateAdjustmentItemCommand request, CancellationToken cancellationToken)
    {
        var adjustment = this.repository.Adjustments.GetAdjustmentDetails(request.AdjustmentItem.JobNo);
        if (adjustment == null)
            throw new UnknownJobNoException();

        var pallet = this.repository.StorageDetails.GetPalletDetail(request.AdjustmentItem.PID);
        if (pallet == null)
            throw new UnknownPIDException();

        if (!pallet.CanBeAdjusted(adjustment.WhsCode, adjustment.CustomerCode, adjustment.JobType))
            throw new IncorrectPalletException();

        if (!adjustment.CanEdit)
            throw new IllegalAdjustmentChangeException($"Adjustment with status 'Cancelled' or 'Completed' cannot be updated");

        var adjustmentItem = mapper.Map<AdjustmentItem>(request.AdjustmentItem);
        mapper.Map(pallet, adjustmentItem);

        if(pallet.Product.IsCPart && adjustmentItem.IsPositive && iLogIntegrationRepository.GetWarehouses().Contains(adjustment.WhsCode))
            throw new IllegalAdjustmentCPartPositiveChange($"Positive adjustment on C-Part is disabled in warehouses with iLog integration");

        if (!adjustmentItem.QtyIsValid)
            throw new IncorrectAdjustmentItemValueException("PID cannot adjusted to the negative Quantity");

        if (!adjustmentItem.IsAdjusted)
            throw new IncorrectAdjustmentItemValueException("PID cannot adjusted be with the same Quantity");

        repository.BeginTransaction();
        var currentDate = dateTimeService.Now;
        try
        {
            if (adjustmentItem.LineItem > 0)
            {
                // update existing object
                await this.repository.AdjustmentItems.Update(adjustmentItem, request.UserCode, currentDate);
            }
            else
            {
                // add new
                // check if PID already exists in this adjustment
                if (repository.AdjustmentItems.PalletAppearsInAdjustment(adjustment.JobNo, pallet.Id))
                    throw new PalletAlreadyUsedInAdjustmentException("PID already exists in Inventory Adjustment");
                // check if PID appears on outgoing adjustment (other adjustment with status New or Processing)
                if (repository.AdjustmentItems.PalletAppearsInOutgoingAdjustment(pallet.Id))
                    throw new PalletAlreadyUsedInAdjustmentException("PID cannot be adjusted as it is pending for completion in other adjustment job");

                // set line item
                adjustmentItem.LineItem = repository.AdjustmentItems.GetLastLineItemNumber(adjustmentItem.JobNo) + 1;
                // add new
                await repository.AdjustmentItems.AddNew(adjustmentItem, request.UserCode, currentDate);
                if (adjustment.Status != InventoryAdjustmentStatus.Processing)
                {
                    adjustment.Status = InventoryAdjustmentStatus.Processing;
                    await repository.Adjustments.Update(adjustment);
                }

            }
            await repository.SaveChangesAsync();
            repository.CommitTransaction();

        }
        catch (Exception)
        {
            repository.RollbackTransaction();
            throw;
        }
        return await Task.FromResult(Unit.Value);
    }
}
