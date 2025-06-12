using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Decants.Queries.GetPalletForDecant;
using Application.UseCases.Relocation.Commands;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
namespace Application.UnitTests.Relocation
{
    public class ForceRelocatePalletTests
    {
        private Mock<IStorageDetailRepository> storageDetailRepository;
        private Mock<ILocationRepository> locationRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRelocationRepository> relocationRepository;
        private Mock<IRepository> repository;
        private Mock<ILogger<ForceRelocatePalletFromiLogCommandHandler>> logger = new();

        private static readonly string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static readonly Location[] Locations = new Location[]
        {
            new()
            {
                Code = "Loc1",
                Name = "Name1",
                WarehouseCode = WhsCodes[0],
                IsActive = true
            },
            new()
            {
                Code = "Loc2",
                Name = "Name2",
                WarehouseCode = WhsCodes[0],
                IsActive = true
            }
        };

        [SetUp]
        public void Setup()
        {
            utilsRepository = new Mock<IUtilsRepository>();
            storageDetailRepository = new Mock<IStorageDetailRepository>();
            locationRepository = new Mock<ILocationRepository>();
            relocationRepository = new Mock<IRelocationRepository>();
            repository = new Mock<IRepository>();

            repository.Setup(_ => _.StorageDetails).Returns(storageDetailRepository.Object);
            repository.Setup(_ => _.Relocations).Returns(relocationRepository.Object);
            repository.Setup(_ => _.Locations).Returns(locationRepository.Object);
            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);

            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsIn(WhsCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsNotIn(WhsCodes))).Returns(false);

            foreach (var loc in Locations) 
            {
                locationRepository.Setup(_ => _.GetLocation(loc.Code, loc.WarehouseCode)).Returns(loc);
            }
        }

        [Test]
        public async Task ShouldFailIfPidNotExist()
        {
            var pid = "Nonexistent";
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pid)).Returns<Pallet?>(null);

            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var query = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pid,
                NewLocation = Locations.Skip(1).First().Code,
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1)
            };

            var act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownPIDException>();
        }

        [Test]
        public async Task ShouldFailIfSrcLocationNotExist()
        {
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = Locations.First().WarehouseCode,
                Location = "Nonexistent",
            };
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);

            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var query = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pallet.Id,
                NewLocation = Locations.Skip(1).First().Code,
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1)
            };

            var act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownLocationException>();
        }

        [Test]
        public async Task ShouldFailIfTrgLocationNotExist()
        {
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = Locations.First().WarehouseCode,
                Location = Locations.First().Code,
            };
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);

            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var query = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pallet.Id,
                NewLocation = "Nonexistent",
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1)
            };

            var act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownLocationException>();
        }

        [Test]
        public async Task ShouldRelocate()
        {
            var srcLocation = Locations.First();
            var trgLocation = Locations.Skip(1).First();
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = srcLocation.WarehouseCode,
                Location = srcLocation.Code,
            };
            locationRepository.Setup(_ => _.IsLocationOfCategory(trgLocation.Code, trgLocation.WarehouseCode, "iLogStorage")).Returns(true);
            locationRepository.Setup(_ => _.IsILogStorageLocation(srcLocation.Code, srcLocation.WarehouseCode)).Returns(true);
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);
            RelocationLog? savedRelocationLog = null;
            relocationRepository
                .Setup(_ => _.AddRelocationLog(It.IsAny<RelocationLog>()))
                .Callback<RelocationLog>(r => savedRelocationLog = r);

            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var request = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pallet.Id,
                NewLocation = trgLocation.Code,
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1),
                AllowedTrgLocationCategory = "iLogStorage"
            };

            await handler.Handle(request, CancellationToken.None);

            storageDetailRepository.Verify(_ => _.Update(It.Is<Pallet>(x => x.Location == request.NewLocation)), Times.Once);
            relocationRepository.Verify(_ => _.AddRelocationLog(It.IsAny<RelocationLog>()), Times.Once);
            var expectedRelocationLog = new RelocationLog
            {
                PalletId = pallet.Id,
                ExternalPalletId = string.Empty,
                SourceLocation = srcLocation,
                TargetLocation = trgLocation,
                ScannerType = ScannerType.ILogScanner,
                RelocatedBy = request.RelocatedBy,
                RelocatedOn = request.RelocatedOn
            };
            savedRelocationLog.Should().BeEquivalentTo(expectedRelocationLog);
        }

        [Test]
        public async Task ShouldFailIfTrgLocationCategoryNotAllowed()
        {

            var srcLocation = Locations.First();
            var trgLocation = Locations.Skip(1).First();
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = srcLocation.WarehouseCode,
                Location = srcLocation.Code,
            };
            locationRepository.Setup(_ => _.IsLocationOfCategory(trgLocation.Code, trgLocation.WarehouseCode, It.IsAny<string>())).Returns(false);
            locationRepository.Setup(_ => _.IsILogStorageLocation(srcLocation.Code, srcLocation.WarehouseCode)).Returns(true);
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);
            
            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var request = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pallet.Id,
                NewLocation = trgLocation.Code,
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1),
                AllowedTrgLocationCategory = "iLogStorage"
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalLocationException>();
        }

        [Test]
        public async Task ShouldFailIfSrcLocationCategoryNotiLogStorageStorage()
        {
            var srcLocation = Locations.First();
            var trgLocation = Locations.Skip(1).First();
            var pallet = new Pallet
            {
                Id = "PID1",
                WhsCode = srcLocation.WarehouseCode,
                Location = srcLocation.Code,
            };
            locationRepository.Setup(_ => _.IsLocationOfCategory(trgLocation.Code, trgLocation.WarehouseCode, It.IsAny<string>())).Returns(true);
            locationRepository.Setup(_ => _.IsILogStorageLocation(srcLocation.Code, srcLocation.WarehouseCode)).Returns(false);
            storageDetailRepository.Setup(_ => _.GetPalletDetail(pallet.Id)).Returns(pallet);

            var handler = new ForceRelocatePalletFromiLogCommandHandler(logger.Object, repository.Object);
            var request = new ForceRelocatePalletFromiLogCommand()
            {
                PID = pallet.Id,
                NewLocation = trgLocation.Code,
                RelocatedBy = "user",
                RelocatedOn = new DateTime(2020, 1, 1),
                AllowedTrgLocationCategory = "iLogStorage"
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalLocationException>();
        }

    }
}
