using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.AdjustmentItems.Commands.DeleteAdjustmentItemCommand;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.AdjustmentItems
{
    public class DeleteAdjustmentItemUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IAdjustmentItemRepository> adjustmentItemRepository;
        private Mock<IRepository> repository;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
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


        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            adjustmentItemRepository = new Mock<IAdjustmentItemRepository>();
            repository = new Mock<IRepository>();
            
            repository.Setup(r => r.Adjustments).Returns(adjustmentRepository.Object);
            repository.Setup(r => r.AdjustmentItems).Returns(adjustmentItemRepository.Object);
            var existingAdjustments = new List<Adjustment>()
            {
                NewAdjustment, ProcessingAdjustment, CompletedAdjustment, CancelledAdjustment, UndoZeroOutAdjustment
            };
            foreach(var existingAdjustment in existingAdjustments)
            {
                adjustmentRepository.Setup(r => r.GetAdjustmentDetails(existingAdjustment.JobNo)).Returns(existingAdjustment);                
            }
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<bool>())).Returns(new List<AdjustmentItem>());
        }

        [Test]
        public async Task DeleteFromProcessingAdjustment()
        {
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(ProcessingAdjustment.JobNo, It.IsAny<string?>(), It.IsAny<bool>())).Returns(new List<AdjustmentItem>());

            var handler = new DeleteAdjustmentItemCommandHandler(repository.Object);
            var request = new DeleteAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                LineItem = 1
            };

            await handler.Handle(request, CancellationToken.None);

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentItemRepository.Verify(r => r.Delete(request.JobNo, request.LineItem), Times.Once());
            adjustmentRepository.Verify(r => r.Update(ProcessingAdjustment), Times.Once());

            Assert.That(ProcessingAdjustment.Status.Equals(InventoryAdjustmentStatus.New));
            //undo changing status
            ProcessingAdjustment.Status = InventoryAdjustmentStatus.Processing;
        }


        [Test]
        public async Task DeleteFromProcessingAdjustment2()
        {
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(ProcessingAdjustment.JobNo, It.IsAny<string?>(), It.IsAny<bool>())).Returns(new List<AdjustmentItem>() { new AdjustmentItem()});

            var handler = new DeleteAdjustmentItemCommandHandler(repository.Object);
            var request = new DeleteAdjustmentItemCommand
            {
                JobNo = ProcessingAdjustment.JobNo,
                LineItem = 1
            };

            await handler.Handle(request, CancellationToken.None);

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(request.JobNo), Times.Once());
            adjustmentItemRepository.Verify(r => r.Delete(request.JobNo, request.LineItem), Times.Once());
            adjustmentRepository.Verify(r => r.Update(ProcessingAdjustment), Times.Never());

            Assert.That(ProcessingAdjustment.Status.Equals(InventoryAdjustmentStatus.Processing));
        }

        [Test]
        public async Task DeleteItemForCompletedOrCancelledAdjustmentFailed()
        {
            adjustmentItemRepository.Setup(r => r.GetAdjustmentItems(ProcessingAdjustment.JobNo, It.IsAny<string?>(), It.IsAny<bool>())).Returns(new List<AdjustmentItem>());

            var handler = new DeleteAdjustmentItemCommandHandler(repository.Object);
            var request = new DeleteAdjustmentItemCommand
            {
                JobNo = CompletedAdjustment.JobNo,
                LineItem = 1
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();

            adjustmentRepository.Verify(r => r.GetAdjustmentDetails(request.JobNo), Times.Once());

            request = new DeleteAdjustmentItemCommand
            {
                JobNo = CancelledAdjustment.JobNo,
                LineItem = 1
            };

            act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalAdjustmentChangeException>();
        }
    }
}
