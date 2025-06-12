using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services;
using Moq;

namespace Application.UnitTests.Adjustments
{
    public class JobNumberGeneratorUnitTests
    {
        private Mock<IJobNumberSource> jobNumberSource;
        private Mock<IUtilsRepository> utilsRepository;
        private Mock<IRepository> repository;
        private Mock<IDateTime> dateTimeService;

        private const string AdjustmentPrefix = "ADJ";
        [SetUp]
        public void Setup()
        {
            jobNumberSource = new Mock<IJobNumberSource>();
            utilsRepository = new Mock<IUtilsRepository>();
            repository = new Mock<IRepository>();
            
            repository.Setup(_ => _.Utils).Returns(utilsRepository.Object);
            utilsRepository.Setup(_ => _.GetJobCode(Domain.Enums.CodePrefix.InventoryAdjustment)).Returns(AdjustmentPrefix);
            jobNumberSource.Setup(_ => _.GetCodePrefix).Returns(Domain.Enums.CodePrefix.InventoryAdjustment);
            dateTimeService = new Mock<IDateTime>();
            var currentDate = DateTime.Now;
            dateTimeService.Setup(_ => _.Now).Returns(currentDate); 
        }

        [Test]
        public void CheckCodeGeneratorIfLastIs0()
        {
            CheckCodeGeneratorForLast(0);
        }

        [Test]
        public void CheckCodeGeneratorIfLastIs100()
        {
            CheckCodeGeneratorForLast(100);
        }

        private void CheckCodeGeneratorForLast(int value)
        {

            var currentDate = dateTimeService.Object.Now;
            string jobCodePrefix = AdjustmentPrefix + currentDate.Year.ToString() + currentDate.Month.ToString().PadLeft(2, '0');

            jobNumberSource.Setup(_ => _.GetLastJobNumber(jobCodePrefix)).Returns(value);

            var jobNumberGenerator = new JobNumberGenerator(repository.Object, dateTimeService.Object);

            var expectedJobNo = jobCodePrefix + (value + 1).ToString().PadLeft(14 - jobCodePrefix.Length, '0');
            var generatedJobNo = jobNumberGenerator.GetJobNumber(jobNumberSource.Object);
            Assert.That(expectedJobNo.Equals(generatedJobNo));
            Assert.True(generatedJobNo.Length == 14);
            var jobNoIntValue = int.Parse(generatedJobNo.Substring(jobCodePrefix.Length));
            Assert.True(jobNoIntValue == value + 1);
        }
    }
}
