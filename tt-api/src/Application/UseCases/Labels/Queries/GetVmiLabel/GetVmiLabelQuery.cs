using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;
using System.Security.Cryptography;

namespace Application.UseCases.Labels.Queries.GetVmiLabel;

public class GetVmiLabelQuery : IRequest<VmiLabelDto>
{
    public string Pid { get; set; }
}

public class GetVmiLabelQueryHandler : IRequestHandler<GetVmiLabelQuery, VmiLabelDto>
{
    private readonly ILabelRepository repository;

    public GetVmiLabelQueryHandler(ILabelRepository repository)
    {
        this.repository = repository;
    }

    public async Task<VmiLabelDto> Handle(GetVmiLabelQuery request, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(request.Pid))
            throw new MandatoryFilterNotSetException();

        VmiLabelDto result = repository.GetVmiLabel(request.Pid);
        return await Task.FromResult(result);
    }
}
