using Microsoft.AspNetCore.Http;
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
    public class InventoryServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task CreatePartMaster_PartMasterAlreadyExists()
        {
            var partMasterDto = new Models.PartMasterDto
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                ProductCode2 = "productCode2",
                ProductCode3 = "productCode3",
                ProductCode4 = "productCode4",
                Description = "description",
                UOM = "uom",
                OriginCountry = "country",
                Status = Core.Enums.ValueStatus.Active,
                EnableSerialNo = true,
                PackageType = "packageType",
                IsStandardPackaging = true,
                IsDefected = true,
                SPQ = 100,
                OrderLot = 20.20m,
                Length = 1.1m,
                Width = 2.2m,
                Height = 3.3m,
                NetWeight = 40.4m,
                GrossWeight = 50.5m,
                IsPalletItem = true,
                IsCPart = true,
                CPartSPQ = 10,
                MasterSlave = true,
                BoxItem = true,
                FloorStackability = 1,
                TruckStackability = 2,
                BoxesInPallet = 4,
                DoNotSyncEDI = true,
                SupplierName = "supplierName"
            };
            using (var context = new Context(options, appSettings.Object))
            {
                context.UOMs.Add(new Core.Entities.UOM { Code = partMasterDto.UOM, Status = 1 });

                context.PartMasters.Add(new Core.Entities.PartMaster
                {
                    CustomerCode = partMasterDto.CustomerCode,
                    SupplierID = partMasterDto.SupplierID,
                    ProductCode1 = partMasterDto.ProductCode1,
                    UOM = partMasterDto.UOM,
                    BoxesInPallet = 1,
                    BoxItem = true,
                    CPartSPQ = 10,
                    CreatedBy = "usercode1",
                    CreatedDate = DateTime.Now,
                    Description = "desc",     
                });
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                using (var mrpContext = new MRPContext(mrpoptions))
                {
                    var result = await GetInventoryService(context, mrpContext).CreatePartMaster(partMasterDto, "userCode1");

                    Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                }
            }
        }

        [TestMethod]
        public async Task CreatePartMaster()
        {
            var partMasterDto = new Models.PartMasterDto
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                ProductCode2 = "productCode2",
                ProductCode3 = "productCode3",
                ProductCode4 = "productCode4",
                Description = "description",
                UOM = "uom",
                OriginCountry = "country",
                Status = Core.Enums.ValueStatus.Active,
                EnableSerialNo = true,
                PackageType = "packageType",
                IsStandardPackaging = true,
                IsDefected = true,
                SPQ = 100,
                OrderLot = 20.20m,
                Length = 1.1m,
                Width = 2.2m,
                Height = 3.3m,
                NetWeight = 40.4m,
                GrossWeight = 50.5m,
                IsPalletItem = true,
                IsCPart = true,
                CPartSPQ = 10,
                MasterSlave = true,
                BoxItem = true,
                FloorStackability = 1,
                TruckStackability = 2,
                BoxesInPallet = 4,
                DoNotSyncEDI = true,
                SupplierName = "supplierName"
            };
            using (var context = new Context(options, appSettings.Object))
            {
                context.UOMs.Add(new Core.Entities.UOM { Code = partMasterDto.UOM, Name = "uomName", Status = 1 });
                context.Warehouses.Add(new Core.Entities.Warehouse { Code = "WH1" });
                context.Warehouses.Add(new Core.Entities.Warehouse { Code = "WH2" });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster { SupplierID = supplierID, FactoryID = factoryID, CompanyName = "companyName" });
                context.SupplierDetails.Add(new Core.Entities.SupplierDetail { SupplierID = supplierID, FactoryID = factoryID });
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                using (var mrpContext = new MRPContext(mrpoptions))
                {
                    var result = await GetInventoryService(context, mrpContext).CreatePartMaster(partMasterDto, "userCode1");

                    Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                }
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(1, context.PartMasters.Count());
                Assert.AreEqual(4, context.Inventory.Count());
                Assert.AreEqual(1, context.ItemMasters.Count());
                Assert.AreEqual(1, context.SupplierItemMasters.Count());
            }
            using (var mrpContext = new MRPContext(mrpoptions))
            {
                Assert.AreEqual(1, mrpContext.MRPItemMasters.Count());
                Assert.AreEqual(1, mrpContext.MRPInventory.Count());
            }
        }

        private IInventoryService GetInventoryService(Context context, MRPContext mrpContext)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var mrprepository = new SqlMRPRepository(mrpContext);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var accessor = new HttpContextAccessor();
            return new InventoryService(repository, mrprepository, mapper, locker, accessor, new Logger<InventoryService>(loggerFactory));
        }

        private readonly string productCode = "PRODCODE1";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";

    }
}
