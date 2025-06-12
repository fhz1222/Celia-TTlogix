using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Decants.Commands.CancelDecantCommand;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class CancelDecantUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository; 
        private Mock<IDateTime> dtService;
        private DateTime now;

        private static string[] UserCodes = new string[] { "0001", "0002" };
        private Decant NewDecant; 

        private readonly Decant CompletedDecant = new Decant
        {
            JobNo = "DCT20221000011",
            Status = DecantStatus.Completed,
            CreatedDate = new DateTime(2022, 10, 1),
            CreatedBy = UserCodes[0],
            CompletedBy = UserCodes[1],
            CompletedDate = DateTime.Now,
        };

        private readonly Decant CancelledDecant = new Decant
        {
            JobNo = "DCT20221000020",
            Status = DecantStatus.Cancelled,
            CreatedDate = new DateTime(2022, 10, 4),
            CreatedBy = UserCodes[0],
            CancelledBy = UserCodes[0],
            CancelledDate = new DateTime(2022, 10, 5),
        };

        private List<Decant> ExistingDecants;


        [SetUp]
        public void Setup()
        {
            decantRepository = new Mock<IDecantRepository>();
            repository = new Mock<IRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            dtService = new Mock<IDateTime>();

            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            repository.Setup(_ => _.Decant).Returns(decantRepository.Object);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);

            NewDecant = new Decant
            {
                JobNo = "DCT20221000001",
                Status = DecantStatus.New,
                CreatedDate = new DateTime(2022, 10, 3),
                CreatedBy = UserCodes[0]
            };
            ExistingDecants = new List<Decant>();
            ExistingDecants.Add(NewDecant);
            ExistingDecants.Add(CompletedDecant);
            ExistingDecants.Add(CancelledDecant);

            foreach (var decant in ExistingDecants)
            {
                decantRepository.Setup(r => r.GetDecant(It.IsIn(decant.JobNo))).Returns(Task.FromResult((Decant?)decant));
            }

            now = DateTime.Now;
            dtService.Setup(s => s.Now).Returns(now);
        }

        [Test]
        public async Task CancelDecanttWithStatusNewSuccess()
        {
            var handler = new CancelDecantCommandHandler(repository.Object, dtService.Object);
            var request = new CancelDecantCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = UserCodes[1]
            };
            var originalStatus = NewDecant.Status;

            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.That(result.Status, Is.EqualTo(DecantStatus.Cancelled));
            Assert.That(originalStatus, Is.Not.EqualTo(result.Status));
            Assert.That(result.CancelledBy, Is.EqualTo(request.UserCode));
            Assert.That(result.CancelledDate, Is.EqualTo(now));

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            decantRepository.Verify(a => a.GetDecant(request.JobNo), Times.Exactly(2));
            decantRepository.Verify(a => a.UpdateDecant(NewDecant), Times.Once);
        }

        [Test]
        public async Task DeleteDecantWithStatusCompletedFailed()
        {
            var handler = new CancelDecantCommandHandler(repository.Object, dtService.Object);
            var request = new CancelDecantCommand
            {
                JobNo = CompletedDecant.JobNo,
                UserCode = UserCodes[1]
            };
            var originalStatus = CompletedDecant.Status;

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<IllegalDecantChangeException>();

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            decantRepository.Verify(a => a.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(CompletedDecant), Times.Never);
        }

        [Test]
        public async Task IncorrectUserCode()
        {
            var handler = new CancelDecantCommandHandler(repository.Object, dtService.Object);
            var request = new CancelDecantCommand
            {
                JobNo = NewDecant.JobNo,
                UserCode = "?????"
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownUserCodeException>();

            utilsRepository.Verify(a => a.CheckIfUserCodeExists(request.UserCode), Times.Once());
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
        }

        [Test]
        public async Task IncorrectJobNumber()
        {
            var handler = new CancelDecantCommandHandler(repository.Object, dtService.Object);
            var request = new CancelDecantCommand
            {
                JobNo = "DCT77777777", // unknown
                UserCode = UserCodes[1]
            };
            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownJobNoException>();

            decantRepository.Verify(a => a.GetDecant(request.JobNo), Times.Once);
            decantRepository.Verify(a => a.UpdateDecant(It.IsAny<Decant>()), Times.Never);
        }
    }
}
