using Application.Interfaces.Gateways;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.PalletTransferRequests.Commands.AddNewPalletTransferRequestCommand;
using Castle.Core.Logging;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.UnitTests.PalletTransferRequests;
public class AddNewPalletTransferRequestUnitTests
{
    private Mock<IRepository> repository;
    private Mock<IPalletTransferRequestsRepository> palletTransferRepository;
    private Mock<IStorageDetailRepository> storageDetailRepository;
    private Mock<ILocationRepository> locationsRepository;
    private Mock<IUtilsRepository> utilsRepository;
    private Mock<IDateTime> dateTime;
    private Mock<IJobNumberGenerator> jobNoGenerator;
    private Mock<IILogConnectGateway> ilogConnectGateway;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IRepository>();
        palletTransferRepository = new Mock<IPalletTransferRequestsRepository>();
        storageDetailRepository = new Mock<IStorageDetailRepository>();
        locationsRepository = new Mock<ILocationRepository>();
        utilsRepository = new Mock<IUtilsRepository>();
        dateTime = new Mock<IDateTime>();
        jobNoGenerator = new Mock<IJobNumberGenerator>();
        ilogConnectGateway = new Mock<IILogConnectGateway>();

        repository.Setup(r => r.PalletTransferRequests).Returns(palletTransferRepository.Object);
        repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);
        repository.Setup(r => r.Utils).Returns(utilsRepository.Object);
        repository.Setup(r => r.Locations).Returns(locationsRepository.Object);
    }


    [Test]
    public async Task AddNewPalletTransferReqest()
    {
        const string pid = "pid1";
        const string user = "user1";
        var location = "loc1";
        var whsCode = "IT";
        const string jobno = "jobno1";
        var now = new DateTime(2020, 1, 1);
        var pallet = new Pallet
        {
            Id = pid,
            Status = StorageStatus.Putaway,
            Location = location,
            WhsCode = whsCode
        };
        utilsRepository.Setup(x => x.CheckIfUserCodeExists(user)).Returns(true);
        jobNoGenerator.Setup(x => x.GetJobNumber(palletTransferRepository.Object)).Returns(jobno);
        storageDetailRepository.Setup(x => x.GetPalletDetail(pid)).Returns(pallet);
        locationsRepository.Setup(x => x.IsILogStorageLocation(location, whsCode)).Returns(true);

        dateTime.Setup(x => x.Now).Returns(now);

        var handler = new AddNewPalletTransferRequestCommandHandler(jobNoGenerator.Object, repository.Object, dateTime.Object, ilogConnectGateway.Object);
        var request = new AddNewPalletTransferRequestCommand
        {
            Pid = pid,
            UserCode = user
        };
        await handler.Handle(request, CancellationToken.None);

        palletTransferRepository.Verify(x => x.Add(It.Is<PalletTransferRequest>(
            ptr => ptr.PID == pid &&
            ptr.JobNo == jobno &&
            ptr.CreatedBy == user &&
            ptr.CreatedOn == now
            )), Times.Once());
        storageDetailRepository.Verify(x => x.Update(It.Is<Pallet>(p => p.Id == pid && p.Status.Equals(StorageStatus.Restricted))), Times.Once());
        ilogConnectGateway.Verify(x => x.PalletTransferRequestCreated(jobno), Times.Once());
    }
}
