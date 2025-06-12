using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Services;

namespace TT.Services.Tests
{
    [TestClass]
    public class EKanbanServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task CreateEKanbanManual()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }
            string newOrderNo; 
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetEKanbanService(context).CreateEKanbanManual(jobno, factoryID, Models.ModelEnums.ManualType.ManualEHP);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newOrderNo = result.Data;
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var header = context.EKanbanHeaders.Find(newOrderNo);
                Assert.AreEqual((int)EKanbanStatus.Imported, header.Status);
                Assert.AreEqual(factoryID, header.FactoryID);
                Assert.AreEqual("EHP", header.Instructions);
                Assert.AreEqual($"9{DateTime.Now:yyMMdd}001", newOrderNo);
            }
        }

        // TODO this cannot be tested until we rewrite raw sql to linq 
        //[TestMethod]
        public async Task CheckEKanbanFulfillable_False()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetEKanbanService(context).CheckEKanbanFulfillable(new string[] { orderno});
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsFalse(result.Data);
            }
        }

        // TODO this cannot be tested until we rewrite raw sql to linq 
        //[TestMethod]
        public async Task CheckEKanbanFulfillable_True()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetEKanbanService(context).CheckEKanbanFulfillable(new string[] { orderno });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsFalse(result.Data);
            }
        }

        // TODO this cannot be tested until we rewrite raw sql to linq 
        //[TestMethod]
        public async Task GetEKanbanPartsStatusByOwnershipEHPTest()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetEKanbanService(context).GetEKanbanPartsStatusByOwnershipEHP(orderno);
                Assert.IsNotNull(result);
                Assert.AreEqual(8, result.Count());
            }
        }

        private IEKanbanService GetEKanbanService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var utilityService = new UtilityService(repository, appSettings.Object);
            return new EKanbanService(repository, utilityService, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
        }


        private void AddTestData(Context context)
        {
            var ekanbanHeader = new EKanbanHeader()
            {
                OrderNo = orderno,
                FactoryID = "PLT",
                RunNo = "",
                IssuedDate = DateTime.Parse("2012-11-24 04:10:00.000"),
                CreatedDate = DateTime.Parse("2012-11-24 04:13:28.827"),
                ConfirmationDate = null,
                Instructions = "",
                Status = (int)EKanbanStatus.New,
                OutJobNo = "",
                ETA = DateTime.Parse("2012-12-04 00:00:00.000"),
                BlanketOrderNo = "0000023511",
                RefNo = "019924"
            };

            context.EKanbanHeaders.Add(ekanbanHeader);
               var detailJson = @"
                [{ 'OrderNo': '"+ orderno + @"', 'ProductCode': '"+productCode+ @"', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 384.00	},
                { 'OrderNo': '" + orderno + @"', 'ProductCode': '" + productCode + @"', 'SerialNo': '2', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 384.00	},
                { 'OrderNo': '" + orderno + @"', 'ProductCode': '" + productCode + @"', 'SerialNo': '3', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 112.00	},
                { 'OrderNo': '" + orderno + @"', 'ProductCode': '192634700', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 384.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '192634700', 'SerialNo': '2', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 150.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '192901300', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 379.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '192901330', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 332.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '379268504', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 285.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '379268514', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 18.00	},
                { 'OrderNo': '"+ orderno + @"', 'ProductCode': '379268820', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 72.00	},
                { 'OrderNo': '" + orderno + @"', 'ProductCode': '379268830', 'SerialNo': '1', 'SupplierID': '"+supplierID+ @"', 'DropPoint': '01', 'Quantity': 210.00	}]";
            detailJson = detailJson.Replace("'", "\"");

                   var detailRows = JsonSerializer.Deserialize<IEnumerable<EKanbanDetail>>(detailJson);
            foreach(var row in detailRows)
            {
                context.EKanbanDetails.Add(row);
            }
            context.SaveChanges();
        }


        private readonly string jobno = "OUT20200900626";
        private readonly string productCode = "PRODCODE1";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";
        private readonly string orderno = "9200909056";
    }
}
