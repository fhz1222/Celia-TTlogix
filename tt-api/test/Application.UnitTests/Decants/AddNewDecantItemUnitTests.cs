using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Decants.Commands.AddNewDecantItemCommand;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class AddNewDecantItemUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<IRepository> repository;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };
        private static string[] SupplierCodes = new string[] { "CCC", "DDD" };

        private Decant NewDecant;
        private Decant ProcessingDecant;
        private Decant CompletedDecant;
        private Decant CancelledDecant;
        private Decant[] ExistingDecants;
        private List<Pallet> Pallets;

        [SetUp]
        public void Setup()
        {
            ;
            decantRepository = new Mock<IDecantRepository>();
            repository = new Mock<IRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            repository.Setup(r => r.StorageDetails).Returns(storageDetailRepository.Object);
            repository.Setup(r=> r.Decant).Returns(decantRepository.Object);
            
            NewDecant = new Decant
            {
                JobNo = "DCT20221000001",
                Status = DecantStatus.New,
                CreatedDate = new DateTime(2022, 10, 3),
                CreatedBy = UserCodes[0],
                WhsCode = WhsCodes[0],
                CustomerCode = CustomerCodes[0],
            };
            ProcessingDecant = new Decant
            {
                JobNo = "DCT20221000002",
                Status = DecantStatus.Processing,
                CreatedDate = new DateTime(2022, 10, 4),
                CreatedBy = UserCodes[0],                
                WhsCode = WhsCodes[0],
                CustomerCode = CustomerCodes[0]
            };
            CompletedDecant = new Decant
            {
                JobNo = "DCT20221000011",
                Status = DecantStatus.Completed,
                CreatedDate = new DateTime(2022, 10, 1),
                CreatedBy = UserCodes[0],
                CompletedBy = UserCodes[1],
                CompletedDate = DateTime.Now,
            };
            CancelledDecant = new Decant
            {
                JobNo = "DCT20221000020",
                Status = DecantStatus.Cancelled,
                CreatedDate = new DateTime(2022, 10, 4),
                CreatedBy = UserCodes[0],
                CancelledBy = UserCodes[0],
                CancelledDate = new DateTime(2022, 10, 5),
            };

            ExistingDecants = new Decant[]
            {
                NewDecant, ProcessingDecant, CompletedDecant, CancelledDecant
            };

            foreach (var decant in ExistingDecants)
            {
                decantRepository.Setup(r => r.GetDecant(It.IsIn(decant.JobNo))).Returns(Task.FromResult((Decant?)decant));
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
        public async Task AddNewDecantItemSuccess()
        {
            var handler = new AddNewDecantItemCommandHandler( repository.Object);
            
            var request = new AddNewDecantItemCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[0].Id,
                NewQuantities = new int[] { 50, 50}
            };
            var originalStatus = ExistingDecants[0].Status;
            await handler.Handle(request, CancellationToken.None);

            storageDetailRepository.Verify(r => r.GetPalletDetail(request.PalletId), Times.Once);
            decantRepository.Verify(r => r.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(r => r.UpdateDecant(NewDecant), Times.Once);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Once);

            Assert.That(NewDecant.Status.Equals(DecantStatus.Processing));
        }

        [Test]
        public async Task AddNewDecantItemSuccess2()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = ProcessingDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[0].Id,
                NewQuantities = new int[] { 50, 50 }
            };
            var originalStatus = ExistingDecants[0].Status;
            await handler.Handle(request, CancellationToken.None);

            storageDetailRepository.Verify(r => r.GetPalletDetail(request.PalletId), Times.Once);
            decantRepository.Verify(r => r.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(r => r.UpdateDecant(ProcessingDecant), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Once);

            Assert.That(ProcessingDecant.Status.Equals(DecantStatus.Processing));
        }

        [Test]
        public async Task IncorrectJobNumber()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = "DCT000",
                UserCode = UserCodes[0],
                PalletId = Pallets[0].Id,
                NewQuantities = new int[] { 50, 50 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            decantRepository.Verify(a => a.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);
        }


        [Test]
        public async Task NotExistingPID()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = "PID0000",
                NewQuantities = new int[] { 50, 50 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownPIDException>();

            storageDetailRepository.Verify(a => a.GetPalletDetail(request.PalletId), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);

        }

        [Test]
        public async Task IncorrectPID()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[2].Id,
                NewQuantities = new int[] { 50, 50 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IncorrectStorageDetailStatusException>();

            storageDetailRepository.Verify(a => a.GetPalletDetail(request.PalletId), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);
        }

        [Test]
        public async Task IncorrectSumOfNewQuantities()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[0].Id,
                NewQuantities = new int[] { 20, 20, 20 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalDecantActionException>();

            storageDetailRepository.Verify(a => a.GetPalletDetail(request.PalletId), Times.Once);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
        }

        [Test]
        public async Task AddNewToCancelledFailed()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = CancelledDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[2].Id,
                NewQuantities = new int[] { 50, 50 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            decantRepository.Verify(a => a.GetDecant(CancelledDecant.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);
        }

        [Test]
        public async Task AddNewToCompletedFailed()
        {
            var handler = new AddNewDecantItemCommandHandler(repository.Object);

            var request = new AddNewDecantItemCommand
            {
                JobNo = CompletedDecant.JobNo,
                UserCode = UserCodes[0],
                PalletId = Pallets[2].Id,
                NewQuantities = new int[] { 50, 50 }
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            decantRepository.Verify(a => a.GetDecant(CompletedDecant.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
            decantRepository.Verify(a => a.AddDecantItem(request.JobNo, request.UserCode, It.IsAny<DecantItem>()), Times.Never);
        }
    }
}
