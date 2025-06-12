using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest.Queries.ShouldValidatePrice;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Commands.ValidatePrice;

public class ValidatePriceCommand : IRequest<ValidationResultDto>
{
    public decimal TotalPrice { get; set; } = default!;
    public List<int> RequestIds { get; set; } = default!;
    public string LoginId { get; set; } = default!;
}

public class ValidatePriceCommandHandler : IRequestHandler<ValidatePriceCommand, ValidationResultDto>
{
    private const decimal TOLERANCE = 0.10m;
    private readonly IRepository repository;
    private readonly IMediator mediator;

    public ValidatePriceCommandHandler(IRepository repository, IMediator mediator)
    {
        this.repository = repository;
        this.mediator = mediator;
    }

    public async Task<ValidationResultDto> Handle(ValidatePriceCommand request, CancellationToken cancellationToken)
    {
        repository.BeginTransaction();

        var shouldValidate = await mediator.Send(new ShouldValidatePriceQuery(), cancellationToken);
        if (!shouldValidate)
        {
            throw new ApplicationError("Price validation is not required");
        }

        var prices = await repository.InvoiceRequests.GetPrices(request.RequestIds);
        if (prices.None())
        {
            throw new ApplicationError("Unable to retrieve price information");
        }

        var mixedCurrency = prices.GroupBy(p => p.Currency).Count() > 1;
        var currency = mixedCurrency ? string.Empty : prices.First().Currency ?? string.Empty;
        var ttlogixPrice = Math.Round(prices.Sum(p => p.Price * p.Qty), 2);
        var supplierPrice = request.TotalPrice;
        var difference = Math.Abs(ttlogixPrice - supplierPrice);
        var success = difference <= TOLERANCE;

        var resultText = success switch
        {
            true => null,
            false => $"Price validation failed. Expected total value of the goods is {ttlogixPrice}{currency} while the total price on the provided invoices is {supplierPrice}{currency}."
        };

        var validationId = await repository.InvoiceRequests.SavePriceValidation(currency, supplierPrice, ttlogixPrice, success, request.RequestIds, request.LoginId);

        await repository.SaveChangesAsync(cancellationToken);
        repository.CommitTransaction();

        return new ValidationResultDto()
        {
            PriceValidationId = validationId,
            IsSuccess = success,
            ResultText = resultText
        };
    }
}
