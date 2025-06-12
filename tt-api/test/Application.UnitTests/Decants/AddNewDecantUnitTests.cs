using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services;
using Application.UseCases.Decants.Commands.AddNewDecantCommand;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Decants
{
    public class AddNewDecantUnitTests
    {
        private Mock<IDecantRepository> decantRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository;
        private Mock<IDateTime> dateTimeService;

        private const string DecantPrefix = "DCT";
        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };

        [SetUp]
        public void Setup()
        {
            decantRepository = new Mock<IDecantRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            repository = new Mock<IRepository>();
            dateTimeService = new Mock<IDateTime>();
            var currentDate = DateTime.Now;
            dateTimeService.Setup(_ => _.Now).Returns(currentDate);

            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            repository.Setup(_ => _.Decant).Returns(decantRepository.Object);
            
            utilsRepository.Setup(_ => _.GetJobCode(Domain.Enums.CodePrefix.Decant)).Returns(DecantPrefix);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);
            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsIn(WhsCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsNotIn(WhsCodes))).Returns(false);
            utilsRepository.Setup(_ => _.CheckIfCustomerCodeExists(It.IsIn(CustomerCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfCustomerCodeExists(It.IsNotIn(CustomerCodes))).Returns(false);
        }

        [Test]
        public async Task AddNewDecant()
        {
            var currentDate = DateTime.Now.Date;
            string jobCodePrefix1 = DecantPrefix + currentDate.Year.ToString() + currentDate.Month.ToString().PadLeft(2, '0');

            decantRepository.Setup(_ => _.GetLastJobNumber(jobCodePrefix1)).Returns(2);
            var jobNumberGenerator = new JobNumberGenerator(repository.Object, dateTimeService.Object);
            var generatedJobNo = jobNumberGenerator.GetJobNumber(decantRepository.Object);

            var handler = new AddNewDecantCommandHandler(jobNumberGenerator, repository.Object);
            var request = new AddNewDecantCommand
            {
                CustomerCode = CustomerCodes[0],
                WhsCode = WhsCodes[0],
                UserCode = UserCodes[0]
            };

            var decant = await handler.Handle(request, CancellationToken.None);
            Assert.That(!decant.Equals(null));
            Assert.That(decant.JobNo.Equals(generatedJobNo));
            Assert.That(decant.CreatedBy.Equals(request.UserCode));
            Assert.That(decant.WhsCode.Equals(request.WhsCode));
            Assert.That(decant.CustomerCode.Equals(request.CustomerCode));
            decantRepository.Verify(a => a.AddNewDecant(decant), Times.Once);
        
        }


        [Test]
        public async Task IncorrectWhsCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();
            
            var handler = new AddNewDecantCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewDecantCommand
            {
                CustomerCode = CustomerCodes[0],
                WhsCode = "WC",
                UserCode = UserCodes[1]
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownWhsCodeException>();
        }

        [Test]
        public async Task IncorrectCustomerCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();

            var handler = new AddNewDecantCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewDecantCommand
            {
                CustomerCode = "Customer Code",
                WhsCode = WhsCodes[0],
                UserCode = UserCodes[1]
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownCustomerCodeException>();
        }


        [Test]
        public async Task IncorrectUserCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();

            var handler = new AddNewDecantCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewDecantCommand
            {
                CustomerCode = CustomerCodes[1],
                WhsCode = WhsCodes[0],
                UserCode = "ksh",
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownUserCodeException>();
        }
    }
}
