using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Application.Interfaces.Utils;
using Application.Interfaces.Gateways;

namespace Application.UnitTests.Adjustments
{
    public class CompleteAdjustmentUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IAdjustmentItemRepository> adjustmentItemRepository;
        private Mock<IStorageDetailRepository> storageDetailsRepository;
        private Mock<IInventoryRepository> inventoryRepository;
        private Mock<IRepository> repository;
        private Mock<IDateTime> dtService;
        private Mock<IInventoryTransactionService> inventoryTransactionService;
        private Mock<IILogConnectGateway> ilogConnectGateway;
        private DateTime now;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private readonly Adjustment NewAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000001",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.New,
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0]
        };

        private readonly Adjustment ProcessingAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000005",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Processing,
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0],
            WhsCode = "WH01",
            CustomerCode = "CUST01",
        };

        private readonly Adjustment CompletedAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000011",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Completed,
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0],
            CompletedBy = UserCodes[1],
            CompletedDate = DateTime.Now,
        };

        private readonly Adjustment CancelledAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000020",
            JobType = InventoryAdjustmentJobType.UndoZeroOut,
            Status = InventoryAdjustmentStatus.Cancelled,
            CreatedDate = DateTime.Now.AddDays(-1),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = DateTime.Now,
        };

        private List<Adjustment> ExistingAdjustments;

        public List<AdjustmentItem> AdjustmentItems;

        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            repository = new Mock<IRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            adjustmentItemRepository = new Mock<IAdjustmentItemRepository>();
            storageDetailsRepository = new Mock<IStorageDetailRepository>();
            inventoryRepository = new Mock<IInventoryRepository>();
            ilogConnectGateway = new Mock<IILogConnectGateway>();

            dtService = new Mock<IDateTime>();
            inventoryTransactionService = new Mock<IInventoryTransactionService>();

            repository.Setup(r => r.Utils).Returns(utilsRepository.Object);
            repository.Setup(r => r.Adjustments).Returns(adjustmentRepository.Object);
            repository.Setup(r => r.AdjustmentItems).Returns(adjustmentItemRepository.Object);
            repository.Setup(r => r.StorageDetails).Returns(storageDetailsRepository.Object);
            repository.Setup(r => r.Inventory).Returns(inventoryRepository.Object);

            utilsRepository.Setup(r => r.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(r => r.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItemGroupedData(It.IsAny<string>())).Returns(new List<AdjustmentItemSummaryDto>());
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItemGroupedData(ProcessingAdjustment.JobNo)).Returns(new List<AdjustmentItemSummaryDto>()
            {
                new AdjustmentItemSummaryDto()
                {
                    JobNo = ProcessingAdjustment.JobNo,
                    CustomerCode = ProcessingAdjustment.CustomerCode,
                    Ownership = Ownership.EHP,
                    ProductCode = "PC01",
                    SupplierId = "SUP01"
                },
                new AdjustmentItemSummaryDto()
                {
                    JobNo = ProcessingAdjustment.JobNo,
                    CustomerCode = ProcessingAdjustment.CustomerCode,
                    Ownership = Ownership.EHP,
                    ProductCode = "PC02",
                    SupplierId = "SUP02"
                },
                new AdjustmentItemSummaryDto()
                {
                    JobNo = ProcessingAdjustment.JobNo,
                    CustomerCode = ProcessingAdjustment.CustomerCode,
                    Ownership = Ownership.EHP,
                    ProductCode = "PC03",
                    SupplierId = "SUP01"
                }
            });
            inventoryRepository.Setup(r => r.GetInventoryItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Ownership>())).Returns(new InventoryItem());

            ExistingAdjustments = new List<Adjustment>();
            ExistingAdjustments.Add(NewAdjustment);
            ExistingAdjustments.Add(ProcessingAdjustment);
            ExistingAdjustments.Add(CompletedAdjustment);
            ExistingAdjustments.Add(CancelledAdjustment);

            foreach (var adjustment in ExistingAdjustments)
            {
                adjustmentRepository.Setup(r => r.GetAdjustmentDetails(It.IsIn(adjustment.JobNo))).Returns(adjustment);
            }

            AdjustmentItems = new List<AdjustmentItem>();
            // add adjustmentItems
            AddAdjustmentItems();

            now = DateTime.Now;
            dtService.Setup(s => s.Now).Returns(now);
        }

        private void AddAdjustmentItems()
        {

            AdjustmentItems.Add(new AdjustmentItem
            {
                JobNo = ProcessingAdjustment.JobNo,
                ProductCode = "PC01",
                SupplierId = "SUP01",
                PID = "PID01",
                InitialQty = 30,
                NewQty = 20,
                InitialQtyPerPkg = 30,
                NewQtyPerPkg = 20,
            }) ;
            AdjustmentItems.Add(new AdjustmentItem
            {
                JobNo = ProcessingAdjustment.JobNo,
                ProductCode = "PC01",
                SupplierId = "SUP01",
                PID = "PID02",
                InitialQty = 100,
                NewQty = 120,
                InitialQtyPerPkg = 100,
                NewQtyPerPkg = 120,
            });
            AdjustmentItems.Add(new AdjustmentItem
            {
                JobNo = ProcessingAdjustment.JobNo,
                ProductCode = "PC02",
                SupplierId = "SUP02",
                PID = "PID03",
                InitialQty = 300,
                NewQty = 350,
                InitialQtyPerPkg = 300,
                NewQtyPerPkg = 350,
            });
            AdjustmentItems.Add(new AdjustmentItem
            {
                JobNo = ProcessingAdjustment.JobNo,
                ProductCode = "PC03",
                SupplierId = "SUP01",
                PID = "PID04",
                InitialQty = 100,
                NewQty = 0,
                InitialQtyPerPkg = 100,
                NewQtyPerPkg = 0,
            });

            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(It.IsNotNull<string>(), It.IsAny<string?>(), It.IsAny<bool>())).Returns(new List<AdjustmentItem>());
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(ProcessingAdjustment.JobNo, It.IsAny<string?>(), It.IsAny<bool>())).Returns(AdjustmentItems);

            foreach (var adjustmentItem in AdjustmentItems)
            {
                storageDetailsRepository.Setup(r => r.GetPalletDetail(adjustmentItem.PID)).Returns(new Pallet
                {
                    Id = adjustmentItem.PID,
                    Product = new Product
                    {
                        Code = adjustmentItem.ProductCode,
                        CustomerSupplier = new CustomerSupplier
                        {
                            CustomerCode = ProcessingAdjustment.CustomerCode,
                            SupplierId = adjustmentItem.SupplierId,
                        }
                    },
                    WhsCode = ProcessingAdjustment.WhsCode,
                    Qty = adjustmentItem.InitialQty,
                    Status = StorageStatus.Putaway
                });
            }
        }

        [Test]
        public async Task CompleteNewAdjustmentSuccess()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var originalStatus = NewAdjustment.Status;
            var request = new CompleteAdjustmentCommand
            {
                JobNo = NewAdjustment.JobNo,
                UserCode = UserCodes[1],
            };
            var result = await handler.Handle(request, CancellationToken.None);
            Assert.That(result.Status, Is.EqualTo(InventoryAdjustmentStatus.Completed));
            Assert.That(originalStatus, Is.Not.EqualTo(result.Status));
            Assert.That(result.CompletedBy, Is.EqualTo(request.UserCode));
            Assert.That(result.CompletedDate, Is.EqualTo(now));

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentRepository.Verify(a => a.Update(NewAdjustment), Times.Once());
        }

        [Test]
        public async Task CompleteProcessingAdjustmentSuccess()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var originalStatus = ProcessingAdjustment.Status;
            var request = new CompleteAdjustmentCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                UserCode = UserCodes[1],
            };
            var result = await handler.Handle(request, CancellationToken.None);
            Assert.That(result.Status, Is.EqualTo(InventoryAdjustmentStatus.Completed));
            Assert.That(originalStatus, Is.Not.EqualTo(result.Status));
            Assert.That(result.CompletedBy, Is.EqualTo(request.UserCode));
            Assert.That(result.CompletedDate, Is.EqualTo(now));

            
            utilsRepository.Verify(r => r.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentItemRepository.Verify(r => r.GetAdjustmentItems(request.JobNo, null, false), Times.Once());

            // get pallets and update status and quantity
            foreach(var item in AdjustmentItems)
            {
                storageDetailsRepository.Verify(r => r.GetPalletDetail(item.PID), Times.Once());
            }
            storageDetailsRepository.Verify(r => r.Update(It.IsAny<Pallet>()), Times.Exactly(AdjustmentItems.Count));

            // get grouped adjustment item data
            adjustmentItemRepository.Verify(r => r.GetAdjustmentItemGroupedData(request.JobNo), Times.Once());

            // check if inventory is updated
            inventoryRepository.Verify(r => r.GetInventoryItem(ProcessingAdjustment.WhsCode, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Ownership>()), Times.AtLeastOnce());
            inventoryRepository.Verify(r => r.Update(It.IsAny<InventoryItem>()), Times.AtLeastOnce());

            // call inventory transaction service
            inventoryTransactionService.Verify(s => s.GenerateInventoryTransactionsOnAdjustmentComplete(request.JobNo), Times.Once());
            
            adjustmentRepository.Verify(a => a.Update(ProcessingAdjustment), Times.Once());
        }

        [Test]
        public async Task IncorrectUserCode()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var request = new CompleteAdjustmentCommand
            {
                JobNo = NewAdjustment.JobNo,
                UserCode = "?????"
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownUserCodeException>();

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }

        [Test]
        public async Task IncorrectJobNumber()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var request = new CompleteAdjustmentCommand
            {
                JobNo = "ADJ77777777", // unknown
                UserCode = UserCodes[1]
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }

        [Test]
        public async Task IllegalAdjustmentChangeForStatusCompleted()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var request = new CompleteAdjustmentCommand
            {
                JobNo = CompletedAdjustment.JobNo,
                UserCode = UserCodes[0]
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }

        [Test]
        public async Task IllegalAdjustmentChangeForStatusCancelled()
        {
            var handler = new CompleteAdjustmentCommandHandler(repository.Object, dtService.Object, inventoryTransactionService.Object, ilogConnectGateway.Object);
            var request = new CompleteAdjustmentCommand
            {
                JobNo = CancelledAdjustment.JobNo,
                UserCode = UserCodes[1]
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }

    }
}
