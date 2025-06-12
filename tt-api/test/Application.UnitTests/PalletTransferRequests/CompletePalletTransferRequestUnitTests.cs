using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;
using Application.UseCases.PalletTransferRequests.Commands.AddNewPalletTransferRequestCommand;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.PalletTransferRequests;
public class CompletePalletTransferRequestUnitTests
{

    private Mock<IRepository> repository;
    private Mock<IPalletTransferRequestsRepository> palletTransferRepository;
    private Mock<IStorageDetailRepository> storageDetailRepository;
    private Mock<IDateTime> dateTime;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IRepository>();
        palletTransferRepository = new Mock<IPalletTransferRequestsRepository>();
        storageDetailRepository = new Mock<IStorageDetailRepository>();
        dateTime = new Mock<IDateTime>();
        repository.Setup(r => r.PalletTransferRequests).Returns(palletTransferRepository.Object);
        repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);
    }


    [Test]
    public async Task CompletePalletTransferRequest()
    {
        var jobNo = "JOBNO";
        var pid = "pid1";
        var user = "user";
        var now = new DateTime(2020, 1, 1);
        var ptr = new PalletTransferRequest(jobNo, pid, now, user);
        var pallet = new Pallet
        {
            Id = pid,
            Status = StorageStatus.Restricted
        };
        palletTransferRepository.Setup(x => x.Get(jobNo)).ReturnsAsync(ptr);
        storageDetailRepository.Setup(x => x.GetPalletDetail(pid)).Returns(pallet);
        dateTime.Setup(x => x.Now).Returns(now);

        var handler = new CompletePalletTransferRequestCommandHandler(repository.Object, dateTime.Object);
        var request = new CompletePalletTransferRequestCommand
        {
            JobNo = jobNo,
            Pid = pid,
        };
        await handler.Handle(request, CancellationToken.None);

        palletTransferRepository.Verify(x => x.Update(It.Is<PalletTransferRequest>(x => x.CompletedOn == now)), Times.Once);
        storageDetailRepository.Verify(x => x.Update(It.Is<Pallet>(p => p.Id == pid && p.Status.Equals(StorageStatus.Putaway))), Times.Once());
    }

}
