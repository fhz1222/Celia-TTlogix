using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Adjustments.Commands.UpdateAdjustmentCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Adjustments
{
    public class UpdateAdjustmentUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository;

        private static string[] UserCodes = new string[] { "0001", "0002" };

        private Adjustment[] ExistingAdjustments = new Adjustment[]
        {
            new Adjustment
            {
                JobNo = "ADJ20221000001",
                JobType = InventoryAdjustmentJobType.Normal,
                Status = InventoryAdjustmentStatus.New,
                CreatedDate = DateTime.Now,
                CreatedBy = UserCodes[0]
            },
            new Adjustment
            {
                JobNo = "ADJ20221000011",
                JobType = InventoryAdjustmentJobType.Normal,
                Status = InventoryAdjustmentStatus.Completed,
                CreatedDate = DateTime.Now,
                CreatedBy = UserCodes[0],
                CompletedBy = UserCodes[1],
                CompletedDate = DateTime.Now,
            },
            new Adjustment
            {
                JobNo = "ADJ20221000020",
                JobType = InventoryAdjustmentJobType.UndoZeroOut,
                Status = InventoryAdjustmentStatus.Cancelled,
                CreatedDate = DateTime.Now.AddDays(-1) ,
                CreatedBy = UserCodes[0],
                CancelledBy = UserCodes[0],
                CancelledDate = DateTime.Now,
            }
        };

        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            repository = new Mock<IRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            repository.Setup(_ => _.Adjustments).Returns(adjustmentRepository.Object);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);

            foreach (var adjustment in ExistingAdjustments)
            {
                adjustmentRepository.Setup(_ => _.GetAdjustmentDetails(It.IsIn(adjustment.JobNo))).Returns(adjustment);
            }
        }

        [Test]
        public async Task UpdateSuccess()
        {
            var handler = new UpdateAdjustmentCommandHandler( repository.Object);
            
            var request = new UpdateAdjustmentCommand
            {
                Adjustment = new UpdatedAdjustmentVM
                {
                    JobNo = ExistingAdjustments[0].JobNo,
                    ReferenceNo = "RefNo1",
                    Reason = "Adjustment reason"
                }
            };
            var originalStatus = ExistingAdjustments[0].Status;
            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.That(result.ReferenceNo == request.Adjustment.ReferenceNo && result.Reason == request.Adjustment.Reason);
            Assert.That(result.Status.Equals(originalStatus));

            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.Adjustment.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(ExistingAdjustments[0]), Times.Once);
        }

        [Test]
        public async Task IncorrectJobNumber()
        {
            var handler = new UpdateAdjustmentCommandHandler(repository.Object);
            var request = new UpdateAdjustmentCommand
            {
                Adjustment = new UpdatedAdjustmentVM
                {
                    JobNo = "ADJ001",
                    ReferenceNo = "RefNo1",
                    Reason = "Adjustment reason"
                }
            };
            
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.Adjustment.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }

        [Test]
        public async Task MissingRequiredField()
        {
            var handler = new UpdateAdjustmentCommandHandler(repository.Object);
            var request = new UpdateAdjustmentCommand
            {
                Adjustment = new UpdatedAdjustmentVM
                {
                    JobNo = ExistingAdjustments[0].JobNo,
                    ReferenceNo = " "
                }
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<RequiredFieldException>();

            adjustmentRepository.Verify(a => a.GetAdjustmentDetails(request.Adjustment.JobNo), Times.Once);
            adjustmentRepository.Verify(a => a.Update(It.IsAny<Adjustment>()), Times.Never);
        }
    }
}
