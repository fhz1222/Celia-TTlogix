using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.ILogPickingRequests.Commands.NewOutboundPickingRequest;

public class NewLoadingPickingRequestCommand : IRequest<List<CreatedOutboundPickingRequestDto>>
{
    public string UserCode { get; set; } = null!;
    public string LoadingJobNo { get; set; } = null!;
}

public class NewLoadingPickingRequestCommandHandler : IRequestHandler<NewLoadingPickingRequestCommand, List<CreatedOutboundPickingRequestDto>>
{
    private readonly IOutboundRepository outboundsRepo;
    private readonly IMediator mediator;

    public NewLoadingPickingRequestCommandHandler(IOutboundRepository outboundsRepository, IMediator mediator)
    {
        outboundsRepo = outboundsRepository;
        this.mediator = mediator;
    }

    public async Task<List<CreatedOutboundPickingRequestDto>> Handle(NewLoadingPickingRequestCommand request, CancellationToken cancellationToken)
    {
        var outbounds = outboundsRepo.GetOutboundsOnLoading(request.LoadingJobNo).Select(x => x.JobNo).ToList();
        if (outbounds.Count == 0)
        {
            throw new ApplicationError("There are no outbounds on this loading.");
        }

        List<CreatedOutboundPickingRequestDto> res = new List<CreatedOutboundPickingRequestDto>();

        foreach (var outbound in outbounds)
        {
            try
            {
                var command = new NewOutboundPickingRequestCommand() { OutboundJobNo = outbound, UserCode = request.UserCode };
                res.Add(await mediator.Send(command, cancellationToken));
            }
            catch { }
        }

        return res;
    }
}
