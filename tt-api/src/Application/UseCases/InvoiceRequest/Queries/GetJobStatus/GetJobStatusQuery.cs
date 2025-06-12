using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.InvoiceRequest.Queries.GetJobStatus;

public class GetJobStatusQuery : IRequest<JobStatusDto>
{
    public string JobNo { get; set; } = default!;
}

public class GetJobStatusQueryHandler : IRequestHandler<GetJobStatusQuery, JobStatusDto>
{
    private readonly IInvoiceRequestRepository repository;

    public GetJobStatusQueryHandler(IInvoiceRequestRepository repository)
    {
        this.repository = repository;
    }

    public Task<JobStatusDto> Handle(GetJobStatusQuery request, CancellationToken cancellationToken)
    {
        var status = GetStatus(request.JobNo);

        var jobStatus = new JobStatusDto()
        {
            Status = status.ToString(),
            CanBeRequestedNow = status == InvRequestStatus.Pending,
            CanBeBlocked = status.In(InvRequestStatus.Pending, InvRequestStatus.NotApplicable),
            CanBeUnblocked = status == InvRequestStatus.Blocked
        };

        return Task.FromResult(jobStatus);
    }

    private InvRequestStatus GetStatus(string jobNo)
    {
        var flow = repository.GetFlow();

        if (flow == InvRequestFlow.None)
        {
            return InvRequestStatus.NotApplicable;
        }

        var invRequest = repository.GetRequest(jobNo);
        if (invRequest is { })
        {
            return invRequest.IsCompleted ? InvRequestStatus.Completed : InvRequestStatus.Issued;
        }

        var isBlocked = repository.IsOnBlocklist(jobNo);
        if (isBlocked)
        {
            return InvRequestStatus.Blocked;
        }

        var isOutbound = jobNo.StartsWith("OUT");
        var relevancy = -repository.GetRelevancyThreshold();

        var eligibleJobs = (isOutbound, flow) switch
        {
            (true, InvRequestFlow.Standard) => repository.GetOutboundsEligibleForStandardFlow(relevancy),
            (true, InvRequestFlow.CustomsClearance) => repository.GetOutboundsEligibleForCustomsClearanceFlow(relevancy),
            (false, InvRequestFlow.Standard) => repository.GetStockTransfersEligibleForStandardFlow(relevancy),
            (false, InvRequestFlow.CustomsClearance) => repository.GetStockTransfersEligibleForCustomsClearanceFlow(relevancy),
            _ => throw new ApplicationError()
        };
        var isEligible = eligibleJobs.Any(i => i.JobNo == jobNo);

        return isEligible ? InvRequestStatus.Pending : InvRequestStatus.NotApplicable;
    }
}
