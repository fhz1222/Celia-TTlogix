using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.UseCases.Decants.Commands.AddNewDecantCommand;

public class AddNewDecantCommand : IRequest<Decant>
{
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}

public class AddNewDecantCommandHandler : IRequestHandler<AddNewDecantCommand, Decant>
{
    private readonly IJobNumberGenerator jobNumberGenerator;
    private readonly IRepository repository;

    public AddNewDecantCommandHandler(IJobNumberGenerator jobNumberGenerator, IRepository repository)
    {
        this.jobNumberGenerator = jobNumberGenerator;
        this.repository = repository;
    }

    public async Task<Decant> Handle(AddNewDecantCommand request, CancellationToken cancellationToken)
    {
        // check if warehouse code and customer code exists 
        if (!repository.Utils.CheckIfWhsCodeExists(request.WhsCode))
            throw new UnknownWhsCodeException();
        if (!repository.Utils.CheckIfCustomerCodeExists(request.CustomerCode))
            throw new UnknownCustomerCodeException();
        if (!repository.Utils.CheckIfUserCodeExists(request.UserCode))
            throw new UnknownUserCodeException();

        var jobNumber = jobNumberGenerator.GetJobNumber(repository.Decant);
        var newDecant = new Decant
        {
            JobNo = jobNumber,
            CustomerCode = request.CustomerCode,
            WhsCode = request.WhsCode,
            CreatedBy = request.UserCode,
            CreatedDate = DateTime.Now,
            Status = DecantStatus.New,
        };
        await repository.Decant.AddNewDecant(newDecant);
        return newDecant;
    }
}
