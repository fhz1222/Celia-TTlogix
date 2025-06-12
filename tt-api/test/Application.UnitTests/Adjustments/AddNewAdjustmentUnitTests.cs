using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services;
using Application.UseCases.Adjustments.Commands.AddNewAdjustmentCommand;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;
namespace Application.UnitTests.Adjustments
{
    public class AddNewAdjustmentUnitTests
    {
        private Mock<IAdjustmentRepository> adjustmentRepository;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository;
        private Mock<IDateTime> dateTimeService;

        private const string AdjustmentPrefix = "ADJ";
        private static string[] UserCodes = new string[] { "0001", "0002" };
        private static string[] WhsCodes = new string[] { "IT", "DE", "PL" };
        private static string[] CustomerCodes = new string[] { "AAA", "BBB" };

        [SetUp]
        public void Setup()
        {
            adjustmentRepository = new Mock<IAdjustmentRepository>();
            utilsRepository = new Mock<IUtilsRepository>();
            repository = new Mock<IRepository>();
            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            repository.Setup(_ => _.Adjustments).Returns(adjustmentRepository.Object);
            utilsRepository.Setup(_ => _.GetJobCode(Domain.Enums.CodePrefix.InventoryAdjustment)).Returns(AdjustmentPrefix);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsIn(UserCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfUserCodeExists(It.IsNotIn(UserCodes))).Returns(false);
            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsIn(WhsCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfWhsCodeExists(It.IsNotIn(WhsCodes))).Returns(false);
            utilsRepository.Setup(_ => _.CheckIfCustomerCodeExists(It.IsIn(CustomerCodes))).Returns(true);
            utilsRepository.Setup(_ => _.CheckIfCustomerCodeExists(It.IsNotIn(CustomerCodes))).Returns(false);
            dateTimeService = new Mock<IDateTime>();
            var currentDate = DateTime.Now;
            dateTimeService.Setup(_ => _.Now).Returns(currentDate);
        }

        [Test]
        public async Task AddNewJobTypeNormal()
        {
            await AddNewAdjustmentFullPath(WhsCodes[0], CustomerCodes[0], UserCodes[0], InventoryAdjustmentJobType.Normal);
        }

        [Test]
        public async Task AddNewJobTypeNormal2()
        {
            await AddNewAdjustmentSimple(WhsCodes[2], CustomerCodes[1], UserCodes[1], InventoryAdjustmentJobType.Normal);
        }

        [Test]
        public async Task AddNewJobTypeUndoZeroOut()
        {
            await AddNewAdjustmentFullPath(WhsCodes[1], CustomerCodes[1], UserCodes[1], InventoryAdjustmentJobType.UndoZeroOut);
        }

        [Test]
        public async Task IncorrectWhsCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();
            
            var handler = new AddNewAdjustmentCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewAdjustmentCommand
            {
                CustomerCode = CustomerCodes[0],
                WhsCode = "WC",
                UserCode = UserCodes[1],
                IsUndoZeroOut = false
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownWhsCodeException>();
        }

        [Test]
        public async Task IncorrectCustomerCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();

            var handler = new AddNewAdjustmentCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewAdjustmentCommand
            {
                CustomerCode = "Customer Code",
                WhsCode = WhsCodes[0],
                UserCode = UserCodes[1],
                IsUndoZeroOut = false
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownCustomerCodeException>();
        }


        [Test]
        public async Task IncorrectUserCode()
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();

            var handler = new AddNewAdjustmentCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewAdjustmentCommand
            {
                CustomerCode = CustomerCodes[1],
                WhsCode = WhsCodes[0],
                UserCode = "ksh",
                IsUndoZeroOut = true
            };

            var act = async () => await handler.Handle(request, CancellationToken.None);
            await act.Should().ThrowAsync<UnknownUserCodeException>();
        }


        private async Task AddNewAdjustmentFullPath(string whsCode, string customerCode, string userCode, InventoryAdjustmentJobType jobType)
        {
            var currentDate = DateTime.Now.Date;
            string jobCodePrefix1 = AdjustmentPrefix + currentDate.Year.ToString() + currentDate.Month.ToString().PadLeft(2, '0');

            adjustmentRepository.Setup(_ => _.GetLastJobNumber(jobCodePrefix1)).Returns(2);
            var jobNumberGenerator = new JobNumberGenerator(repository.Object, dateTimeService.Object);
            var generatedJobNo = jobNumberGenerator.GetJobNumber(adjustmentRepository.Object);

            adjustmentRepository.Setup(_ => _.AddNewAdjustment(It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<string>()
                    , It.IsNotNull<InventoryAdjustmentJobType>(), It.IsNotNull<InventoryAdjustmentStatus>(), generatedJobNo, CancellationToken.None))
                .Returns(Task.FromResult(generatedJobNo));

            var handler = new AddNewAdjustmentCommandHandler(jobNumberGenerator, repository.Object);
            var request = new AddNewAdjustmentCommand
            {
                CustomerCode = customerCode,
                WhsCode = whsCode,
                UserCode = userCode,
                IsUndoZeroOut = jobType.Equals(InventoryAdjustmentJobType.UndoZeroOut)
            };

            var jobNo = await handler.Handle(request, CancellationToken.None);
            Assert.That(jobNo.Equals(generatedJobNo));
            adjustmentRepository.Verify(a => a.AddNewAdjustment(request.WhsCode, request.CustomerCode, request.UserCode, jobType,
                InventoryAdjustmentStatus.New, generatedJobNo, CancellationToken.None), Times.Once);
        }

        private async Task AddNewAdjustmentSimple(string whsCode, string customerCode, string userCode, InventoryAdjustmentJobType jobType)
        {
            var jobNumberGenerator = new Mock<IJobNumberGenerator>();
            var newJobNumber = "ADJ20221000012";
            jobNumberGenerator.Setup(_ => _.GetJobNumber(adjustmentRepository.Object)).Returns(newJobNumber);

            adjustmentRepository.Setup(_ => _.AddNewAdjustment(It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<string>()
                    , It.IsNotNull<InventoryAdjustmentJobType>(), It.IsNotNull<InventoryAdjustmentStatus>(), newJobNumber, CancellationToken.None))
                .Returns(Task.FromResult(newJobNumber));

            var handler = new AddNewAdjustmentCommandHandler(jobNumberGenerator.Object, repository.Object);
            var request = new AddNewAdjustmentCommand
            {
                CustomerCode = customerCode,
                WhsCode = whsCode,
                UserCode = userCode,
                IsUndoZeroOut = jobType.Equals(InventoryAdjustmentJobType.UndoZeroOut)
            };

            var jobNo = await handler.Handle(request, CancellationToken.None);
            Assert.That(jobNo.Equals(newJobNumber));
            adjustmentRepository.Verify(a => a.AddNewAdjustment(request.WhsCode, request.CustomerCode, request.UserCode, jobType,
                InventoryAdjustmentStatus.New, newJobNumber, CancellationToken.None), Times.Once);
        }

    }
}
