using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Services;

namespace TT.Services.Tests
{
    [TestClass]
    public class PriceMasterServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task UpdatePriceMasterInbound_CurrencyNotExists()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterInbound(new Models.UpdatePriceMasterInboundDetailsDto[]
                { new Models.UpdatePriceMasterInboundDetailsDto{
                    Price = 100.7777777m,
                    ProductCode = productCode
                } }, factoryID, supplierID, inJobNo, "userCode1", "XXX");

                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterInbound_QuantityZero()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterInbound(new Models.UpdatePriceMasterInboundDetailsDto[]
                { new Models.UpdatePriceMasterInboundDetailsDto{
                    Price = 0,
                    ProductCode = productCode
                } }, factoryID, supplierID, inJobNo, "userCode1", "EUR");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNull(priceMaster);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterInbound_AddNewRecord()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterInbound(new Models.UpdatePriceMasterInboundDetailsDto[]
                { new Models.UpdatePriceMasterInboundDetailsDto{
                    Price = 100.7777777m,
                    ProductCode = productCode
                } }, factoryID, supplierID, inJobNo, "userCode1", "EUR");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNotNull(priceMaster);
                Assert.AreEqual(100.777777m, priceMaster.BuyingPrice);
                Assert.AreEqual(100.777777m, priceMaster.SellingPrice);
                Assert.AreEqual(factoryID, priceMaster.CustomerCode);
                Assert.AreEqual(supplierID, priceMaster.SupplierID);
                Assert.AreEqual(productCode, priceMaster.ProductCode1);
                Assert.AreEqual("EUR", priceMaster.Currency);
                Assert.AreEqual(inJobNo, priceMaster.LastUpdatedInbound);
                Assert.AreEqual(inJobNo, priceMaster.LastUpdatedOutbound);
                Assert.AreEqual(string.Empty, priceMaster.RevisedBy);
                Assert.AreEqual(string.Empty, priceMaster.OutRevisedBy);
                Assert.AreEqual(null, priceMaster.RevisedDate);
                Assert.AreEqual(null, priceMaster.OutRevisedDate);
                Assert.AreEqual("userCode1", priceMaster.CreatedBy);
                Assert.IsTrue(now < priceMaster.CreatedDate);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterInbound_UpdateExistingRecord()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                context.PriceMasters.Add(new Core.Entities.PriceMaster
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Currency = "USD",
                    BuyingPrice = 500.123m,
                    SellingPrice = 600.124m,
                    CreatedBy = "CREATEDBY",
                    CreatedDate = now.AddDays(-1),
                    LastUpdatedInbound = "INJOB",
                    LastUpdatedOutbound = "INJOB"
                }); ;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterInbound(new Models.UpdatePriceMasterInboundDetailsDto[]
                { new Models.UpdatePriceMasterInboundDetailsDto{
                    Price = 100.7777777m,
                    ProductCode = productCode
                } }, factoryID, supplierID, inJobNo, "userCode1", "EUR");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNotNull(priceMaster);
                Assert.AreEqual(100.777777m, priceMaster.BuyingPrice);
                Assert.AreEqual(600.124m, priceMaster.SellingPrice);
                Assert.AreEqual(factoryID, priceMaster.CustomerCode);
                Assert.AreEqual(supplierID, priceMaster.SupplierID);
                Assert.AreEqual(productCode, priceMaster.ProductCode1);
                Assert.AreEqual("EUR", priceMaster.Currency);
                Assert.AreEqual(inJobNo, priceMaster.LastUpdatedInbound);
                Assert.AreEqual("INJOB", priceMaster.LastUpdatedOutbound);
                Assert.AreEqual("CREATEDBY", priceMaster.CreatedBy);
                Assert.AreEqual("userCode1", priceMaster.RevisedBy);
                Assert.AreEqual(string.Empty, priceMaster.OutRevisedBy);
                Assert.AreEqual(null, priceMaster.OutRevisedDate);
                Assert.IsTrue(now < priceMaster.RevisedDate);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterOutbound_PriceIsEmpty()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                context.StorageDetails.Add(new Core.Entities.StorageDetail
                {
                    PID = pid,
                    InJobNo = inJobNo,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    Ownership = Core.Enums.Ownership.Supplier,
                    Status = Core.Enums.StorageStatus.Picked,
                    Qty = 1000,
                    BuyingPrice = 100,
                    SellingPrice = 200
                }); ;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterOutbound(new Models.UpdatePriceMasterPickingListDto[]
                { new Models.UpdatePriceMasterPickingListDto
                {
                    PID = pid,
                    SupplierId = supplierID,
                    Price = 0,
                    ProductCode = productCode
                }
                }, factoryID, outJobNo, "userCode1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNull(priceMaster);

                // check if storage detail price was NOT updated 
                var sd = context.StorageDetails.Single();
                Assert.AreEqual(200m, sd.SellingPrice);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterOutbound_AddNewRecord()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                context.StorageDetails.Add(new Core.Entities.StorageDetail
                {
                    PID = pid,
                    InJobNo = inJobNo,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    Ownership = Core.Enums.Ownership.Supplier,
                    Status = Core.Enums.StorageStatus.Picked,
                    Qty = 1000,
                    BuyingPrice = 100,
                    SellingPrice = 200
                }); ;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterOutbound(new Models.UpdatePriceMasterPickingListDto[]
                { new Models.UpdatePriceMasterPickingListDto
                {
                    PID = pid,
                    SupplierId = supplierID,
                    Price = 100.7777777m,
                    ProductCode = productCode            
                } 
                }, factoryID, outJobNo, "userCode1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNotNull(priceMaster);
                Assert.AreEqual(100.777777m, priceMaster.BuyingPrice);
                Assert.AreEqual(100.777777m, priceMaster.SellingPrice);
                Assert.AreEqual(factoryID, priceMaster.CustomerCode);
                Assert.AreEqual(supplierID, priceMaster.SupplierID);
                Assert.AreEqual(productCode, priceMaster.ProductCode1);
                Assert.AreEqual("", priceMaster.Currency);
                Assert.AreEqual("", priceMaster.LastUpdatedInbound);
                Assert.AreEqual(outJobNo, priceMaster.LastUpdatedOutbound);
                Assert.AreEqual(string.Empty, priceMaster.RevisedBy);
                Assert.AreEqual("userCode1", priceMaster.OutRevisedBy);
                Assert.AreEqual(null, priceMaster.RevisedDate);
                Assert.IsTrue(now < priceMaster.OutRevisedDate);
                Assert.AreEqual("userCode1", priceMaster.CreatedBy);
                Assert.IsTrue(now < priceMaster.CreatedDate);

                // check if storage detail price was updated 
                var sd = context.StorageDetails.Single();
                Assert.AreEqual(100.777777m, sd.SellingPrice);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterOutbound_UpdateExistingRecord_ChangeSellingPrice()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                context.PriceMasters.Add(new Core.Entities.PriceMaster
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Currency = "USD",
                    BuyingPrice = 500.123m,
                    SellingPrice = 600.124m,
                    CreatedBy = "CREATEDBY",
                    CreatedDate = now.AddDays(-1),
                    LastUpdatedInbound = "INJOB",
                    LastUpdatedOutbound = "INJOB"
                }); ;
                context.StorageDetails.Add(new Core.Entities.StorageDetail
                {
                    PID = pid,
                    InJobNo = inJobNo,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    Ownership = Core.Enums.Ownership.Supplier,
                    Status = Core.Enums.StorageStatus.Picked,
                    Qty = 1000,
                    BuyingPrice = 100,
                    SellingPrice = 200
                }); ;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterOutbound(new Models.UpdatePriceMasterPickingListDto[]
                { new Models.UpdatePriceMasterPickingListDto
                {
                    PID = pid,
                    SupplierId = supplierID,
                    Price = 100.7777777m,
                    ProductCode = productCode
                }
                }, factoryID, outJobNo, "userCode1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNotNull(priceMaster);
                Assert.AreEqual(500.123m, priceMaster.BuyingPrice);
                Assert.AreEqual(100.777777m, priceMaster.SellingPrice);
                Assert.AreEqual(factoryID, priceMaster.CustomerCode);
                Assert.AreEqual(supplierID, priceMaster.SupplierID);
                Assert.AreEqual(productCode, priceMaster.ProductCode1);
                Assert.AreEqual("USD", priceMaster.Currency);
                Assert.AreEqual("INJOB", priceMaster.LastUpdatedInbound);
                Assert.AreEqual(outJobNo, priceMaster.LastUpdatedOutbound);
                Assert.AreEqual(string.Empty, priceMaster.RevisedBy);
                Assert.AreEqual("userCode1", priceMaster.OutRevisedBy);
                Assert.AreEqual(null, priceMaster.RevisedDate);
                Assert.IsTrue(now < priceMaster.OutRevisedDate);
                Assert.AreEqual("CREATEDBY", priceMaster.CreatedBy);
                Assert.IsTrue(now > priceMaster.CreatedDate);

                // check if storage detail price was updated 
                var sd = context.StorageDetails.Single();
                Assert.AreEqual(100.777777m, sd.SellingPrice);
            }
        }

        [TestMethod]
        public async Task UpdatePriceMasterOutbound_UpdateExistingRecord_DoNotChangeSellingPrice()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                context.PriceMasters.Add(new Core.Entities.PriceMaster
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Currency = "USD",
                    BuyingPrice = 500.123m,
                    SellingPrice = 100.777777m,
                    CreatedBy = "CREATEDBY",
                    CreatedDate = now.AddDays(-1),
                    LastUpdatedInbound = "INJOB",
                    LastUpdatedOutbound = "INJOB"
                }); ;
                context.StorageDetails.Add(new Core.Entities.StorageDetail
                {
                    PID = pid,
                    InJobNo = inJobNo,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    Ownership = Core.Enums.Ownership.Supplier,
                    Status = Core.Enums.StorageStatus.Picked,
                    Qty = 1000,
                    BuyingPrice = 100,
                    SellingPrice = 200
                }); ;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPriceMasterService(context).UpdatePriceMasterOutbound(new Models.UpdatePriceMasterPickingListDto[]
                { new Models.UpdatePriceMasterPickingListDto
                {
                    PID = pid,
                    SupplierId = supplierID,
                    Price = 100.7777777m,
                    ProductCode = productCode
                }
                }, factoryID, outJobNo, "userCode1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var priceMaster = context.PriceMasters.SingleOrDefault();
                Assert.IsNotNull(priceMaster);
                Assert.AreEqual(500.123m, priceMaster.BuyingPrice);
                Assert.AreEqual(100.777777m, priceMaster.SellingPrice);
                Assert.AreEqual(factoryID, priceMaster.CustomerCode);
                Assert.AreEqual(supplierID, priceMaster.SupplierID);
                Assert.AreEqual(productCode, priceMaster.ProductCode1);
                Assert.AreEqual("USD", priceMaster.Currency);
                Assert.AreEqual("INJOB", priceMaster.LastUpdatedInbound);
                Assert.AreEqual(outJobNo, priceMaster.LastUpdatedOutbound);
                Assert.AreEqual(string.Empty, priceMaster.RevisedBy);
                Assert.AreEqual("userCode1", priceMaster.OutRevisedBy);
                Assert.AreEqual(null, priceMaster.RevisedDate);
                Assert.IsTrue(now < priceMaster.OutRevisedDate);
                Assert.AreEqual("CREATEDBY", priceMaster.CreatedBy);
                Assert.IsTrue(now > priceMaster.CreatedDate);

                // check if storage detail price was NOT updated 
                var sd = context.StorageDetails.Single();
                Assert.AreEqual(200, sd.SellingPrice);
            }
        }

        private IPriceMasterService GetPriceMasterService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            return new PriceMasterService(repository, locker, new Logger<PriceMasterService>(loggerFactory));
        }

        private readonly string inJobNo = "INB20200900626";
        private readonly string outJobNo = "OUT20200900626";
        private readonly string pid = "PID00001";
        private readonly string productCode = "PRODCODE1";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";

    }
}
