using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.ShouldValidatePrice;

public class ShouldValidatePriceQuery : IRequest<bool>
{

}

public class ShouldValidatePriceQueryHandler : IRequestHandler<ShouldValidatePriceQuery, bool>
{
    private readonly IInvoiceRequestRepository repository;

    public ShouldValidatePriceQueryHandler(IInvoiceRequestRepository repository) => this.repository = repository;

    public Task<bool> Handle(ShouldValidatePriceQuery request, CancellationToken cancellationToken)
    {
        var flow = repository.GetFlow();
        var noPriceValidation = repository.GetNoPriceValidationStatus();

        var shouldValidatePrice = (flow, noPriceValidation) switch
        {
            (InvRequestFlow.Standard, false) => true,
            _ => false
        };

        return Task.FromResult(shouldValidatePrice);
    }
}
