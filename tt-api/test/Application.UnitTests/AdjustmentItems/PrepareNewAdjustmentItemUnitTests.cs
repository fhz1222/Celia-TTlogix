using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.AdjustmentItems.Commands.PrepareNewAdjustmentItemCommand;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.AdjustmentItems
{
    public class PrepareNewAdjustmentItemUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IRepository> repository;
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<ILocationRepository> locationRepository;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };
        private static string[] SupplierCodes = new string[] { "SUP1", "SUP2" };
        private static readonly string ILogInboundLoc = "iLogInbound";

        private readonly Adjustment NewAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000001",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.New,
            WhsCode = WhsCodes[0],
            CustomerCode = CustomerCodes[0],
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0]
        };

        private readonly Adjustment ProcessingAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000005",
            ReferenceNo = "test",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Processing,
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0],
            WhsCode = WhsCodes[0],
            CustomerCode = CustomerCodes[0],
        };

        private readonly Adjustment UndoZeroOutAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000105",
            ReferenceNo = "test",
            JobType = InventoryAdjustmentJobType.UndoZeroOut,
            Status = InventoryAdjustmentStatus.Processing,
            CreatedDate = DateTime.Now,
            CreatedBy = UserCodes[0],
            WhsCode = WhsCodes[0],
            CustomerCode = CustomerCodes[0],
        };

        private readonly Adjustment CompletedAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000011",
            ReferenceNo = "test",
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
            ReferenceNo = "test",
            JobType = InventoryAdjustmentJobType.UndoZeroOut,
            Status = InventoryAdjustmentStatus.Cancelled,
            CreatedDate = DateTime.Now.AddDays(-1),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = DateTime.Now,
        };

        private List<Pallet> Pallets;


        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            locationRepository = new Mock<ILocationRepository>();
            repository = new Mock<IRepository>();
            
            repository.Setup(r => r.Adjustments).Returns(adjustmentRepository.Object);
            repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);
            repository.Setup(r => r.Locations).Returns(locationRepository.Object);

            locationRepository.Setup(_ => _.IsILogInboundLocation(ILogInboundLoc, It.IsAny<string>())).Returns(true);
            locationRepository.Setup(_ => _.IsILogInboundLocation(It.IsNotIn(ILogInboundLoc), It.IsAny<string>())).Returns(false);

            var existingAdjustments = new List<Adjustment>()
            {
                NewAdjustment, ProcessingAdjustment, CompletedAdjustment, CancelledAdjustment, UndoZeroOutAdjustment
            };
            foreach(var existingAdjustment in existingAdjustments)
            {
                adjustmentRepository.Setup(r => r.GetAdjustmentDetails(existingAdjustment.JobNo)).Returns(existingAdjustment);                
            }

            Pallets = new List<Pallet>()
            {
                new Pallet
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
                    Status = StorageStatus.Putaway
                },
                new Pallet
                {
                    Id = "PID02",
                    Product = new Product
                    {
                        Code = "002",
                        CustomerSupplier = new CustomerSupplier
                        {
                            CustomerCode = CustomerCodes[1],
                            SupplierId = SupplierCodes[1]
                        }
                    },
                    Qty = 500,
                    QtyPerPkg = 500,
                    InboundDate = DateTime.Now.AddDays(-10),
                    WhsCode = WhsCodes[0],
                    Status = StorageStatus.Quarantine
                },
                new Pallet
                {
                    Id = "PID03",
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
                    Status = StorageStatus.ZeroOut
                },
            };
            foreach (var pallet in Pallets)
            {
                storageDetailRepository.Setup(r => r.GetPalletDetail(pallet.Id)).Returns(pallet);
            }
        }

        [Test]
        public async Task PrepareNewForNewAdjustment()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = NewAdjustment.JobNo,
                PID = Pallets[0].Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            NewAdjustment.ReferenceNo = "REF001";

            var newAdjustmentItem = await handler.Handle(request, CancellationToken.None);


            Assert.That(newAdjustmentItem.Pallet, Is.SameAs(Pallets[0]));
            Assert.That(newAdjustmentItem.NewQty.Equals(Pallets[0].Qty));
            Assert.That(newAdjustmentItem.NewQtyPerPkg.Equals(Pallets[0].QtyPerPkg));
            Assert.That(newAdjustmentItem.JobNo.Equals(NewAdjustment.JobNo));

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(NewAdjustment.JobNo), Times.AtLeastOnce());
            storageDetailRepository.Verify(r => r.GetPalletDetail(Pallets[0].Id), Times.Once());
        }

        [Test]
        public async Task PrepareNewForProcessingAdjustment()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                PID = Pallets[0].Id,
            };

            var newAdjustmentItem = await handler.Handle(request, CancellationToken.None);


            Assert.That(newAdjustmentItem.Pallet, Is.SameAs(Pallets[0]));
            Assert.That(newAdjustmentItem.NewQty.Equals(Pallets[0].Qty));
            Assert.That(newAdjustmentItem.NewQtyPerPkg.Equals(Pallets[0].QtyPerPkg));
            Assert.That(newAdjustmentItem.JobNo.Equals(ProcessingAdjustment.JobNo));

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(ProcessingAdjustment.JobNo), Times.Once());
            storageDetailRepository.Verify(r => r.GetPalletDetail(Pallets[0].Id), Times.Once());
        }

        [Test]
        public async Task PrepareNewForCompletedOrCancelledAdjustmentFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = CompletedAdjustment.JobNo,
                PID = Pallets[0].Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = CancelledAdjustment.JobNo,
                PID = Pallets[0].Id,
            };

            act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();
        }

        [Test]
        public async Task NotExistingPalletFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                PID = "PID00002211",
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnknownPIDException>();
        }

        [Test]
        public async Task IncorrectPalletFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                PID = Pallets[1].Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IncorrectPalletException>();
        }


        [Test]
        public async Task AddPutawayPalletToUndoZeroOutAdjustmentFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = UndoZeroOutAdjustment.JobNo,
                PID = Pallets[0].Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IncorrectStorageDetailStatusException>();
        }

        [Test]
        public async Task AddZeroOutPalletToNormalAdjustmentFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                PID = Pallets[2].Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IncorrectStorageDetailStatusException>();
        }

        [Test]
        public async Task PrepareNewForZeroOutAdjustment()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var pallet = Pallets[2];
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = UndoZeroOutAdjustment.JobNo,
                PID = pallet.Id,
            };

            var newAdjustmentItem = await handler.Handle(request, CancellationToken.None);

            Assert.That(newAdjustmentItem.Pallet, Is.SameAs(pallet));
            Assert.That(newAdjustmentItem.NewQty.Equals(pallet.Qty));
            Assert.That(newAdjustmentItem.NewQtyPerPkg.Equals(pallet.QtyPerPkg));
            Assert.That(newAdjustmentItem.JobNo.Equals(UndoZeroOutAdjustment.JobNo));

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(UndoZeroOutAdjustment.JobNo), Times.Once());
            storageDetailRepository.Verify(r => r.GetPalletDetail(pallet.Id), Times.Once());
        }

        [Test]
        public async Task ShouldPreventAdjustmentOnILogInboundLocation()
        {
            var pallet = new Pallet
            {
                Id = "PID04",
                Product = new()
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
                InboundDate = new DateTime(2020, 1, 1),
                WhsCode = WhsCodes[0],
                Status = StorageStatus.Putaway,
                Location = ILogInboundLoc
            };

            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);

            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                PID = pallet.Id,
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>().WithMessage("Cannot adjust PID on ILog Inbound location.");
        }
    }
}
