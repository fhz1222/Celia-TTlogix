using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Adjustments.Commands.CancelAdjustmentCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Adjustments
{
    public class CancelAdjustmentUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository; 
        private Mock<IDateTime> dtService;
        private DateTime now;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private readonly Adjustment NewAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000001",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.New,
            CreatedDate = new DateTime(2022, 10, 3),
            CreatedBy = UserCodes[0]
        };

        private readonly Adjustment CompletedAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000011",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Completed,
            CreatedDate = new DateTime(2022, 10, 1),
            CreatedBy = UserCodes[0],
            CompletedBy = UserCodes[1],
            CompletedDate = DateTime.Now,
        };

        private readonly Adjustment CancelledAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000020",
            JobType = InventoryAdjustmentJobType.UndoZeroOut,
            Status = InventoryAdjustmentStatus.Cancelled,
            CreatedDate = new DateTime(2022, 10, 4),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = new DateTime(2022, 10, 5),
        };

        private List<Adjustment> ExistingAdjustments = new List<Adjustment>();


        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            repository = new Mock<IRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            dtService = new Mock<IDateTime>();

            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            repository.Setup(_ => _.Adjustments).Returns(adjustmentRepository.Object);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);

            ExistingAdjustments.Add(NewAdjustment);
            ExistingAdjustments.Add(CompletedAdjustment);
            ExistingAdjustments.Add(CancelledAdjustment);

            foreach (var adjustment in ExistingAdjustments)
            {
                adjustmentRepository.Setup(_ => _.GetAdjustmentDetails(It.IsIn(adjustment.JobNo))).Returns(adjustment);
            }

            now = DateTime.Now;
            dtService.Setup(s => s.Now).Returns(now);
        }

        [Test]
        public async Task CancelAdjustmentWithStatusNewSuccess()
        {
            var handler = new CancelAdjustmentCommandHandler(repository.Object, dtService.Object);
            var request = new CancelAdjustmentCommand
            {
                JobNo = NewAdjustment.JobNo,
                UserCode = UserCodes[1]
            };
            var originalStatus = NewAdjustment.Status;

            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.That(result.Status, Is.EqualTo(InventoryAdjustmentStatus.Cancelled));
            Assert.That(originalStatus, Is.Not.EqualTo(result.Status));
            Assert.That(result.CancelledBy, Is.EqualTo(request.UserCode));
            Assert.That(result.CancelledDate, Is.EqualTo(now));

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(NewAdjustment), Times.Once);
        }

        [Test]
        public async Task DeleteAdjustmentWithStatusCompletedSuccess()
        {
            var handler = new CancelAdjustmentCommandHandler(repository.Object, dtService.Object);
            var request = new CancelAdjustmentCommand
            {
                JobNo = CompletedAdjustment.JobNo,
                UserCode = UserCodes[1]
            };
            var originalStatus = CompletedAdjustment.Status;

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.That(result.Status, Is.EqualTo(InventoryAdjustmentStatus.Cancelled));
            Assert.That(originalStatus, Is.Not.EqualTo(result.Status));
            Assert.That(result.CancelledBy, Is.EqualTo(request.UserCode));
            Assert.That(result.CancelledDate, Is.EqualTo(now));

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(CompletedAdjustment), Times.Once);
        }

        [Test]
        public async Task IncorrectUserCode()
        {
            var handler = new CancelAdjustmentCommandHandler(repository.Object, dtService.Object);
            var request = new CancelAdjustmentCommand
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
            var handler = new CancelAdjustmentCommandHandler(repository.Object, dtService.Object);
            var request = new CancelAdjustmentCommand
            {
                JobNo = "ADJ77777777", // unknown
                UserCode = UserCodes[1]
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }
    }
}
