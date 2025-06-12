using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TT.Common;
using TT.Services.Interfaces;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TT.DB;
using System;

namespace TT.Services.Tests
{
    public class TestBase
    {
        protected Mock<IOptions<AppSettings>> appSettings;
        protected Mapper mapper;
        protected Mock<ILoggerService> logger;
        protected DbContextOptions<Context> options;
        protected DbContextOptions<MRPContext> mrpoptions;

        [TestInitialize]
        public virtual void Initialize()
        {
            var builder = new DbContextOptionsBuilder<Context>();
            //builder.EnableNullabilityCheck();// TODO
            options = builder.UseInMemoryDatabase(databaseName: "in_memory_db").Options;
            var mrpbuilder = new DbContextOptionsBuilder<MRPContext>();
            mrpoptions = mrpbuilder.UseInMemoryDatabase(databaseName: "in_memory_mrp_db").Options;
            appSettings = new Mock<IOptions<AppSettings>>();
            appSettings.Setup(s => s.Value).Returns(new AppSettings() { OwnerCode = "TESA", SAPFactories = new string[] { "HU01", "HU11", "PLV" } });

            using (var context = new Context(options, appSettings.Object))
                context.Database.EnsureDeleted();


            logger = new Mock<ILoggerService>();

            var config = new MapperConfiguration(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
            mapper = new Mapper(config);
            config.AssertConfigurationIsValid();
        }

    }
}
