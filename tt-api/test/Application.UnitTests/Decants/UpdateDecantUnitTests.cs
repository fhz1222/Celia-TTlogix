using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.UseCases.Decants.Commands.UpdateDecantCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class UpdateDecantUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository;

        private static string[] UserCodes = new string[] { "0001", "0002" };

        private Decant NewDecant;
        private Decant ProcessingDecant;
        private Decant CompletedDecant;
        private Decant CancelledDecant;
        private Decant[] ExistingDecants;

        [SetUp]
        public void Setup()
        {
            ;
            decantRepository = new Mock<IDecantRepository>();
            repository = new Mock<IRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            repository.Setup(r => r.Utils).Returns(utilsRepository.Object);
            repository.Setup(r=> r.Decant).Returns(decantRepository.Object);
            utilsRepository.Setup(r => r.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(r => r.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);

            NewDecant = new Decant
            {
                JobNo = "DCT20221000001",
                Status = DecantStatus.New,
                CreatedDate = new DateTime(2022, 10, 3),
                CreatedBy = UserCodes[0]
            };
            ProcessingDecant = new Decant
            {
                JobNo = "DCT20221000002",
                Status = DecantStatus.Processing,
                CreatedDate = new DateTime(2022, 10, 4),
                CreatedBy = UserCodes[0]
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
        }

        [Test]
        public async Task UpdateSuccess()
        {
            var handler = new UpdateDecantCommandHandler( repository.Object);
            
            var request = new UpdateDecantCommand
            {
                JobNo = ExistingDecants[0].JobNo,
                ReferenceNo = "RefNo1",
                Remark = "Decant reason"
            };
            var originalStatus = ExistingDecants[0].Status;
            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.That(result.ReferenceNo == request.ReferenceNo && result.Remark == request.Remark);
            Assert.That(result.Status.Equals(originalStatus));

            decantRepository.Verify(r => r.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(r => r.UpdateDecant(ExistingDecants[0]), Times.Once);
        }

        [Test]
        public async Task IncorrectJobNumber()
        {
            var handler = new UpdateDecantCommandHandler(repository.Object);
            var request = new UpdateDecantCommand
            {
                JobNo = "DCT001",
                ReferenceNo = "RefNo1",
                Remark = "Decant reason"
            };
            
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            decantRepository.Verify(a => a.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
        }
    }
}
