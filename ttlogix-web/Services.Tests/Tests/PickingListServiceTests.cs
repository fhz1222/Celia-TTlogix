using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Services.Tests
{
    [TestClass]
    public class PickingListServiceTests : TestBase
    {
        private Mock<IILogConnect> _iLogConnect;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _iLogConnect = new Mock<IILogConnect>();
            _iLogConnect.Setup(x => x.IsProcessingOutbound(It.IsAny<string>())).ReturnsAsync(false);
        }

        //[TestMethod]
        public async Task AutoAllocate_QtyGreaterThanRequired()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 20);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var pickingListService = GetPickingListService(context);
                var result = await pickingListService.AutoAllocate(new AllocationDto
                {
                    JobNo = jobno,
                    LineItem = 1
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(1, context.PickingLists.Count());
                var pl = context.PickingLists.First();
                Assert.AreEqual(150, pl.Qty);
                Assert.AreEqual("", pl.PID);
                Assert.AreEqual("L4", pl.LocationCode);
                Assert.AreEqual(whscode, pl.WHSCode);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-30), pl.InboundDate);

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual(150, outboundDetail.Qty);
                Assert.AreEqual(150, outboundDetail.PickedQty);
                Assert.AreEqual(1, outboundDetail.PickedPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(150, storageDetail.AllocatedQty);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail.Status);
            }
        }

        //[TestMethod]
        public async Task AutoAllocate_QtyLessThanRequired()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 500);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var pickingListService = GetPickingListService(context);
                var result = await pickingListService.AutoAllocate(new AllocationDto
                {
                    JobNo = jobno,
                    LineItem = 1
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(3, context.PickingLists.Count());
                var pls = context.PickingLists.ToArray();
                Assert.AreEqual(150, pls[0].Qty);
                Assert.AreEqual("", pls[0].PID);
                Assert.AreEqual("L4", pls[0].LocationCode);
                Assert.AreEqual(whscode, pls[0].WHSCode);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-30), pls[0].InboundDate);

                Assert.AreEqual(100, pls[1].Qty);
                Assert.AreEqual("", pls[1].PID);
                Assert.AreEqual("L5", pls[1].LocationCode);
                Assert.AreEqual(whscode, pls[1].WHSCode);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-20), pls[1].InboundDate);

                Assert.AreEqual(50, pls[2].Qty);
                Assert.AreEqual("", pls[2].PID);
                Assert.AreEqual("L6", pls[2].LocationCode);
                Assert.AreEqual(whscode, pls[2].WHSCode);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-10), pls[2].InboundDate);

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual(500, outboundDetail.Qty);
                Assert.AreEqual(300, outboundDetail.PickedQty);
                Assert.AreEqual(3, outboundDetail.PickedPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(150, storageDetail.AllocatedQty);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail.Status);
                var storageDetail2 = context.StorageDetails.Find("TESAP202009000EC7");
                Assert.AreEqual(100, storageDetail2.AllocatedQty);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail2.Status);
                var storageDetail3 = context.StorageDetails.Find("TESAP202009000EC8");
                Assert.AreEqual(50, storageDetail3.AllocatedQty);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail3.Status);
            }
        }


        [TestMethod]
        public async Task AutoAllocate_CPart()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 150);
                var pm = context.PartMasters.Find(factoryID, supplierID, productCode);
                pm.CPartSPQ = 50;
                pm.IsCPart = 1;

                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;


                // add inventory
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandQty = 1000,
                    OnHandPkg = 100,
                    AllocatedQty = 500,
                    AllocatedPkg = 50,
                    QuarantineQty= 100,
                    QuarantinePkg = 10,
                    TransitQty = 0,
                    TransitPkg = 0,
                    DiscrepancyQty= 0
                });

                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandQty = 1000,
                    OnHandPkg = 100,
                    AllocatedQty = 500,
                    AllocatedPkg = 50,
                    QuarantineQty = 100,
                    QuarantinePkg = 10,
                    TransitQty = 0,
                    TransitPkg = 0,
                    DiscrepancyQty = 0
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var pickingListService = GetPickingListService(context);
                var result = await pickingListService.AutoAllocate(new AllocationDto
                {
                    JobNo = jobno,
                    LineItem = 1
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(3, context.PickingLists.Count());
                var pl1 = context.PickingLists.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SeqNo == 1).FirstOrDefault();
                Assert.AreEqual(50, pl1.Qty);
                Assert.AreEqual("", pl1.PID);
                Assert.AreEqual("L4", pl1.LocationCode);
                Assert.AreEqual(whscode, pl1.WHSCode);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-30), pl1.InboundDate);
                var pl2 = context.PickingLists.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SeqNo == 2).FirstOrDefault();
                Assert.AreEqual(50, pl2.Qty);
                Assert.AreEqual("", pl2.PID);
                var pl3 = context.PickingLists.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SeqNo == 3).FirstOrDefault();
                Assert.AreEqual(50, pl3.Qty);
                Assert.AreEqual("", pl3.PID);

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual(150, outboundDetail.Qty);
                Assert.AreEqual(150, outboundDetail.PickedQty);
                Assert.AreEqual(3, outboundDetail.PickedPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(150, storageDetail.AllocatedQty);
                Assert.AreEqual(Ownership.EHP, storageDetail.Ownership);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail.Status);

                Assert.AreEqual(3, context.PickingAllocatedPIDs.Count());
                var pla1 = context.PickingAllocatedPIDs.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SerialNo == 1).FirstOrDefault();
                Assert.AreEqual(50, pla1.AllocatedQty);
                Assert.AreEqual(pid, pla1.PID);
                Assert.AreEqual(jobno, pla1.JobNo);

                Assert.AreEqual(DateTime.Now.Date.AddDays(-30), pl1.InboundDate);
                var pla2 = context.PickingAllocatedPIDs.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SerialNo == 2).FirstOrDefault();
                Assert.AreEqual(50, pla2.AllocatedQty);
                Assert.AreEqual(pid, pla2.PID);
                Assert.AreEqual(jobno, pla2.JobNo);
                var pla3 = context.PickingAllocatedPIDs.Where(l => l.JobNo == jobno && l.LineItem == 1 && l.SerialNo == 3).FirstOrDefault();
                Assert.AreEqual(50, pla3.AllocatedQty);
                Assert.AreEqual(pid, pla3.PID);
                Assert.AreEqual(jobno, pla3.JobNo);

                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                Assert.AreEqual(150, inventoryEHP.AllocatedQty);
                Assert.AreEqual(0, inventorySupplier.AllocatedQty);
            }
        }


        //[TestMethod]
        public void AutoAllocate_ConcurrentTransactions()
        {
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();

            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 500);
            }
            var context1 = new Context(options, appSettings.Object);
            var repository1 = new SqlTTLogixRepository(context1, new Mock<IRawSqlExecutor>().Object);
            var utilityService1 = new UtilityService(repository1, appSettings.Object);


            var eKanbanService1 = new EKanbanService(repository1, utilityService1, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var reportService = new Mock<IReportService>().Object;
            var labelProvider = new Mock<ILabelProvider>().Object;
            var billingService = new BillingService(repository1);
            var storageService1 = new StorageService(repository1, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var outboundService1 = new OutboundService(repository1, appSettings.Object, mapper, utilityService1, eKanbanService1,
                reportService, logger.Object, storageService1, locker, _iLogConnect.Object, new Logger<OutboundService>(loggerFactory), billingService, null);

            var pickingListService1 = new PickingListService(repository1, outboundService1, utilityService1, _iLogConnect.Object, appSettings.Object, mapper, locker, new Logger<PickingListService>(loggerFactory));
            var t1 = pickingListService1.AutoAllocate(new AllocationDto
            {
                JobNo = jobno,
                LineItem = 1
            });

            var context2 = new Context(options, appSettings.Object);
            var repository2 = new SqlTTLogixRepository(context2, new Mock<IRawSqlExecutor>().Object);
            var utilityService2 = new UtilityService(repository2, appSettings.Object);
            var eKanbanService2 = new EKanbanService(repository2, utilityService2, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var storageService = new StorageService(repository2, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var outboundService2 = new OutboundService(repository2, appSettings.Object, mapper, utilityService2, eKanbanService2,
                reportService, logger.Object, storageService, locker, _iLogConnect.Object, new Logger<OutboundService>(loggerFactory), billingService);
            var pickingListService2 = new PickingListService(repository2, outboundService2, utilityService2, _iLogConnect.Object, appSettings.Object, mapper, locker, new Logger<PickingListService>(loggerFactory));

            var t2 = pickingListService2.AutoAllocate(new AllocationDto
            {
                JobNo = jobno,
                LineItem = 1
            });

            Task.WaitAll(t1, t2);

            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(3, context.PickingLists.Count());
            }
        }

        //[TestMethod]
        public void Allocate_ConcurrentTransactions()
        {
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();

            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 500);
            }
            var context1 = new Context(options, appSettings.Object);
            var repository1 = new SqlTTLogixRepository(context1, new Mock<IRawSqlExecutor>().Object);
            var utilityService1 = new UtilityService(repository1, appSettings.Object);

            var eKanbanService1 = new EKanbanService(repository1, utilityService1, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var reportService = new Mock<IReportService>().Object;
            var labelProvider = new Mock<ILabelProvider>().Object;
            var billingService = new BillingService(repository1);
            var storageService1 = new StorageService(repository1, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var outboundService1 = new OutboundService(repository1, appSettings.Object, mapper, utilityService1, eKanbanService1,
                reportService, logger.Object, storageService1, locker, _iLogConnect.Object, new Logger<OutboundService>(loggerFactory), billingService);

            var pickingListService1 = new PickingListService(repository1, outboundService1, utilityService1, _iLogConnect.Object, appSettings.Object, mapper, locker, new Logger<PickingListService>(loggerFactory));
            var t1 = pickingListService1.AutoAllocate(new AllocationDto
            {
                JobNo = jobno,
                LineItem = 1
            });

            var context2 = new Context(options, appSettings.Object);
            var repository2 = new SqlTTLogixRepository(context2, new Mock<IRawSqlExecutor>().Object);
            var utilityService2 = new UtilityService(repository2, appSettings.Object);
            var eKanbanService2 = new EKanbanService(repository2, utilityService2, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var storageService = new StorageService(repository2, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var outboundService2 = new OutboundService(repository2, appSettings.Object, mapper, utilityService2, eKanbanService2,
                reportService, logger.Object, storageService, locker, _iLogConnect.Object, new Logger<OutboundService>(loggerFactory), billingService);
            var pickingListService2 = new PickingListService(repository2, outboundService2, utilityService2, _iLogConnect.Object, appSettings.Object, mapper, locker, new Logger<PickingListService>(loggerFactory));

            var t2 = pickingListService2.AllocatePickingListBatch(new[] { new PickingListAllocateDto
            {
                JobNo = jobno,
                LineItem = 1,
                PID = pid
            }});

            Task.WaitAll(t1, t2);
     
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(3, context.PickingLists.Count());
            }
        }

        //[TestMethod]
        public async Task AllocatePickingListItem()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 150);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result =  await GetPickingListService(context).AllocatePickingListBatch(new [] { new PickingListAllocateDto
                {
                    JobNo = jobno,
                    LineItem = 1,
                    PID = pid
                } });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var storageDetail = context.StorageDetails.Find(pid);
                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                var outboundDetail2 = context.OutboundDetails.Find(jobno, 2);
                var outbound = context.Outbounds.Find(jobno);

                // picking list added
                Assert.AreEqual(1, context.PickingLists.Count());
                var pl = context.PickingLists.First();

                Assert.AreEqual(jobno, pl.JobNo);
                Assert.AreEqual(1, pl.LineItem);
                Assert.AreEqual(storageDetail.Qty, pl.Qty);
                Assert.AreEqual(pid, pl.PID);
                Assert.AreEqual(storageDetail.LocationCode, pl.LocationCode);
                Assert.AreEqual(storageDetail.InboundDate, pl.InboundDate);
                Assert.AreEqual("", pl.PickedBy);
                Assert.AreEqual(whscode, pl.WHSCode);
                Assert.IsNull(pl.PickedDate);

                Assert.AreEqual(150, outboundDetail.Qty);
                Assert.AreEqual(150, outboundDetail.PickedQty);
                Assert.AreEqual(1, outboundDetail.PickedPkg);
              
                Assert.AreEqual(500, outboundDetail2.Qty);
                Assert.AreEqual(0, outboundDetail2.PickedQty);
                Assert.AreEqual(0, outboundDetail2.PickedPkg);

                Assert.AreEqual(150, storageDetail.AllocatedQty);
                Assert.AreEqual(StorageStatus.Allocated, (StorageStatus)storageDetail.Status);
                var storageDetail2 = context.StorageDetails.Find("TESAP202009000EC7");
                Assert.AreEqual(0, storageDetail2.AllocatedQty);
                Assert.AreEqual(StorageStatus.Putaway, (StorageStatus)storageDetail2.Status);
                var storageDetail3 = context.StorageDetails.Find("TESAP202009000EC8");
                Assert.AreEqual(0, storageDetail3.AllocatedQty);
                Assert.AreEqual(StorageStatus.Putaway, (StorageStatus)storageDetail3.Status);

            }
        }

        [TestMethod]
        public async Task AutoAllocate_ManualEntry_InventoryAllocationValuesUpdated()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 310);

                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = "TESAP202009000EB0",
                    InJobNo = "INB20200900125",
                    LineItem = 1,
                    SeqNo = 1,
                    ParentID = "",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    InboundDate = DateTime.Now.Date.AddDays(-30),
                    OriginalQty = 10,
                    Qty = 10,
                    QtyPerPkg = 10,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    WHSCode = whscode,
                    LocationCode = "L4",
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.Supplier
                });

                context.Inventory.Add(new Core.Entities.Inventory()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandQty = 10,
                    OnHandPkg = 1,
                    AllocatedQty = 10,
                    AllocatedPkg = 1,
                    QuarantineQty = 0,
                    QuarantinePkg = 0,
                    DiscrepancyQty = 0,
                    TransitPkg = 0,
                    TransitQty = 0
                });
                context.Inventory.Add(new Core.Entities.Inventory()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandQty = 300,
                    OnHandPkg = 3,
                    AllocatedQty = 300,
                    AllocatedPkg = 3,
                    QuarantineQty = 0,
                    QuarantinePkg = 0,
                    DiscrepancyQty = 0,
                    TransitPkg = 0,
                    TransitQty = 0
                });
                context.SaveChanges();

            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetPickingListService(context).AutoAllocate(new AllocationDto { 
                    JobNo = jobno,
                    LineItem = 1
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var storageDetail = context.StorageDetails.Find(pid);
                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                var outboundDetail2 = context.OutboundDetails.Find(jobno, 2);
                var outbound = context.Outbounds.Find(jobno);

                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                Assert.AreEqual(inventorySupplier.AllocatedQty, 10);
                Assert.AreEqual(inventorySupplier.AllocatedPkg, 1);
                Assert.AreEqual(inventoryEHP.AllocatedQty, 300);
                Assert.AreEqual(inventoryEHP.AllocatedPkg, 3);
            }
        }

        //[TestMethod]
        public async Task UnAllocate()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, 20);
               await GetPickingListService(context).AutoAllocate(new AllocationDto
                {
                    JobNo = jobno,
                    LineItem = 1
                });
                var pl = context.PickingLists.Single();
                pl.PickedBy = "USER1";
                pl.PID = pid;
                pl.PickedDate = DateTime.Now.AddDays(-1);
                var sd = context.StorageDetails.Find(pid);
                sd.OutJobNo = jobno;
                context.SaveChanges();   
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var pickingListService = GetPickingListService(context);
                var unAllocationDto = new UndoAllocationDto() { JobNo = jobno, LineItem = 1, PID = null };
                var result = await pickingListService.UnAllocateBatch(new UndoAllocationDto[] { unAllocationDto });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // picking list deleted
                Assert.AreEqual(0, context.PickingLists.Count());

                // outbound picked qty & pkg deducted
                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual(150, outboundDetail.Qty);
                Assert.AreEqual(0, outboundDetail.PickedQty);
                Assert.AreEqual(0, outboundDetail.PickedPkg);

                // storage out job no unassigned
                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual("", storageDetail.OutJobNo);
                Assert.AreEqual(0, storageDetail.AllocatedQty);
                Assert.AreEqual(StorageStatus.Putaway, (StorageStatus)storageDetail.Status);
            }
        }
       
        private IPickingListService GetPickingListService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var utilityService = new UtilityService(repository, appSettings.Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var reportService = new Mock<IReportService>().Object;
            var labelProvider = new Mock<ILabelProvider>().Object;
            var billingService = new BillingService(repository);
            var eKanbanService2 = new EKanbanService(repository, utilityService, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var storageService = new StorageService(repository, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var outboundService = new OutboundService(repository, appSettings.Object, mapper, utilityService, eKanbanService2,
                reportService, logger.Object, storageService, locker, _iLogConnect.Object, new Logger<OutboundService>(loggerFactory), billingService);

            return new PickingListService(repository, outboundService, utilityService, _iLogConnect.Object, appSettings.Object, mapper, locker, new Logger<PickingListService>(loggerFactory));
        }

        private void AddTestData(Context context, decimal qty)
        {
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });
 
            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.ManualEntry,
                Remark = "Remark",
                Status = 0,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid,
                InJobNo = "INB20200900125",
                LineItem = 1,
                SeqNo = 1,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 150,
                Qty = 150,
                QtyPerPkg = 150,
                AllocatedQty = 0,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.EHP
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "TESAP202009000EC7",
                InJobNo = "INB20200900125",
                LineItem = 1,
                SeqNo = 2,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-20),
                OriginalQty = 100,
                Qty = 100,
                QtyPerPkg = 100,
                AllocatedQty = 0,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L5",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.EHP
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "TESAP202009000EC8",
                InJobNo = "INB20200900125",
                LineItem = 1,
                SeqNo = 3,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-10),
                OriginalQty = 50,
                Qty = 50,
                QtyPerPkg = 50,
                AllocatedQty = 0,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L6",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.EHP
            });

            context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            {
                JobNo = jobno,
                LineItem = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = qty,
                PickedQty = 0,
                Pkg = 0,
                PickedPkg = 0,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            }); 
            context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            {
                JobNo = jobno,
                LineItem = 2,
                ProductCode = "132732024",
                SupplierID = supplierID,
                Qty = 500,
                PickedQty = 0,
                Pkg = 1,
                PickedPkg = 0,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            });
            context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            {
                JobNo = jobno,
                LineItem = 3,
                ProductCode = "132732025",
                SupplierID = "504163",
                Qty = 700,
                PickedQty = 0,
                Pkg = 1,
                PickedPkg = 0,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            });

            context.SupplierMasters.Add(new Core.Entities.SupplierMaster
            {
                FactoryID = factoryID,
                SupplierID = supplierID,
                CompanyName = "Company1",
                Status = 1
            });
            context.SupplierMasters.Add(new Core.Entities.SupplierMaster
            {
                FactoryID = factoryID,
                SupplierID = "504163",
                CompanyName = "Company2",
                Status = 1
            });

            context.PartMasters.Add(new Core.Entities.PartMaster()
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                Description = "HOS PASS C346 VEVA ADP LH ACTUAL",
                UOM = "UOM0041",
                PackageType = "PKG0002",
                SPQ = 144,
                CPartSPQ = 1,
                IsCPart = 0
            });

            context.Inbounds.Add(new Core.Entities.Inbound() {
                JobNo = "INB20200900125",
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = "ELPS-HT2020072502-507188"
            });

            context.Locations.Add(new Core.Entities.Location()
            {
                Code = "L4",
                WHSCode = whscode,
                AreaCode = "Line",
                Name = "L4",
                Status = 1,
                Type = LocationType.Normal
            });
            context.Locations.Add(new Core.Entities.Location()
            {
                Code = "L5",
                WHSCode = whscode,
                AreaCode = "Line",
                Name = "L5",
                Status = 1,
                Type = LocationType.Normal
            });
            context.Locations.Add(new Core.Entities.Location()
            {
                Code = "L6",
                WHSCode = whscode,
                AreaCode = "Line",
                Name = "L6",
                Status = 1,
                Type = LocationType.Normal
            });

            context.ILogLocationCategories.Add(new Core.Entities.ILogLocationCategory()
            {
                Id = 10,
                Name = "OtherTTLogixManaged"
            });
            context.ILogLocationCategories.Add(new Core.Entities.ILogLocationCategory()
            {
                Id = 11,
                Name = "Inbound"
            });
            context.ILogLocationCategories.Add(new Core.Entities.ILogLocationCategory()
            {
                Id = 12,
                Name = "Outbound"
            });
            context.ILogLocationCategories.Add(new Core.Entities.ILogLocationCategory()
            {
                Id = 13,
                Name = "iLogTransfer"
            });
            context.ILogLocationCategories.Add(new Core.Entities.ILogLocationCategory()
            {
                Id = 14,
                Name = "iLogStorage"
            });

            context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
            {
                UOM = "UOM0041",
                CustomerCode = factoryID,
                Status = 1,
                DecimalNum = 3
            });
            context.SaveChanges();
        }

        private readonly string jobno = "OUT20200900626";
        private readonly string productCode = "PRODCODE1";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";
        private readonly string pid = "TESAP202009000EC6";
        private readonly string whscode = "PL";
        private readonly string refno = "9200909056";
        //private readonly string injobno = "INB20200900125";
        //private readonly string uom = "UOM0000";
    }
}
