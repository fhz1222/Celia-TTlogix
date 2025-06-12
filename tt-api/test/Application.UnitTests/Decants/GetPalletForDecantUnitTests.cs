using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Decants.Queries.GetPalletForDecant;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class GetPalletForDecantUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<ILocationRepository> locationRepository;
        private Mock<IRepository> repository;

        private static readonly string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static readonly string[] CustomerCodes = new string[] { "AAA", "BBB" };
        private static readonly string[] Locations = new string[] { "Loc1" };
        private static readonly string ILogInboundLoc = "iLogInbound";
        private static readonly string ILogStorageLoc = "iLogStorage";

        [SetUp]
        public void Setup()
        {
            decantRepository = new Mock<IDecantRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            locationRepository = new Mock<ILocationRepository>();
            repository = new Mock<IRepository>();

            repository.Setup(_ => _.StorageDetails).Returns(storageDetailRepository.Object);
            repository.Setup(_ => _.Decant).Returns(decantRepository.Object);
            repository.Setup(_ => _.Locations).Returns(locationRepository.Object);

            locationRepository.Setup(_ => _.IsILogInboundLocation(ILogInboundLoc, It.IsAny<string>())).Returns(true);
            locationRepository.Setup(_ => _.IsILogInboundLocation(It.IsNotIn(ILogInboundLoc), It.IsAny<string>())).Returns(false);
            locationRepository.Setup(_ => _.IsILogStorageLocation(ILogStorageLoc, It.IsAny<string>())).Returns(true);
            locationRepository.Setup(_ => _.IsILogStorageLocation(It.IsNotIn(ILogStorageLoc), It.IsAny<string>())).Returns(false);
        }

        [Test]
        public async Task ShouldPreventFromDecantingPalletOnILogInboundLocation()
        {
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = WhsCodes.First(),
                Location = ILogInboundLoc,
                InboundDate = new DateTime(2020, 1, 1),
                Qty = 1,
                QtyPerPkg = 1,
                OriginalQty = 1,
                Product = new()
                {
                    Code = "Product1",
                    CustomerSupplier = new CustomerSupplier
                    {
                        CustomerCode = CustomerCodes.First(),
                        SupplierId = "Supplier1"
                    }
                },
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                Length = 1,
                Width = 1,
                Height = 1,
                NetWeight = 1, 
                GrossWeight = 1
            };
            var decant = new Decant
            {
                JobNo = "DCT1",
                CustomerCode = CustomerCodes.First(),
                CustomerName = "Customer1",
                WhsCode = WhsCodes.First(),
                ReferenceNo = "Ref1",
                Remark = "Remark1",
                Status = DecantStatus.New,
                CreatedBy = "test",
                CreatedDate = new DateTime(2020,1,1)
            };
            decantRepository.Setup(_ => _.GetDecant(decant.JobNo)).Returns(() => Task.FromResult<Decant?>(decant));
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(() => pallet);

            var handler = new GetPalletForDecantQueryHandler(repository.Object);
            var query = new GetPalletForDecantQuery()
            {
                PID = pallet.Id,
                JobNo = decant.JobNo
            };

            var act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalDecantActionException>().WithMessage("Cannot decant PID on ILog Inbound location.");
        }

        [Test]
        public async Task ShouldPreventFromDecantingPalletOnILogStorageLocation()
        {
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = WhsCodes.First(),
                Location = ILogStorageLoc,
                InboundDate = new DateTime(2020, 1, 1),
                Qty = 1,
                QtyPerPkg = 1,
                OriginalQty = 1,
                Product = new()
                {
                    Code = "Product1",
                    CustomerSupplier = new CustomerSupplier
                    {
                        CustomerCode = CustomerCodes.First(),
                        SupplierId = "Supplier1"
                    }
                },
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                Length = 1,
                Width = 1,
                Height = 1,
                NetWeight = 1,
                GrossWeight = 1
            };
            var decant = new Decant
            {
                JobNo = "DCT1",
                CustomerCode = CustomerCodes.First(),
                CustomerName = "Customer1",
                WhsCode = WhsCodes.First(),
                ReferenceNo = "Ref1",
                Remark = "Remark1",
                Status = DecantStatus.New,
                CreatedBy = "test",
                CreatedDate = new DateTime(2020, 1, 1)
            };
            decantRepository.Setup(_ => _.GetDecant(decant.JobNo)).Returns(() => Task.FromResult<Decant?>(decant));
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(() => pallet);

            var handler = new GetPalletForDecantQueryHandler(repository.Object);
            var query = new GetPalletForDecantQuery()
            {
                PID = pallet.Id,
                JobNo = decant.JobNo
            };

            var act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalDecantActionException>().WithMessage("Cannot decant PID on ILog Storage location.");
        }


        [Test]
        public async Task ShouldReturnPalletDetailsForDecant()
        {
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = WhsCodes.First(),
                Location = Locations.First(),
                InboundDate = new DateTime(2020, 1, 1),
                Qty = 1,
                QtyPerPkg = 1,
                OriginalQty = 1,
                Product = new()
                {
                    Code = "Product1",
                    CustomerSupplier = new CustomerSupplier
                    {
                        CustomerCode = CustomerCodes.First(),
                        SupplierId = "Supplier1"
                    }
                },
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                Length = 1,
                Width = 1,
                Height = 1,
                NetWeight = 1,
                GrossWeight = 1
            };
            var decant = new Decant
            {
                JobNo = "DCT1",
                CustomerCode = CustomerCodes.First(),
                CustomerName = "Customer1",
                WhsCode = WhsCodes.First(),
                ReferenceNo = "Ref1",
                Remark = "Remark1",
                Status = DecantStatus.New,
                CreatedBy = "test",
                CreatedDate = new DateTime(2020, 1, 1)
            };
            decantRepository.Setup(_ => _.GetDecant(decant.JobNo)).Returns(() => Task.FromResult<Decant?>(decant));
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(() => pallet);

            var handler = new GetPalletForDecantQueryHandler(repository.Object);
            var query = new GetPalletForDecantQuery()
            {
                PID = pallet.Id,
                JobNo = decant.JobNo
            };

            var act = await handler.Handle(query, CancellationToken.None);
            act.Should().BeEquivalentTo(pallet);
        }

    }
}
