using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Decants.Commands.CompleteDecantCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class CompleteDecantUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<IRepository> repository;
        private Mock<IDateTime> dateTimeService;
        private Mock<IPIDGenerator> pidGenerator;
        private Mock<IInventoryTransactionService> inventoryTransactionService;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };
        private static string[] SupplierCodes = new string[] { "CCC", "DDD" };

        private Pallet pallet;

        [SetUp]
        public void Setup()
        {
            decantRepository = new Mock<IDecantRepository>();
            repository = new Mock<IRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            dateTimeService = new Mock<IDateTime>();
            var currentDate = DateTime.Now;
            dateTimeService.Setup(_ => _.Now).Returns(currentDate); 
            pidGenerator = new Mock<IPIDGenerator>();
            inventoryTransactionService = new Mock<IInventoryTransactionService>();

            repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);
            repository.Setup(r=> r.Decant).Returns(decantRepository.Object);
            pallet = new Pallet
            {
                Id = "PID01",
                Product = new Product
                {
                    Code = "001",
                    CustomerSupplier = new CustomerSupplier
                    {
                        CustomerCode = CustomerCodes[0],
                        SupplierId = SupplierCodes[0]
                    }
                },
                Qty = 100,
                QtyPerPkg = 100,
                InboundDate = DateTime.Now.AddDays(-10),
                WhsCode = WhsCodes[0],
                Status = StorageStatus.Decant
            };
                
            storageDetailRepository.Setup(r => r.GetPalletDetail(pallet.Id)).Returns(pallet);
        }

        [Test]
        public async Task CompleteNewDecantSuccess()
        {
            var newDecant = new Decant
            {
                JobNo = "DCT20221000001",
                Status = DecantStatus.New,
                CreatedDate = new DateTime(2022, 10, 3),
                CreatedBy = UserCodes[0],
                WhsCode = WhsCodes[0],
                CustomerCode = CustomerCodes[0],
            };
            decantRepository.Setup(r => r.GetDecant(It.IsIn(newDecant.JobNo))).Returns(Task.FromResult((Decant?)newDecant));

            var handler = new CompleteDecantCommandHandler(repository.Object, dateTimeService.Object, pidGenerator.Object, inventoryTransactionService.Object);
            
            var request = new CompleteDecantCommand
            {
                JobNo = newDecant.JobNo,
                UserCode = UserCodes[0],
            };
            var originalStatus = newDecant.Status;
            await handler.Handle(request, CancellationToken.None);

            storageDetailRepository.Verify(r => r.GetPalletDetail(It.IsAny<string>()), Times.Never);
            decantRepository.Verify(r => r.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(r => r.UpdateDecant(newDecant), Times.Once);

            Assert.That(newDecant.Status.Equals(DecantStatus.Completed));
        }

        [Test]
        public async Task CompleteProcessingDecantSuccess()
        {
            var newDecantItemPallet1 = new DecantItemPallet
            {
                SequenceNo = 1,
                Qty = 70,
            };
            newDecantItemPallet1.CopyDataFromPallet(pallet);
            var newDecantItemPallet2 = new DecantItemPallet
            {
                SequenceNo = 2,
                Qty = 30,
            };
            newDecantItemPallet1.CopyDataFromPallet(pallet);

            var processingDecant = new Decant
            {
                JobNo = "DCT20221000002",
                Status = DecantStatus.Processing,
                CreatedDate = new DateTime(2022, 10, 4),
                CreatedBy = UserCodes[0],
                WhsCode = WhsCodes[0],
                CustomerCode = CustomerCodes[0],
                Items = new List<DecantItem>()
                {
                    new DecantItem
                    {
                        SourcePalletId = pallet.Id,
                        NewPallets = new List<DecantItemPallet>()
                        {
                            newDecantItemPallet1, newDecantItemPallet2
                        }
                    }
                }
            };
            decantRepository.Setup(r => r.GetDecant(It.IsIn(processingDecant.JobNo))).Returns(Task.FromResult((Decant?)processingDecant));


            pidGenerator.Setup(s => s.GetNewPIDs(repository.Object, It.IsAny<int>())).Returns(new string[] { "PID_NEW1", "PID_NEW2" });
            var handler = new CompleteDecantCommandHandler(repository.Object, dateTimeService.Object, pidGenerator.Object, inventoryTransactionService.Object);

            var request = new CompleteDecantCommand
            {
                JobNo = processingDecant.JobNo,
                UserCode = UserCodes[0],
            };

            await handler.Handle(request, CancellationToken.None);

            decantRepository.Verify(r => r.GetDecant(request.JobNo), Times.Once);

            foreach (var p in processingDecant.Items)
            {
                storageDetailRepository.Verify(r => r.GetPalletDetail(p.SourcePalletId), Times.Once);
                storageDetailRepository.Verify(r => r.AddNewPallet(It.IsAny<Pallet>(), It.IsAny<string>()), Times.Exactly(2));
            }
            pidGenerator.Verify(s => s.GetNewPIDs(repository.Object, 2), Times.Once);    
            storageDetailRepository.Verify(r => r.Update(It.IsAny<Pallet>()), Times.Exactly(processingDecant.Items.Count));   
            decantRepository.Verify(r => r.UpdateDecant(processingDecant), Times.Once);

            Assert.That(processingDecant.Status.Equals(DecantStatus.Completed));
        }

        [Test]
        public async Task CompleteOnCompletedDecantFailed()
        {
            var completedDecant = new Decant
            {
                JobNo = "DCT20221000011",
                Status = DecantStatus.Completed,
                CreatedDate = new DateTime(2022, 10, 1),
                CreatedBy = UserCodes[0],
                CompletedBy = UserCodes[1],
                CompletedDate = DateTime.Now,
            };
            decantRepository.Setup(r => r.GetDecant(It.IsIn(completedDecant.JobNo))).Returns(Task.FromResult((Decant?)completedDecant));

            var handler = new CompleteDecantCommandHandler(repository.Object, dateTimeService.Object, pidGenerator.Object, inventoryTransactionService.Object);

            var request = new CompleteDecantCommand
            {
                JobNo = completedDecant.JobNo,
                UserCode = UserCodes[0],
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            decantRepository.Verify(a => a.GetDecant(completedDecant.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            storageDetailRepository.Verify(a => a.AddNewPallet(It.IsAny<Pallet>(), It.IsAny<string>()), Times.Never);
        }

        public async Task CompleteOnCancelledDecantFailed()
        {
            var cancelledDecant = new Decant
            {
                JobNo = "DCT20221000020",
                Status = DecantStatus.Cancelled,
                CreatedDate = new DateTime(2022, 10, 4),
                CreatedBy = UserCodes[0],
                CancelledBy = UserCodes[0],
                CancelledDate = new DateTime(2022, 10, 5),
            };
            decantRepository.Setup(r => r.GetDecant(It.IsIn(cancelledDecant.JobNo))).Returns(Task.FromResult((Decant?)cancelledDecant));

            var handler = new CompleteDecantCommandHandler(repository.Object, dateTimeService.Object, pidGenerator.Object, inventoryTransactionService.Object);

            var request = new CompleteDecantCommand
            {
                JobNo = cancelledDecant.JobNo,
                UserCode = UserCodes[0],
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            decantRepository.Verify(a => a.GetDecant(cancelledDecant.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            storageDetailRepository.Verify(a => a.AddNewPallet(It.IsAny<Pallet>(), It.IsAny<string>()), Times.Never);
        }
    }
}
