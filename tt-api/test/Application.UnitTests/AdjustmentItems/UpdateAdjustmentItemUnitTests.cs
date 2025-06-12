using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Mappings;
using Application.UseCases.AdjustmentItems;
using Application.UseCases.AdjustmentItems.Commands.UpdateAdjustmentItemCommand;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.AdjustmentItems
{
    public class UpdateAdjustmentItemUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IAdjustmentItemRepository> adjustmentItemRepository;
        private Mock<IRepository> repository;
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<IDateTime> dateTimeService;
        private Mock<IILogIntegrationRepository> iLogIntegrationRepository;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL", "IT_ILOG" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };
        private static string[] SupplierCodes = new string[] { "SUP1", "SUP2" };

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
            WhsCode = WhsCodes[0],
            CustomerCode = CustomerCodes[0],
        };

        private readonly Adjustment CancelledAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000020",
            ReferenceNo = "test",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Cancelled,
            CreatedDate = DateTime.Now.AddDays(-1),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = DateTime.Now,
            WhsCode = WhsCodes[0],
            CustomerCode = CustomerCodes[0],
        };

        private readonly Adjustment CPartPositiveAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000099",
            ReferenceNo = "test",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Processing,
            CreatedDate = DateTime.Now.AddDays(-1),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = DateTime.Now,
            WhsCode = WhsCodes[3],
            CustomerCode = CustomerCodes[0],
        };

        private List<Pallet> Pallets;
        IMapper mapper;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AdjustmentItemMapperProfile()));
            mapper = config.CreateMapper();
            
            dateTimeService = new Mock<IDateTime>();
            var currentDate = DateTime.Now;
            dateTimeService.Setup(_ => _.Now).Returns(currentDate);

            adjustmentRepository = new Mock<IAdjustmentRepository>();
            adjustmentItemRepository = new Mock<IAdjustmentItemRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            repository = new Mock<IRepository>();
            iLogIntegrationRepository = new Mock<IILogIntegrationRepository>();

            iLogIntegrationRepository.Setup(x => x.GetWarehouses()).Returns(() => new string[] { WhsCodes[3] });

            repository.Setup(r => r.Adjustments).Returns(adjustmentRepository.Object);
            repository.Setup(r => r.AdjustmentItems).Returns(adjustmentItemRepository.Object);
            repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);

            var existingAdjustments = new List<Adjustment>()
            {
                NewAdjustment, ProcessingAdjustment, CompletedAdjustment, CancelledAdjustment, UndoZeroOutAdjustment, CPartPositiveAdjustment
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
                new Pallet
                {
                    Id = "PID04",
                    Product = new Product
                    {
                        Code = "001",
                        CustomerSupplier = new CustomerSupplier
                        {
                            CustomerCode = CustomerCodes[0],
                            SupplierId = SupplierCodes[0]
                        },
                        IsCPart = true,
                    },
                    Qty = 100,
                    QtyPerPkg = 100,
                    InboundDate = DateTime.Now.AddDays(-10),
                    WhsCode = WhsCodes[3],
                    Status = StorageStatus.Putaway
                },
            };
            foreach (var pallet in Pallets)
            {
                storageDetailRepository.Setup(r => r.GetPalletDetail(pallet.Id)).Returns(pallet);
            }
        }

        [Test]
        public async Task AddNewAdjustmentItemForNewAdjustment()
        {
            var pallet = Pallets[0];
            var adjustment = NewAdjustment;
            adjustmentItemRepository.Setup(r => r.PalletAppearsInAdjustment(adjustment.JobNo, pallet.Id)).Returns(false);
            adjustmentItemRepository.Setup(r => r.PalletAppearsInOutgoingAdjustment(pallet.Id)).Returns(false);

            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = adjustment.JobNo,
                    LineItem = 0,
                    PID = pallet.Id,
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };

            await handler.Handle(request, CancellationToken.None);
            
            adjustmentItemRepository.Verify(r => r.PalletAppearsInAdjustment(adjustment.JobNo, pallet.Id), Times.AtLeastOnce());
            adjustmentItemRepository.Verify(r => r.PalletAppearsInOutgoingAdjustment(pallet.Id), Times.Once());
            adjustmentItemRepository.Verify(r => r.AddNew(It.IsAny<AdjustmentItem>(), request.UserCode, dateTimeService.Object.Now), Times.Once());
            storageDetailRepository.Verify(r => r.GetPalletDetail(Pallets[0].Id), Times.Once());
            adjustmentRepository.Verify(r => r.Update(adjustment), Times.Once());
            Assert.That(adjustment.Status.Equals(InventoryAdjustmentStatus.Processing));
        }

        [Test]
        public async Task UpdateAdjustmentItemForProcessingAdjustment()
        {
            var pallet = Pallets[0];
            var adjustment = ProcessingAdjustment;
            
            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = adjustment.JobNo,
                    LineItem = 1,
                    PID = pallet.Id,
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };

            await handler.Handle(request, CancellationToken.None);
            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(adjustment.JobNo), Times.Once());
            storageDetailRepository.Verify(r => r.GetPalletDetail(Pallets[0].Id), Times.Once());
            adjustmentItemRepository.Verify(r => r.Update(It.IsAny<AdjustmentItem>(), request.UserCode, dateTimeService.Object.Now), Times.Once());
            adjustmentItemRepository.Verify(r => r.AddNew(It.IsAny<AdjustmentItem>(), request.UserCode, dateTimeService.Object.Now), Times.Never());

            Assert.That(adjustment.Status.Equals(InventoryAdjustmentStatus.Processing));
        }
        

        [Test]
        public async Task AddNewForCompletedOrCancelledAdjustmentFailed()
        {
            var pallet = Pallets[0];

            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = CompletedAdjustment.JobNo,
                    LineItem = 1,
                    PID = pallet.Id,
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = CancelledAdjustment.JobNo,
                    LineItem = 1,
                    PID = pallet.Id,
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };

            act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();
        }


        [Test]
        public async Task NotExistingPalletFailed()
        {
            var adjustment = ProcessingAdjustment;

            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = adjustment.JobNo,
                    LineItem = 1,
                    PID = "PID_NOT_EXISTING",
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<UnknownPIDException>();
        }

        [Test]
        public async Task IncorrectPalletFailed()
        {
            var pallet = Pallets[1];
            var adjustment = NewAdjustment;

            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = adjustment.JobNo,
                    LineItem = 0,
                    PID = pallet.Id,
                    NewQty = 10,
                    NewQtyPerPkg = 10
                }
            };


            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IncorrectPalletException>();
        }

        [Test]
        public async Task CPartPositiveAdjustmentFailed()
        {
            var pallet = Pallets[3];
            var adjustment = CPartPositiveAdjustment;

            var handler = new UpdateAdjustmentItemCommandHandler(repository.Object, mapper, dateTimeService.Object, iLogIntegrationRepository.Object);
            var request = new UpdateAdjustmentItemCommand
            {
                UserCode = UserCodes[0],
                AdjustmentItem = new AdjustmentItemDto
                {
                    JobNo = adjustment.JobNo,
                    LineItem = 0,
                    PID = pallet.Id,
                    NewQty = 9999,
                    NewQtyPerPkg = 9999
                }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IllegalAdjustmentCPartPositiveChange>();
        }

        /*
        
        [Test]
        public async Task AddPutawayPalletToUndoZeroOutAdjustmentFailed()
        {
            var handler = new PrepareNewAdjustmentItemCommandHandler(repository.Object);
            var request = new PrepareNewAdjustmentItemCommand
            {
                CustomerCode = CustomerCodes[0],
                WhsCode = WhsCodes[0],
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
                CustomerCode = CustomerCodes[0],
                WhsCode = WhsCodes[0],
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
                CustomerCode = CustomerCodes[0],
                WhsCode = WhsCodes[0],
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
        */

    }
}
