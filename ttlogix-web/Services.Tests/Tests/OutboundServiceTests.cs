using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT.DB;
using TT.Services.Services;
using TT.Core.Enums;
using TT.Services.Models;
using TT.Services.Interfaces;
using System.Collections.Generic;
using Moq;
using TT.Core.Entities;
using Newtonsoft.Json.Linq;
using TT.Core.Interfaces;

namespace TT.Services.Tests
{
    [TestClass]
    public class OutboundServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            sqlExecutor = new Mock<IRawSqlExecutor>();
        }

        // TODO commented out as this test will fail on subcollection ToList() expression (suppliers subquery)
        //[TestMethod]
        public async Task GetOutboundList()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.GetOutboundList(new Core.QueryFilters.OutboundListQueryFilter()
                {
                    JobNo = jobno,
                    PageNo = 1,
                    PageSize = 20
                });
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(1, result.Total);
                Assert.AreEqual(1, result.PageNo);
                Assert.AreEqual(20, result.PageSize);
                var row = result.Data.First();
                Assert.AreEqual(jobno, row.JobNo);
                Assert.AreEqual("Company1, Company2", row.SupplierName);
            }
        }

        [TestMethod]
        public async Task CreateOutboundManual()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }
            string newjobno;
            var outboundDto = new OutboundManualDto()
            {
                CustomerCode = factoryID,
                ManualType = Models.ModelEnums.ManualType.ManualEHP,
                NewWHSCode = "newwhs",
                ETD = DateTime.Now,
                OSNo = "osno",
                RefNo = "refno",
                Remark = "remark",
                Status = (byte)OutboundStatus.NewJob,
                TransType = (byte)OutboundType.ManualEntry,
                WHSCode = whscode
            };
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CreateOutboundManual(outboundDto, "USERCODE1");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newjobno = result.Data;
                Assert.IsNotNull(newjobno);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(newjobno);
                var header = context.EKanbanHeaders.Where(h => h.OutJobNo == newjobno).FirstOrDefault();
                Assert.IsNotNull(outbound);
                Assert.IsNotNull(header);
                Assert.AreEqual(outboundDto.CustomerCode, outbound.CustomerCode);
                Assert.AreEqual(outboundDto.NewWHSCode, outbound.NewWHSCode);
                Assert.AreEqual(outboundDto.ETD, outbound.ETD);
                Assert.AreEqual(outboundDto.OSNo, outbound.OSNo);
                Assert.AreEqual(outboundDto.Remark, outbound.Remark);
                Assert.AreEqual(outboundDto.Status, outbound.Status);
                Assert.AreEqual(outboundDto.TransType, outbound.TransType);
                Assert.AreEqual(outboundDto.WHSCode, outbound.WHSCode);
                Assert.AreEqual(header.OrderNo, outbound.RefNo);
            }
        }

        [TestMethod]
        public async Task UpdateOutbound_NewJobEKanban()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
            }

            var outboundDto = new OutboundDto()
            {
                JobNo = jobno,
                RefNo = "refno",
                Remark = "remarkchanged",
                ETD = DateTime.Now.Date.AddDays(100),
                Status = OutboundStatus.OutStanding,
                CommInvNo= "comminvnochanged",
                NoOfPallet = 1000,
                DeliveryTo = "deliverytochanged",

                CustomerCode = "XXX",
                NewWHSCode = "XX",
                OSNo = "X",
                TransType = OutboundType.CrossDock,
                WHSCode = "XX",
                RevisedDate = DateTime.Now.Date,
                CancelledDate = DateTime.Now.Date,
                CancelledBy = "XX",
                Charged = 1,
                CreatedBy= "XX",
                CreatedDate = DateTime.Now.Date,
                DispatchedBy = "XX",
                DispatchedDate = DateTime.Now.Date,
                RevisedBy = "XX"
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutbound(jobno, outboundDto);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);

                // only these fields should change
                Assert.AreEqual(outbound.Remark, "remarkchanged");
                Assert.AreEqual(outbound.ETD, DateTime.Now.Date.AddDays(100));
                Assert.AreEqual(outbound.Status, OutboundStatus.NewJob);
                Assert.AreEqual(outbound.CommInvNo, "comminvnochanged");
                Assert.AreEqual(outbound.NoOfPallet, 1000);
                Assert.AreEqual(outbound.DeliveryTo, "deliverytochanged");

                // these fields should stay unchanged
                Assert.AreNotEqual(outbound.RefNo, "refno");
                Assert.AreNotEqual(outbound.CustomerCode, "XXX");
                Assert.AreNotEqual(outbound.NewWHSCode, "XX");
                Assert.AreNotEqual(outbound.OSNo, "X");
                Assert.AreNotEqual(outbound.TransType, OutboundType.CrossDock);
                Assert.AreNotEqual(outbound.WHSCode, "XX");
                Assert.AreNotEqual(outbound.RevisedDate, DateTime.Now.Date);
                Assert.AreNotEqual(outbound.CancelledDate, DateTime.Now.Date);
                Assert.AreNotEqual(outbound.CancelledBy, "XX");
                Assert.AreNotEqual(outbound.Charged, 1);
                Assert.AreNotEqual(outbound.CreatedBy, "XX");
                Assert.AreNotEqual(outbound.CreatedDate, DateTime.Now.Date);
                Assert.AreNotEqual(outbound.DispatchedBy, "XX");
                Assert.AreNotEqual(outbound.DispatchedDate, DateTime.Now.Date);
                Assert.AreNotEqual(outbound.RevisedBy, "XX");
            }
        }

        [TestMethod]
        public async Task UpdateOutbound_ReturnNewJob_RefNoChanged()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                var outbound = context.Outbounds.First();
                outbound.TransType = OutboundType.Return;
                context.SaveChanges();
            }

            var outboundDto = new OutboundDto()
            {
                JobNo = jobno,
                RefNo = "refno"
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutbound(jobno, outboundDto);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.RefNo, "refno");
            }
        }

        [TestMethod]
        public async Task UpdateOutbound_ReturnCancelled_RefNoNotChanged()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                var outbound = context.Outbounds.First();
                outbound.TransType = OutboundType.Return;
                outbound.Status = OutboundStatus.Cancelled;
                context.SaveChanges();
            }

            var outboundDto = new OutboundDto()
            {
                JobNo = jobno,
                RefNo = "refno"
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutbound(jobno, outboundDto);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreNotEqual(outbound.RefNo, "refno");
            }
        }

        [TestMethod]
        public async Task UpdateOutbound_ManualNewJob_RefNoNotChanged()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                var outbound = context.Outbounds.First();
                outbound.TransType = OutboundType.Return;
                outbound.Status = OutboundStatus.Cancelled;
                context.SaveChanges();
            }

            var outboundDto = new OutboundDto()
            {
                JobNo = jobno,
                RefNo = "refno"
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutbound(jobno, outboundDto);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreNotEqual(outbound.RefNo, "refno");
            }
        }

        [TestMethod]
        public async Task DeleteOutboundDetail_NotManual()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddDeleteOutboundDetailTestData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.Status = OutboundStatus.PartialPicked;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.DeleteOutboundDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);

                Assert.AreEqual(1300, inventoryEHP.AllocatedQty); // not changed
                Assert.AreEqual(800, inventorySupplier.AllocatedQty); // all amount deducted

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.IsNull(outboundDetail);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.PartialPicked, outbound.Status);
            }
        }

        [TestMethod]
        public async Task DeleteOutboundDetail_Manual()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddDeleteOutboundDetailTestData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;
                outbound.Status = OutboundStatus.PartialPicked;
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                inventorySupplier.AllocatedQty = 100;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.DeleteOutboundDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);

                Assert.AreEqual(900, inventoryEHP.AllocatedQty); //  changed, deducted the rest of outbound qty 1300 - (500-100)
                Assert.AreEqual(0, inventorySupplier.AllocatedQty); // all amount deducted i.e. 100

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.IsNull(outboundDetail);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.PartialPicked, outbound.Status);
            }
        }

        [TestMethod]
        public async Task DeleteOutboundDetailSingle_Manual()
        {
            using(var context = new Context(options, appSettings.Object))
            {
                AddTestDataSingle(context);
                AddDeleteOutboundDetailTestData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;
                outbound.Status = OutboundStatus.PartialPicked;
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                inventorySupplier.AllocatedQty = 100;
                context.SaveChanges();
            }

            using(var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.DeleteOutboundDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using(var context = new Context(options, appSettings.Object))
            {
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);

                Assert.AreEqual(900, inventoryEHP.AllocatedQty); //  changed, deducted the rest of outbound qty 1300 - (500-100)
                Assert.AreEqual(0, inventorySupplier.AllocatedQty); // all amount deducted i.e. 100

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.IsNull(outboundDetail);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
            }
        }

        [TestMethod]
        public async Task DeleteOutboundDetailNotpicked_Manual()
        {
            using(var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddDeleteOutboundDetailTestData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;
                outbound.Status = OutboundStatus.PartialPicked;
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                inventorySupplier.AllocatedQty = 100;
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 3,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 640,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2,
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 3,
                    SeqNo = 4,
                    ProductCode = "132732025",
                    SupplierID = supplierID,
                    Qty = 640,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid3,
                });
                context.SaveChanges();
            }

            using(var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.DeleteOutboundDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using(var context = new Context(options, appSettings.Object))
            {
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);

                Assert.AreEqual(900, inventoryEHP.AllocatedQty); //  changed, deducted the rest of outbound qty 1300 - (500-100)
                Assert.AreEqual(0, inventorySupplier.AllocatedQty); // all amount deducted i.e. 100

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.IsNull(outboundDetail);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.Picked, outbound.Status);
            }
        }

        [TestMethod]
        public async Task UpdateOutboundStatus_TESANotPicked()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);

                // not picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    //PickedBy = "USER1",
                    //PickedDate = DateTime.Now.AddDays(-5),
                    //PID = pid,
                });

                // picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 2,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    //PickedBy = "USER1",
                    //PickedDate = DateTime.Now.AddDays(-5),
                    //PID = pid,
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutboundStatus(jobno);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.Status, OutboundStatus.NewJob);

                //TODO: 
                //Assert.AreEqual(outbound.RefNo, "refno");
                //Assert.AreEqual(outbound.Remark, "remarkchanged");
                //Assert.AreEqual(outbound.ETD, DateTime.Now.Date.AddDays(100));
                //Assert.AreEqual(outbound.CommInvNo, "comminvnochanged");
                //Assert.AreEqual(outbound.NoOfPallet, 1000);
                //Assert.AreEqual(outbound.DeliveryTo, "deliverytochanged");

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetail.Status);// not changed
            }
        }

        [TestMethod]
        public async Task UpdateOutboundStatus_TESAPartiallyPicked()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);

                // not picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    //PickedBy = "USER1",
                    //PickedDate = DateTime.Now.AddDays(-5),
                    //PID = pid,
                });

                // picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 2,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid,
                });

                // picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 1,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 640,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid,
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutboundStatus(jobno);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.Status, OutboundStatus.PartialPicked);

                //TODO: 
                //Assert.AreEqual(outbound.RefNo, "refno");
                //Assert.AreEqual(outbound.Remark, "remarkchanged");
                //Assert.AreEqual(outbound.ETD, DateTime.Now.Date.AddDays(100));
                //Assert.AreEqual(outbound.CommInvNo, "comminvnochanged");
                //Assert.AreEqual(outbound.NoOfPallet, 1000);
                //Assert.AreEqual(outbound.DeliveryTo, "deliverytochanged");

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetail.Status);
            }
        }

        [TestMethod]
        public async Task UpdateOutboundStatus_TESAFullyPicked()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);

                // not picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid,
                });

                // picked
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 2,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 250,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid,
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UpdateOutboundStatus(jobno);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.Status, OutboundStatus.Picked);

                //TODO: 
                //Assert.AreEqual(outbound.RefNo, "refno");
                //Assert.AreEqual(outbound.Remark, "remarkchanged");
                //Assert.AreEqual(outbound.ETD, DateTime.Now.Date.AddDays(100));
                //Assert.AreEqual(outbound.CommInvNo, "comminvnochanged");
                //Assert.AreEqual(outbound.NoOfPallet, 1000);
                //Assert.AreEqual(outbound.DeliveryTo, "deliverytochanged");

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);
            }
        }

        [TestMethod]
        public async Task CancelOutbound_EKanban()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddCancelOutboundData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CancelOutbound(jobno, "USERNAME2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.Status, OutboundStatus.Cancelled);
                Assert.IsNotNull(outbound.CancelledDate);
                Assert.AreEqual("USERNAME2", outbound.CancelledBy);

                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                Assert.IsNotNull(inventory);
                Assert.AreEqual(800, inventory.AllocatedQty);

                var header = context.EKanbanHeaders.Find(refno);
                //Assert.AreEqual((int)EKanbanStatus.New, header.Status);
                //Assert.AreEqual("", header.OutJobNo);

                Assert.IsFalse(context.EKanbanDetails.Where(d => d.OrderNo == refno && d.Quantity == 0).Any());
                Assert.IsFalse(context.EOrders.Where(d => d.PurchaseOrderNo == refno && d.OrderQuantity == "0").Any());

                Assert.IsFalse( context.LoadingDetails.Where(l => l.OrderNo == refno).Any());
                var loading = context.Loadings.Where(l => l.JobNo == jobno).FirstOrDefault();
                Assert.AreEqual(LoadingStatus.NewJob, loading.Status);
                Assert.AreEqual("USERNAME2", loading.RevisedBy);
                Assert.IsNotNull(loading.RevisedDate);
            }
        }

        [TestMethod]
        public async Task CancelOutbound_NotEKanban()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddCancelOutboundData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.WHSTransfer;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CancelOutbound(jobno, "USERNAME2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.IsNotNull(outbound);
                Assert.AreEqual(outbound.Status, OutboundStatus.Cancelled);
                Assert.IsNotNull(outbound.CancelledDate);
                Assert.AreEqual("USERNAME2", outbound.CancelledBy);

                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                Assert.IsNotNull(inventory);
                Assert.AreEqual(800, inventory.AllocatedQty);

                var header = context.EKanbanHeaders.Find(refno);
                Assert.AreEqual((int)EKanbanStatus.Cancelled, header.Status);
                Assert.AreEqual("", header.OutJobNo);

                Assert.IsTrue(context.EKanbanDetails.Where(d => d.OrderNo == refno && d.Quantity == 0).Any());
                Assert.IsTrue(context.EOrders.Where(d => d.PurchaseOrderNo == refno && d.OrderQuantity == "0").Any());

                Assert.IsFalse(context.LoadingDetails.Where(l => l.OrderNo == refno).Any());
                var loading = context.Loadings.Where(l => l.JobNo == jobno).FirstOrDefault();
                Assert.AreEqual(LoadingStatus.NewJob, loading.Status);
                Assert.AreEqual("USERNAME2", loading.RevisedBy);
                Assert.IsNotNull(loading.RevisedDate);
            }
        }

        [TestMethod]
        public async Task CancelAllocation()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                AddCancelAllocationData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.Status = OutboundStatus.PartialPicked;
                context.SaveChanges();
            }
            var data = new CancelAllocationDto()
            {
                JobNo = jobno,
                LineItem = 1,
                ItemsToCancel = new PickingListItemId[] { 
                    new PickingListItemId { JobNo = jobno, LineItem = 1, SeqNo = 1 },
                    new PickingListItemId { JobNo = jobno, LineItem = 1, SeqNo = 2 } 
                }
            };
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CancelAllocation(data);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType, message: result.Errors.FirstOrDefault());
            }
            using (var context = new Context(options, appSettings.Object))
            {
                // 2 picking lists were removed
                Assert.AreEqual(2, context.PickingLists.Count());

                var header = context.EKanbanHeaders.Find(refno);
                //Assert.AreEqual((int)EKanbanStatus.Cancelled, header.Status);
                //Assert.AreEqual("", header.Supp);
                var detail1 = context.EKanbanDetails.Find(refno, productCode, "SS" + productCode);
                var detail2 = context.EKanbanDetails.Find(refno, productCode, "SY" + productCode);
                Assert.AreEqual("", detail1.SupplierID);
                Assert.AreEqual(100, detail1.Quantity);
                Assert.AreEqual(0, detail1.QuantitySupplied);
                Assert.AreEqual("", detail2.SupplierID);
                Assert.AreEqual(150, detail2.Quantity);
                Assert.AreEqual(0, detail2.QuantitySupplied);
                
                var storage1 = context.StorageDetails.Find(pid);
                var storage2 = context.StorageDetails.Find("2");
                Assert.AreEqual(0, storage1.AllocatedQty);
                Assert.AreEqual(0, storage2.AllocatedQty);
                Assert.AreEqual("", storage1.OutJobNo);
                Assert.AreEqual("", storage2.OutJobNo);
                Assert.AreEqual(StorageStatus.Putaway, storage1.Status);
                Assert.AreEqual(StorageStatus.Putaway, storage2.Status);

                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);

                Assert.AreEqual(900, inventoryEHP.AllocatedQty);
                Assert.AreEqual(850, inventorySupplier.AllocatedQty);

                var outboundDetail = context.OutboundDetails.Find(jobno, 1);

                Assert.AreEqual(250, outboundDetail.PickedQty);
                Assert.AreEqual(2, outboundDetail.PickedPkg);
                Assert.AreEqual(250, outboundDetail.Qty);
                Assert.AreEqual(2, outboundDetail.Pkg);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.PartialPicked, outbound.Status);
            }

        }

        [TestMethod]
        public async Task CompleteOutbound_MixedOwnership()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddAutoAllocateTestData(context, 100, 100);

                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 2,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 200,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2
                });
                var sd = context.StorageDetails.Find(pid2);
                sd.Ownership = Ownership.Supplier;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteOutboundEurope(new string[] { jobno }, "TESTUSER2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task CompleteOutbound()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddAutoAllocateTestData(context, 100, 100);
            }
            sqlExecutor.Setup(i => i.ExecuteSqlRawAsync(It.IsRegex("^UPDATE EKanbanHeader SET Status"), It.IsAny<object[]>())).ReturnsAsync(() => {
                using (var context = new Context(options, appSettings.Object))
                {
                    foreach (var header in context.EKanbanHeaders.ToList())
                    {
                        header.Status = (byte)EKanbanStatus.InTransit;
                        header.ConfirmationDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
                return 1;
            });
            sqlExecutor.Setup(i => i.ExecuteSqlRawAsync(It.IsRegex("^UPDATE EKanbanDetail SET QuantityReceived = QuantitySupplied"), It.IsAny<object[]>())).ReturnsAsync(() => {
                using (var context = new Context(options, appSettings.Object))
                {
                    foreach (var detail in context.EKanbanDetails.ToList())
                    {
                        detail.QuantityReceived = detail.QuantitySupplied;
                    }
                    context.SaveChanges();
                }
                return 1;
            });


            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteOutboundEurope(new string[] { jobno }, "TESTUSER2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                Assert.AreEqual(900, inventory.OnHandQty);
                Assert.AreEqual(900, inventory.AllocatedQty);
                Assert.AreEqual(0, inventory.TransitQty);
                Assert.AreEqual(4, inventory.OnHandPkg);
                Assert.AreEqual(4, inventory.AllocatedPkg);
                Assert.AreEqual(0, inventory.TransitPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(StorageStatus.Dispatched, storageDetail.Status);
                Assert.AreEqual(0, storageDetail.Qty);
                Assert.AreEqual(0, storageDetail.AllocatedQty);

                var invTransactions = context.InvTransactions.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactions.Count());
                var addedTransaction = invTransactions.First();
                Assert.AreEqual(999, addedTransaction.BalancePkg);
                Assert.AreEqual(4900, addedTransaction.BalanceQty);
                Assert.AreEqual(100, addedTransaction.Qty);
                Assert.AreEqual(factoryID, addedTransaction.CustomerCode);
                Assert.AreEqual(jobno, addedTransaction.JobNo);
                Assert.AreEqual(productCode, addedTransaction.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransaction.Act);

                var invTransactionsWHS = context.InvTransactionsPerWHS.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsWHS.Count());
                var addedTransactionWHS = invTransactionsWHS.First();
                Assert.AreEqual(999, addedTransactionWHS.BalancePkg);
                Assert.AreEqual(4900, addedTransactionWHS.BalanceQty);
                Assert.AreEqual(100, addedTransactionWHS.Qty);
                Assert.AreEqual(1, addedTransactionWHS.Pkg);
                Assert.AreEqual(whscode, addedTransactionWHS.WHSCode);
                Assert.AreEqual(factoryID, addedTransactionWHS.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionWHS.JobNo);
                Assert.AreEqual(productCode, addedTransactionWHS.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionWHS.Act);

                var invTransactionsSupplier = context.InvTransactionsPerSupplier.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsSupplier.Count());
                var addedTransactionSupplier = invTransactionsSupplier.First();
                Assert.AreEqual(Ownership.EHP, addedTransactionSupplier.Ownership);
                Assert.AreEqual(4900, addedTransactionSupplier.BalanceQty);
                Assert.AreEqual(100, addedTransactionSupplier.Qty);
                Assert.AreEqual(supplierID, addedTransactionSupplier.SupplierID);
                Assert.AreEqual(factoryID, addedTransactionSupplier.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionSupplier.JobNo);
                Assert.AreEqual(productCode, addedTransactionSupplier.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionSupplier.Act);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.Completed, outbound.Status);
                Assert.AreEqual("TESTUSER2", outbound.DispatchedBy);
                Assert.IsNotNull(outbound.DispatchedDate.Value);

                var billingLogs = context.BillingLogs.ToList();
                Assert.AreEqual(1, billingLogs.Count());
                Assert.AreEqual(0, billingLogs.First().Quantity);
                Assert.AreEqual(200, billingLogs.First().CostPrice);
                Assert.AreEqual("EUR2", billingLogs.First().CostCurrency);

                var header = context.EKanbanHeaders.Find(outbound.RefNo);
                Assert.AreEqual((int)EKanbanStatus.InTransit, header.Status);
                Assert.IsNotNull(header.ConfirmationDate);

                var ekanbanDetailsAll = context.EKanbanDetails.ToList();
                var ekanbanDetails = context.EKanbanDetails.Where(e => e.OrderNo == header.OrderNo && e.ProductCode == productCode).ToList();
                Assert.AreEqual(1, ekanbanDetails.Count());
                Assert.AreEqual(100, ekanbanDetails.First().QuantitySupplied);
                Assert.AreEqual(100, ekanbanDetails.First().QuantityReceived);
            }
        }

        [TestMethod]
        public async Task CompleteOutboundManual()
        {
            sqlExecutor.Setup(i => i.ExecuteSqlRawAsync(It.IsRegex("^UPDATE EKanbanHeader SET Status"), It.IsAny<object[]>())).ReturnsAsync(() => {
                using (var context = new Context(options, appSettings.Object))
                {
                    foreach (var header in context.EKanbanHeaders.ToList())
                    {
                        header.Status = (byte)EKanbanStatus.InTransit;
                        header.ConfirmationDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
                return 1;
            });
            sqlExecutor.Setup(i => i.ExecuteSqlRawAsync(It.IsRegex("^UPDATE EKanbanDetail SET QuantityReceived = QuantitySupplied"), It.IsAny<object[]>())).ReturnsAsync(() => {
                using (var context = new Context(options, appSettings.Object))
                {
                    foreach (var detail in context.EKanbanDetails.ToList())
                    {
                        detail.QuantityReceived = detail.QuantitySupplied;
                    }
                    context.SaveChanges();
                }
                return 1;
            });
            using (var context = new Context(options, appSettings.Object))
            {
                AddAutoAllocateTestData(context, 100, 100);
                foreach (var outbound in context.Outbounds.ToList())
                {
                    outbound.TransType = (byte)OutboundType.ManualEntry;
                }

                foreach (var ed in context.EKanbanDetails)
                {
                    context.EKanbanDetails.Remove(ed);
                }

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteOutboundManual(new string[] { jobno }, "TESTUSER2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            
            using (var context = new Context(options, appSettings.Object))
            {
                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);

                Assert.AreEqual(900, inventory.OnHandQty);
                Assert.AreEqual(900, inventory.AllocatedQty);
                Assert.AreEqual(4, inventory.OnHandPkg);
                Assert.AreEqual(4, inventory.AllocatedPkg);
                Assert.AreEqual(0, inventory.TransitQty);
                Assert.AreEqual(0, inventory.TransitPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(StorageStatus.Dispatched, storageDetail.Status);
                Assert.AreEqual(0, storageDetail.Qty);
                Assert.AreEqual(0, storageDetail.AllocatedQty);

                var invTransactions = context.InvTransactions.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactions.Count());
                var addedTransaction = invTransactions.First();
                Assert.AreEqual(999, addedTransaction.BalancePkg);
                Assert.AreEqual(4900, addedTransaction.BalanceQty);
                Assert.AreEqual(100, addedTransaction.Qty);
                Assert.AreEqual(factoryID, addedTransaction.CustomerCode);
                Assert.AreEqual(jobno, addedTransaction.JobNo);
                Assert.AreEqual(productCode, addedTransaction.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransaction.Act);

                var invTransactionsWHS = context.InvTransactionsPerWHS.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsWHS.Count());
                var addedTransactionWHS = invTransactionsWHS.First();
                Assert.AreEqual(999, addedTransactionWHS.BalancePkg);
                Assert.AreEqual(4900, addedTransactionWHS.BalanceQty);
                Assert.AreEqual(100, addedTransactionWHS.Qty);
                Assert.AreEqual(1, addedTransactionWHS.Pkg);
                Assert.AreEqual(whscode, addedTransactionWHS.WHSCode);
                Assert.AreEqual(factoryID, addedTransactionWHS.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionWHS.JobNo);
                Assert.AreEqual(productCode, addedTransactionWHS.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionWHS.Act);

                var invTransactionsSupplier = context.InvTransactionsPerSupplier.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsSupplier.Count());
                var addedTransactionSupplier = invTransactionsSupplier.First();
                Assert.AreEqual(Ownership.EHP, addedTransactionSupplier.Ownership);
                Assert.AreEqual(4900, addedTransactionSupplier.BalanceQty);
                Assert.AreEqual(100, addedTransactionSupplier.Qty);
                Assert.AreEqual(supplierID, addedTransactionSupplier.SupplierID);
                Assert.AreEqual(factoryID, addedTransactionSupplier.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionSupplier.JobNo);
                Assert.AreEqual(productCode, addedTransactionSupplier.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionSupplier.Act);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.Completed, outbound.Status);
                Assert.AreEqual("TESTUSER2", outbound.DispatchedBy);
                Assert.IsNotNull(outbound.DispatchedDate.Value);

                var billingLogs = context.BillingLogs.ToList();
                Assert.AreEqual(1, billingLogs.Count());
                Assert.AreEqual(0, billingLogs.First().Quantity);
                Assert.AreEqual(200, billingLogs.First().CostPrice);
                Assert.AreEqual("EUR2", billingLogs.First().CostCurrency);

                var header = context.EKanbanHeaders.Find(outbound.RefNo);
                Assert.AreEqual((int)EKanbanStatus.InTransit, header.Status);
                Assert.IsNotNull(header.ConfirmationDate);

                // for manual outbound, new EKanbanDetail and new PickingListEKanban lines are added
                var ekanbanDetails = context.EKanbanDetails.ToList();
                Assert.AreEqual(1, ekanbanDetails.Count);
                Assert.AreEqual(100, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(100, ekanbanDetails[0].QuantityReceived);

                var pickingListEKanban = context.PickingListEKanbans.ToList();
                Assert.AreEqual(1, pickingListEKanban.Count);
                Assert.AreEqual(jobno, pickingListEKanban[0].JobNo);
                Assert.AreEqual(1, pickingListEKanban[0].LineItem);
                Assert.AreEqual(1, pickingListEKanban[0].SeqNo);
                Assert.AreEqual(refno, pickingListEKanban[0].OrderNo);
                Assert.AreEqual("1", pickingListEKanban[0].SerialNo);
                Assert.AreEqual(productCode, pickingListEKanban[0].ProductCode);

            }
        }
       
        [TestMethod]
        public async Task CompleteOutboundReturn()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddAutoAllocateTestData(context, 100, 100);
                foreach (var outbound in context.Outbounds.ToList())
                {
                    outbound.TransType = OutboundType.Return;
                }

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteOutboundReturn(new string[] { jobno }, "TESTUSER2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
         
            using (var context = new Context(options, appSettings.Object))
            {
                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);

                Assert.AreEqual(900, inventory.OnHandQty);
                Assert.AreEqual(900, inventory.AllocatedQty);
                Assert.AreEqual(0, inventory.TransitQty);
                Assert.AreEqual(4, inventory.OnHandPkg);
                Assert.AreEqual(4, inventory.AllocatedPkg);
                Assert.AreEqual(0, inventory.TransitPkg);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(StorageStatus.Dispatched, storageDetail.Status);
                Assert.AreEqual(0, storageDetail.Qty);
                Assert.AreEqual(0, storageDetail.AllocatedQty);

                var invTransactions = context.InvTransactions.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactions.Count());
                var addedTransaction = invTransactions.First();
                Assert.AreEqual(999, addedTransaction.BalancePkg);
                Assert.AreEqual(4900, addedTransaction.BalanceQty);
                Assert.AreEqual(100, addedTransaction.Qty);
                Assert.AreEqual(factoryID, addedTransaction.CustomerCode);
                Assert.AreEqual(jobno, addedTransaction.JobNo);
                Assert.AreEqual(productCode, addedTransaction.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransaction.Act);

                var invTransactionsWHS = context.InvTransactionsPerWHS.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsWHS.Count());
                var addedTransactionWHS = invTransactionsWHS.First();
                Assert.AreEqual(999, addedTransactionWHS.BalancePkg);
                Assert.AreEqual(4900, addedTransactionWHS.BalanceQty);
                Assert.AreEqual(100, addedTransactionWHS.Qty);
                Assert.AreEqual(1, addedTransactionWHS.Pkg);
                Assert.AreEqual(whscode, addedTransactionWHS.WHSCode);
                Assert.AreEqual(factoryID, addedTransactionWHS.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionWHS.JobNo);
                Assert.AreEqual(productCode, addedTransactionWHS.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionWHS.Act);

                var invTransactionsSupplier = context.InvTransactionsPerSupplier.ToList().OrderByDescending(t => t.SystemDateTime);
                Assert.AreEqual(2, invTransactionsSupplier.Count());
                var addedTransactionSupplier = invTransactionsSupplier.First();
                Assert.AreEqual(Ownership.EHP, addedTransactionSupplier.Ownership);
                Assert.AreEqual(4900, addedTransactionSupplier.BalanceQty);
                Assert.AreEqual(100, addedTransactionSupplier.Qty);
                Assert.AreEqual(supplierID, addedTransactionSupplier.SupplierID);
                Assert.AreEqual(factoryID, addedTransactionSupplier.CustomerCode);
                Assert.AreEqual(jobno, addedTransactionSupplier.JobNo);
                Assert.AreEqual(productCode, addedTransactionSupplier.ProductCode);
                Assert.AreEqual((int)InventoryTransactionType.Outbound, addedTransactionSupplier.Act);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.Completed, outbound.Status);
                Assert.AreEqual("TESTUSER2", outbound.DispatchedBy);
                Assert.IsNotNull(outbound.DispatchedDate.Value);

                var billingLogs = context.BillingLogs.ToList();
                Assert.AreEqual(0, billingLogs.Count());

                var header = context.EKanbanHeaders.Find(outbound.RefNo);
                Assert.IsNull(header.ConfirmationDate);
                Assert.AreEqual((byte)EKanbanStatus.New, header.Status);

                var ekanbanDetails = context.EKanbanDetails.Where(e => e.OrderNo == header.OrderNo && e.ProductCode == productCode).ToList();
                Assert.AreEqual(1, ekanbanDetails.Count());
                Assert.AreEqual(100, ekanbanDetails.First().QuantitySupplied);
                Assert.AreEqual(0, ekanbanDetails.First().QuantityReceived);

                var pickingListEKanban = context.PickingListEKanbans.ToList();
                Assert.AreEqual(0, pickingListEKanban.Count);
            }
        }

        [TestMethod]
        public async Task CargoInTransit()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddCargoInTransitTestData(context, 100, 100);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CargoInTransit(new string[] { jobno }, "TESTUSER2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var inventory = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                Assert.AreEqual(1900, inventory.OnHandQty);
                Assert.AreEqual(1900, inventory.AllocatedQty);
                Assert.AreEqual(100, inventory.TransitQty);
                Assert.AreEqual(9, inventory.OnHandPkg);
                Assert.AreEqual(9, inventory.AllocatedPkg);
                Assert.AreEqual(1, inventory.TransitPkg);

                var storageDetail = context.StorageDetails.Where(sd => sd.OutJobNo == jobno).First();
                Assert.AreEqual(StorageStatus.InTransit, storageDetail.Status);
                Assert.AreEqual(0, storageDetail.Qty);
                Assert.AreEqual(0, storageDetail.AllocatedQty);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.InTransit, outbound.Status);
                Assert.AreEqual("TESTUSER2", outbound.DispatchedBy);
                Assert.IsNotNull(outbound.DispatchedDate.Value);

                var header = context.EKanbanHeaders.Find(outbound.RefNo);
                Assert.AreEqual((int)EKanbanStatus.InTransit, header.Status);
            }
        }

        [TestMethod]
        public async Task AddNewOutboundDetail()
        {
            var newProductCode = "PRODCODE2";
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = newProductCode,
                    WHSCode = whscode,
                    OnHandQty = 2000,
                    OnHandPkg = 10,
                    AllocatedQty = 1000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = newProductCode,
                    WHSCode = whscode,
                    OnHandQty = 2000,
                    OnHandPkg = 10,
                    AllocatedQty = 1000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = (int)OutboundType.ManualEntry;
                context.SaveChanges();
            }
            var newLine = 0;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.AddNewOutboundDetail(new OutboundDetailAddDto
                {
                    JobNo = jobno,
                    Qty = 99,
                    SupplierID = supplierID,
                    ProductCode = newProductCode
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newLine = result.Data;
                Assert.AreEqual(4, result.Data);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundDetail = context.OutboundDetails.Find(jobno, newLine);
                Assert.AreEqual(99, outboundDetail.Qty);

                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, newProductCode, whscode, Ownership.EHP);
                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, newProductCode, whscode, Ownership.Supplier);

                Assert.AreEqual(1099, inventoryEHP.AllocatedQty);  // added Qty of 99 to 1000
                Assert.AreEqual(1000, inventorySupplier.AllocatedQty); // not changed
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_NonCPart_NonEHP_TheSameSupplier()
        {
            var productCode2 = productCode + "2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    //Instructions = "EHP"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                //context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                //{
                //    OrderNo = refno,
                //    ProductCode = productCode2,
                //    SerialNo = "1",
                //    SupplierID = supplierID,
                //    DropPoint = "01",
                //    Quantity = 500
                //});
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                string locationCode = "LOC1";
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code =locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail() 
                { 
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway
                });

                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.SaveChanges();
            }
         
            string newJobNo = null;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode,"USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newJobNo = result.Data;
            }
     
            using (var context = new Context(options, appSettings.Object))
            {
                // 1 outbound row created
                var outbound = context.Outbounds.Single();
                Assert.AreEqual(newJobNo, outbound.JobNo);
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(refno, outbound.RefNo);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE2", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual("Company1", outbound.Remark);

                // 1 outbound detail line created
                var outboundDetail = context.OutboundDetails.Single();
                Assert.AreEqual(newJobNo, outboundDetail.JobNo);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(200, outboundDetail.Qty);
                Assert.AreEqual(200, outboundDetail.PickedQty);
                Assert.AreEqual(2, outboundDetail.Pkg);
                Assert.AreEqual(2, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);

                //2 picking lists created
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(2, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(100, pickingList1.Qty);
                var pickingList2 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(newJobNo, pickingList2.JobNo);
                Assert.AreEqual(100, pickingList2.Qty);

                //2 PickingListEKanban created
                var pickingListsEKanbans = context.PickingListEKanbans.ToList();
                Assert.AreEqual(2, pickingListsEKanbans.Count());
                var pickingListsEKanban1 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban1.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban1.OrderNo);
                Assert.AreEqual("1", pickingListsEKanban1.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban1.ProductCode);
                var pickingListsEKanban2 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban2.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban2.OrderNo);
                Assert.AreEqual("2", pickingListsEKanban2.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban2.ProductCode);

                // storage status changed
                var storage = context.StorageDetails.Where(s => s.ProductCode == productCode).ToArray();
                Assert.AreEqual(StorageStatus.Allocated, storage[0].Status);
                Assert.AreEqual(StorageStatus.Allocated, storage[1].Status);
                Assert.AreEqual("", storage[0].OutJobNo);
                Assert.AreEqual("", storage[1].OutJobNo);
                Assert.AreEqual(100, storage[0].AllocatedQty);
                Assert.AreEqual(100, storage[1].AllocatedQty);

                // eKanbanDetail changes
                var ekanbanDetails = context.EKanbanDetails.ToArray();
                Assert.AreEqual(100, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(100, ekanbanDetails[1].QuantitySupplied);

                // eKanbanHeader changed
                var ekanbanHeader = context.EKanbanHeaders.First();
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanbanHeader.Status);
                Assert.AreEqual(newJobNo, ekanbanHeader.OutJobNo);

                // inventory allocated qty changed 
                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(5000, inventoryEHP.AllocatedQty);
                Assert.AreEqual(5, inventoryEHP.AllocatedPkg);

                var inventorySupplier = context.Inventory.Where(i => i.Ownership == (int)Ownership.Supplier).Single();
                Assert.AreEqual(5200, inventorySupplier.AllocatedQty);
                Assert.AreEqual(7, inventorySupplier.AllocatedPkg);
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_MissingStock()
        {
            var productCode2 = productCode + "2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    //Instructions = "EHP"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                string locationCode = "LOC1";
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                Assert.IsTrue(result.Errors.Any(e => JObject.Parse(e)["MessageKey"].ToString() == "NoStockFoundForEKanban"));
            }

        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_NonCPart_EHP_TheSameSupplier()
        {
            var productCode2 = productCode + "2";
            var pid2 = "PID2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    Instructions = "EHP"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                string locationCode = "LOC1";
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });

                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.SaveChanges();
            }

            string newJobNo = null;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newJobNo = result.Data;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // 1 outbound row created
                var outbound = context.Outbounds.Single();
                Assert.AreEqual(newJobNo, outbound.JobNo);
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(refno, outbound.RefNo);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE2", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual("Company1", outbound.Remark);

                // 1 outbound detail line created
                var outboundDetail = context.OutboundDetails.Single();
                Assert.AreEqual(newJobNo, outboundDetail.JobNo);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(200, outboundDetail.Qty);
                Assert.AreEqual(200, outboundDetail.PickedQty);
                Assert.AreEqual(2, outboundDetail.Pkg);
                Assert.AreEqual(2, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);

                //2 picking lists created
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(2, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(100, pickingList1.Qty);
                var pickingList2 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(newJobNo, pickingList2.JobNo);
                Assert.AreEqual(100, pickingList2.Qty);

                //2 PickingListEKanban created
                var pickingListsEKanbans = context.PickingListEKanbans.ToList();
                Assert.AreEqual(2, pickingListsEKanbans.Count());
                var pickingListsEKanban1 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban1.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban1.OrderNo);
                Assert.AreEqual("1", pickingListsEKanban1.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban1.ProductCode);
                var pickingListsEKanban2 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban2.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban2.OrderNo);
                Assert.AreEqual("2", pickingListsEKanban2.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban2.ProductCode);

                // storage status changed
                var storage = context.StorageDetails.Where(s => s.ProductCode == productCode).ToArray();
                Assert.AreEqual(StorageStatus.Allocated, storage[0].Status);
                Assert.AreEqual(StorageStatus.Allocated, storage[1].Status);
                Assert.AreEqual("", storage[0].OutJobNo);
                Assert.AreEqual("", storage[1].OutJobNo);
                Assert.AreEqual(100, storage[0].AllocatedQty);
                Assert.AreEqual(100, storage[1].AllocatedQty);

                // eKanbanDetail changes
                var ekanbanDetails = context.EKanbanDetails.ToArray();
                Assert.AreEqual(100, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(supplierID, ekanbanDetails[0].SupplierID);
                Assert.AreEqual(100, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(supplierID, ekanbanDetails[1].SupplierID);

                // eKanbanHeader changed
                var ekanbanHeader = context.EKanbanHeaders.First();
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanbanHeader.Status);
                Assert.AreEqual(newJobNo, ekanbanHeader.OutJobNo);

                // inventory allocated qty changed 
                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(5200, inventoryEHP.AllocatedQty);
                Assert.AreEqual(7, inventoryEHP.AllocatedPkg);

                var inventorySupplier = context.Inventory.Where(i => i.Ownership == (int)Ownership.Supplier).Single();
                Assert.AreEqual(5000, inventorySupplier.AllocatedQty);
                Assert.AreEqual(5, inventorySupplier.AllocatedPkg);
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_NonCPart_MixedOwnership()
        {
            var productCode2 = productCode + "2";
            var pid2 = "PID2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    Instructions = "DEJLIT"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                string locationCode = "LOC1";
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.Supplier
                });

                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.SaveChanges();
            }

            string newJobNo = null;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newJobNo = result.Data;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // 1 outbound row created
                var outbounds = context.Outbounds.ToList();

                var outbound = outbounds.First();
                Assert.AreEqual(newJobNo, outbound.JobNo);
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(refno, outbound.RefNo);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE2", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.IsTrue(outbound.Remark.Contains("Split"));

                var outbound2 = outbounds.Last();
                Assert.IsTrue(outbound2.JobNo.EndsWith("2"));
                Assert.AreEqual(factoryID, outbound2.CustomerCode);
                Assert.AreEqual(whscode, outbound2.WHSCode);
                Assert.AreNotEqual(refno, outbound2.RefNo);// new EKanbanHeader created
                Assert.AreEqual(OutboundType.EKanban, outbound2.TransType);
                Assert.IsNotNull(outbound2.ETD);
                Assert.AreEqual("USERCODE2", outbound2.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound2.Status);
                Assert.IsTrue(outbound2.Remark.Contains("Split"));

                // 1 outbound detail line per outbound created
                var outboundDetails = context.OutboundDetails.ToList();
                var outboundDetail = context.OutboundDetails.Where(o => o.JobNo == outbound.JobNo).SingleOrDefault();
                var outboundDetail2 = context.OutboundDetails.Where(o => o.JobNo == outbound2.JobNo).SingleOrDefault();
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(100, outboundDetail.Qty);
                Assert.AreEqual(100, outboundDetail.PickedQty);
                Assert.AreEqual(1, outboundDetail.Pkg);
                Assert.AreEqual(1, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetail.Status);
               
                Assert.AreEqual(productCode, outboundDetail2.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail2.SupplierID);
                Assert.AreEqual(1, outboundDetail2.LineItem);
                Assert.AreEqual(100, outboundDetail2.Qty);
                Assert.AreEqual(100, outboundDetail2.PickedQty);
                Assert.AreEqual(1, outboundDetail2.Pkg);
                Assert.AreEqual(1, outboundDetail2.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetail2.Status);

                //2 picking lists created
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(2, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.JobNo == outbound.JobNo && pl.LineItem == 1 && pl.SeqNo == 1).FirstOrDefault();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(100, pickingList1.Qty);
                var pickingList2 = context.PickingLists.Where(pl => pl.JobNo == outbound2.JobNo && pl.LineItem == 1 && pl.SeqNo == 2).FirstOrDefault();
                Assert.IsNotNull(pickingList2);
                Assert.AreEqual(100, pickingList2.Qty);
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_CPart_NonEHP_TheSameSupplier()
        {
            var productCode2 = productCode + "2";
            string locationCode = "LOC1";
            string locationCode2 = "LOC2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    //Instructions = "EHP"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                //context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                //{
                //    OrderNo = refno,
                //    ProductCode = productCode2,
                //    SerialNo = "1",
                //    SupplierID = supplierID,
                //    DropPoint = "01",
                //    Quantity = 500
                //});
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 1,
                    CPartSPQ = 50
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode2,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode2,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 300,
                    OriginalQty = 300,
                    QtyPerPkg = 50,
                    AllocatedQty = 50,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode2,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 200,
                    OriginalQty = 200,
                    QtyPerPkg = 50,
                    AllocatedQty = 50,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway
                });

                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });

                //context.PickingAllocatedPIDs.Add(new Core.Entities.PickingAllocatedPID()
                //{
                //    JobNo = "job1",
                //    LineItem = 2,
                //    AllocatedQty = 10,
                //    PickedQty = 10,
                //    PID = pid2,
                //    SerialNo = 1
                //});
                //context.PickingAllocatedPIDs.Add(new Core.Entities.PickingAllocatedPID()
                //{
                //    JobNo = "job2",
                //    LineItem = 1,
                //    AllocatedQty = 10,
                //    PickedQty = 10,
                //    PID = pid,
                //    SerialNo = 1
                //});
                context.SaveChanges();
            }

            string newJobNo = null;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newJobNo = result.Data;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // 1 outbound row created
                var outbound = context.Outbounds.Single();
                Assert.AreEqual(newJobNo, outbound.JobNo);
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(refno, outbound.RefNo);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE2", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual("Company1", outbound.Remark);

                // 1 outbound detail line created
                var outboundDetail = context.OutboundDetails.Single();
                Assert.AreEqual(newJobNo, outboundDetail.JobNo);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(200, outboundDetail.Qty);
                Assert.AreEqual(200, outboundDetail.PickedQty);
                Assert.AreEqual(4, outboundDetail.Pkg);
                Assert.AreEqual(4, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);
               
                // 4 AllocatedPIDs are createed
                var allocatedPIDs = context.PickingAllocatedPIDs.OrderBy(p => p.LineItem).ThenBy(p => p.SerialNo).ToList();
                Assert.AreEqual(4, allocatedPIDs.Count());
                var allocatedPID1 = allocatedPIDs[0];
                var allocatedPID2 = allocatedPIDs[1];
                var allocatedPID3 = allocatedPIDs[2];
                var allocatedPID4 = allocatedPIDs[3];
                Assert.AreEqual(newJobNo, allocatedPID1.JobNo);
                Assert.AreEqual(newJobNo, allocatedPID2.JobNo);
                Assert.AreEqual(newJobNo, allocatedPID3.JobNo);
                Assert.AreEqual(newJobNo, allocatedPID4.JobNo);

                Assert.AreEqual(1, allocatedPID1.LineItem);
                Assert.AreEqual(1, allocatedPID2.LineItem);
                Assert.AreEqual(1, allocatedPID3.LineItem);
                Assert.AreEqual(1, allocatedPID4.LineItem);

                Assert.AreEqual(1, allocatedPID1.SerialNo);
                Assert.AreEqual(2, allocatedPID2.SerialNo);
                Assert.AreEqual(3, allocatedPID3.SerialNo);
                Assert.AreEqual(4, allocatedPID4.SerialNo);

                Assert.AreEqual(pid2, allocatedPID1.PID);
                Assert.AreEqual(pid2, allocatedPID2.PID);
                Assert.AreEqual(pid2, allocatedPID3.PID);
                Assert.AreEqual(pid, allocatedPID4.PID);

                Assert.AreEqual(50, allocatedPID1.AllocatedQty);
                Assert.AreEqual(50, allocatedPID2.AllocatedQty);
                Assert.AreEqual(50, allocatedPID3.AllocatedQty);
                Assert.AreEqual(50, allocatedPID4.AllocatedQty);

                Assert.AreEqual(null, allocatedPID1.PickedQty);
                Assert.AreEqual(null, allocatedPID2.PickedQty);
                Assert.AreEqual(null, allocatedPID3.PickedQty);
                Assert.AreEqual(null, allocatedPID4.PickedQty);

                //4 picking lists created
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(4, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                var pickingList2 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                var pickingList3 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 3).Single();
                var pickingList4 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 4).Single();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(newJobNo, pickingList2.JobNo);
                Assert.AreEqual(newJobNo, pickingList3.JobNo);
                Assert.AreEqual(newJobNo, pickingList4.JobNo);

                Assert.AreEqual(1, pickingList1.LineItem);
                Assert.AreEqual(1, pickingList2.LineItem);
                Assert.AreEqual(1, pickingList3.LineItem);
                Assert.AreEqual(1, pickingList4.LineItem);

                Assert.AreEqual(locationCode2, pickingList1.LocationCode);
                Assert.AreEqual(locationCode2, pickingList2.LocationCode);
                Assert.AreEqual(locationCode2, pickingList3.LocationCode);
                Assert.AreEqual(locationCode, pickingList4.LocationCode);

                Assert.AreEqual(1, pickingList1.SeqNo);
                Assert.AreEqual(2, pickingList2.SeqNo);
                Assert.AreEqual(3, pickingList3.SeqNo);
                Assert.AreEqual(4, pickingList4.SeqNo);

                Assert.AreEqual(50, pickingList1.Qty);
                Assert.AreEqual(50, pickingList2.Qty);
                Assert.AreEqual(50, pickingList3.Qty);
                Assert.AreEqual(50, pickingList4.Qty);

                Assert.AreEqual("PL", pickingList1.WHSCode);
                Assert.AreEqual("PL", pickingList2.WHSCode);
                Assert.AreEqual("PL", pickingList3.WHSCode);
                Assert.AreEqual("PL", pickingList4.WHSCode);

                Assert.AreEqual(DateTime.Now.Date.AddDays(-15), pickingList1.InboundDate.Date);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-15), pickingList2.InboundDate.Date);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-15), pickingList3.InboundDate.Date);
                Assert.AreEqual(DateTime.Now.Date.AddDays(-5), pickingList4.InboundDate.Date);

                // 4 EKanbanDetail rows now present (2 new created)
                var ekanbanDetails = context.EKanbanDetails.OrderBy(d => d.SerialNo).ToList();
                Assert.AreEqual(4, ekanbanDetails.Count());
                Assert.AreEqual(50, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(50, ekanbanDetails[1].QuantitySupplied);
                Assert.AreEqual(50, ekanbanDetails[2].QuantitySupplied);
                Assert.AreEqual(50, ekanbanDetails[3].QuantitySupplied);

                Assert.AreEqual("1", ekanbanDetails[0].SerialNo);
                Assert.AreEqual("2", ekanbanDetails[1].SerialNo);
                Assert.AreEqual("3", ekanbanDetails[2].SerialNo);
                Assert.AreEqual("4", ekanbanDetails[3].SerialNo);

                Assert.AreEqual(100, ekanbanDetails[0].Quantity);
                Assert.AreEqual(100, ekanbanDetails[1].Quantity);
                Assert.AreEqual(0, ekanbanDetails[2].Quantity);
                Assert.AreEqual(0, ekanbanDetails[3].Quantity);

                Assert.AreEqual("1", ekanbanDetails[0].SerialNo);
                Assert.AreEqual("2", ekanbanDetails[1].SerialNo);
                Assert.AreEqual("3", ekanbanDetails[2].SerialNo);
                Assert.AreEqual("4", ekanbanDetails[3].SerialNo);

                //4 PickingListEKanban created
                var pickingListsEKanbans = context.PickingListEKanbans.ToList();
                Assert.AreEqual(4, pickingListsEKanbans.Count());
                var pickingListsEKanban1 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban1.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban1.OrderNo);
                Assert.AreEqual("1", pickingListsEKanban1.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban1.ProductCode);
                
                var pickingListsEKanban2 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban2.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban2.OrderNo);
                Assert.AreEqual("2", pickingListsEKanban2.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban2.ProductCode);

                var pickingListsEKanban3 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 3).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban3.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban3.OrderNo);
                Assert.AreEqual("3", pickingListsEKanban3.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban3.ProductCode);

                var pickingListsEKanban4 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 4).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban4.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban4.OrderNo);
                Assert.AreEqual("4", pickingListsEKanban4.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban4.ProductCode);

                // storage status changed
                var storage = context.StorageDetails.Where(s => s.ProductCode == productCode).OrderBy(s => s.InboundDate).ToList();
                Assert.AreEqual(StorageStatus.Allocated, storage[0].Status);
                Assert.AreEqual(StorageStatus.Allocated, storage[1].Status);
                Assert.AreEqual("", storage[0].OutJobNo);
                Assert.AreEqual("", storage[1].OutJobNo);
                Assert.AreEqual(pid2, storage[0].PID);
                Assert.AreEqual(pid, storage[1].PID);
             
                Assert.AreEqual(200, storage[0].OriginalQty);
                Assert.AreEqual(300, storage[1].OriginalQty);


                Assert.AreEqual(200, storage[0].AllocatedQty);
                Assert.AreEqual(100, storage[1].AllocatedQty);
                Assert.AreEqual(200, storage[0].Qty);
                Assert.AreEqual(300, storage[1].Qty);

                // eKanbanHeader changed
                var ekanbanHeader = context.EKanbanHeaders.First();
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanbanHeader.Status);
                Assert.AreEqual(newJobNo, ekanbanHeader.OutJobNo);

                // inventory allocated qty changed 
                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(5000, inventoryEHP.AllocatedQty);
                Assert.AreEqual(5, inventoryEHP.AllocatedPkg);

                var inventorySupplier = context.Inventory.Where(i => i.Ownership == (int)Ownership.Supplier).Single();
                Assert.AreEqual(5200, inventorySupplier.AllocatedQty);
                Assert.AreEqual(5, inventorySupplier.AllocatedPkg);// we do not seem to update the qty for cpart
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_EHP_NoSupplier_STOMessage()
        {
            var productCode2 = productCode + "2";
            var pid2 = "PID2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    Status = (byte)EKanbanStatus.New,
                    Instructions = "EHP",
                    ETA = DateTime.Now,
                    BlanketOrderNo = "0186001263",
                    RefNo = "1100001"
                });
                context.EKanbanDetails.Add(new EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    DropPoint = "01",
                    Quantity = 100,
                    ExternalLineItem = 10
                });
                context.PartMasters.Add(new PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                var locationCode = "LOC1";
                context.Locations.Add(new Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new StorageDetail()
                {
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });

                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.SaveChanges();
            }

            string newJobNo = null;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newJobNo = result.Data;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // 1 outbound row created
                var outbound = context.Outbounds.Single();
                Assert.AreEqual(newJobNo, outbound.JobNo);
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(refno, outbound.RefNo);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE2", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual(string.Empty, outbound.Remark);

                // 1 outbound detail line created
                var outboundDetail = context.OutboundDetails.Single();
                Assert.AreEqual(newJobNo, outboundDetail.JobNo);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(100, outboundDetail.Qty);
                Assert.AreEqual(100, outboundDetail.PickedQty);
                Assert.AreEqual(1, outboundDetail.Pkg);
                Assert.AreEqual(1, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);

                //1 picking list created
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(1, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(100, pickingList1.Qty);

                //1 PickingListEKanban created
                var pickingListsEKanbans = context.PickingListEKanbans.ToList();
                Assert.AreEqual(1, pickingListsEKanbans.Count());
                var pickingListsEKanban1 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban1.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban1.OrderNo);
                Assert.AreEqual("1", pickingListsEKanban1.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban1.ProductCode);

                // storage status changed
                var storage = context.StorageDetails.Where(s => s.ProductCode == productCode).ToArray();
                Assert.AreEqual(StorageStatus.Allocated, storage[1].Status);
                Assert.AreEqual("", storage[1].OutJobNo);
                Assert.AreEqual(100, storage[1].AllocatedQty);

                // eKanbanDetail changes
                var ekanbanDetails = context.EKanbanDetails.ToArray();
                Assert.AreEqual(100, ekanbanDetails[0].QuantitySupplied);
                Assert.AreEqual(supplierID, ekanbanDetails[0].SupplierID);

                // eKanbanHeader changed
                var ekanbanHeader = context.EKanbanHeaders.First();
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanbanHeader.Status);
                Assert.AreEqual(newJobNo, ekanbanHeader.OutJobNo);

                // inventory allocated qty changed 
                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(5100, inventoryEHP.AllocatedQty);
                Assert.AreEqual(6, inventoryEHP.AllocatedPkg);
            }
        }

        [TestMethod]
        public async Task SplitOutbound_DuplicatePickingListsPassed_ShouldNotThrowException()
        {
            string firstJobNo = await AddSplitTestData();
            var splitOutboundDto = new SplitOutboundDto()
            {
                JobNo = firstJobNo,
                OwnershipSplit = false,
                PickingListItemIds = new List<PickingListItemId>()
                {
                    new PickingListItemId { JobNo = firstJobNo, LineItem = 1, SeqNo = 1 },
                    new PickingListItemId { JobNo = firstJobNo, LineItem = 1, SeqNo = 1 }
                }
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.SplitOutbound(splitOutboundDto, "USERCODE3");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task SplitOutbound_NonOwnershipSplit()
        {
            string firstJobNo = await AddSplitTestData();
            var splitOutboundDto = new SplitOutboundDto()
            {
                JobNo = firstJobNo,
                OwnershipSplit = false,
                PickingListItemIds = new List<PickingListItemId>()
                {
                    new PickingListItemId { JobNo = firstJobNo, LineItem = 1, SeqNo = 1 }
                }
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.SplitOutbound(splitOutboundDto, "USERCODE3");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // 1 additional outbound row created
                var outbounds = context.Outbounds.OrderBy(o => o.JobNo).ToList();
                Assert.AreEqual(2, outbounds.Count());

                var outbound = outbounds.Last();
                Assert.AreNotEqual(firstJobNo, outbound.JobNo);
                var newJobNo = outbound.JobNo;
                Assert.AreEqual(factoryID, outbound.CustomerCode);
                Assert.AreEqual(whscode, outbound.WHSCode);
                Assert.AreEqual(OutboundType.EKanban, outbound.TransType);
                Assert.IsNotNull(outbound.ETD);
                Assert.AreEqual("USERCODE3", outbound.CreatedBy);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual($"Splitted from {firstJobNo}", outbound.Remark);

                // 1 outbound detail line created
                var outboundDetails = context.OutboundDetails.Where(o => o.JobNo == newJobNo).ToList();
                Assert.AreEqual(1, outboundDetails.Count());

                var outboundDetail = outboundDetails.Single();
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(supplierID, outboundDetail.SupplierID);
                Assert.AreEqual(1, outboundDetail.LineItem);
                Assert.AreEqual(productCode, outboundDetail.ProductCode);
                Assert.AreEqual(100, outboundDetail.Qty);
                Assert.AreEqual(100, outboundDetail.PickedQty);
                Assert.AreEqual(1, outboundDetail.Pkg);
                Assert.AreEqual(1, outboundDetail.PickedPkg);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetail.Status);

                // picking list moved from one outbound to another
                var pickingLists = context.PickingLists.ToList();
                Assert.AreEqual(2, pickingLists.Count());
                var pickingList1 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingList1.JobNo);
                Assert.AreEqual(100, pickingList1.Qty);
                var pickingList2 = context.PickingLists.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(firstJobNo, pickingList2.JobNo);
                Assert.AreEqual(100, pickingList2.Qty);

                // eKanbanHeader created
                var ekanbanHeaders = context.EKanbanHeaders.OrderBy(e => e.OrderNo).ToList();
                var ekanbanHeader = ekanbanHeaders.Last();
                Assert.AreNotEqual(refno, ekanbanHeader.OrderNo);
                var newOrderNo = ekanbanHeader.OrderNo;
                Assert.AreEqual($"9{DateTime.Now:yyMMdd}001", newOrderNo);
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanbanHeader.Status);
                Assert.AreEqual(newJobNo, ekanbanHeader.OutJobNo);
                Assert.AreEqual(refno, ekanbanHeader.RefNo);

                Assert.AreEqual(newOrderNo, outbound.RefNo);

                // eKanbanDetail moved to new eKanban header
                var ekanbanDetail = context.EKanbanDetails.Where(e => e.OrderNo == newOrderNo).FirstOrDefault();
                Assert.IsNotNull(ekanbanDetail);

                //PickingListEKanban moved from one outbound to another
                var pickingListsEKanbans = context.PickingListEKanbans.ToList();
                Assert.AreEqual(2, pickingListsEKanbans.Count());
                var pickingListsEKanban1 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 1).Single();
                Assert.AreEqual(newJobNo, pickingListsEKanban1.JobNo);
                Assert.AreEqual(newOrderNo, pickingListsEKanban1.OrderNo);
                Assert.AreEqual("1", pickingListsEKanban1.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban1.ProductCode);

                var pickingListsEKanban2 = context.PickingListEKanbans.Where(pl => pl.LineItem == 1 && pl.SeqNo == 2).Single();
                Assert.AreEqual(firstJobNo, pickingListsEKanban2.JobNo);
                Assert.AreEqual(refno, pickingListsEKanban2.OrderNo);
                Assert.AreEqual("2", pickingListsEKanban2.SerialNo);
                Assert.AreEqual(productCode, pickingListsEKanban2.ProductCode);

                // storage status NOT changed (as we do not have PIDs and OutJobNo was null anyway)
                var storage = context.StorageDetails.Where(s => s.ProductCode == productCode).ToArray();
                Assert.AreEqual(StorageStatus.Allocated, storage[0].Status);
                Assert.AreEqual(StorageStatus.Allocated, storage[1].Status);
                Assert.AreEqual("", storage[0].OutJobNo);
                Assert.AreEqual("", storage[1].OutJobNo);
                Assert.AreEqual(100, storage[0].AllocatedQty);
                Assert.AreEqual(100, storage[1].AllocatedQty);

                // inventory allocated qty NOT changed 
                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(5000, inventoryEHP.AllocatedQty);
                Assert.AreEqual(5, inventoryEHP.AllocatedPkg);

                var inventorySupplier = context.Inventory.Where(i => i.Ownership == (int)Ownership.Supplier).Single();
                Assert.AreEqual(5200, inventorySupplier.AllocatedQty);
                Assert.AreEqual(7, inventorySupplier.AllocatedPkg);
            }
        }

        [TestMethod]
        public async Task SplitOutbound_ByOwnership()
        {
            //var firstJobNo = await AddSplitTestData(mixOwnership: true);
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    Status = (byte)EKanbanStatus.New,
                    Instructions = "",
                    ETA = DateTime.Now,
                    BlanketOrderNo = "0186001263",
                    RefNo = "1100001"
                });
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
                    InJobNo = injobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ParentID = "",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    InboundDate = DateTime.Now.Date.AddDays(-30),
                    OriginalQty = 100,
                    Qty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = jobno,
                    WHSCode = whscode,
                    LocationCode = "L4",
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    InJobNo = injobno,
                    LineItem = 1,
                    SeqNo = 2,
                    ParentID = "",
                    ProductCode = productCode2,
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
                    Ownership = Ownership.Supplier
                });

                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    PickedQty = 0,
                    Pkg = 1,
                    PickedPkg = 0,
                    Status = (int)OutboundDetailStatus.New,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    ProductCode = productCode2,
                    SupplierID = supplierID,
                    Qty = 100,
                    PickedQty = 0,
                    Pkg = 1,
                    PickedPkg = 0,
                    Status = (int)OutboundDetailStatus.New,
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

                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "HOS PASS C346 VEVA ADP LH ACTUAL",
                    UOM = uom,
                    PackageType = "PKG0002",
                    SPQ = 100,
                    CPartSPQ = 1,
                    IsCPart = 0
                });

                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "HOS PASS C346 VEVA ADP LH ACTUAL 2",
                    UOM = uom,
                    PackageType = "PKG0002",
                    SPQ = 100,
                    CPartSPQ = 1,
                    IsCPart = 0
                });

                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 1,
                    ProductCode = productCode2,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2
                });

                //var pickingLists = context.PickingLists.ToList();
                //var pl1 = pickingLists[0];
                //pl1.PickedBy = "PICKUSER";
                //pl1.PID = context.StorageDetails.First().PID;
                //pl1.PickedDate = DateTime.Now.AddDays(-1);

                //var pl2 = pickingLists[1];
                //pl2.PickedBy = "PICKUSER";
                //pl2.PID = context.StorageDetails.Last().PID;
                //pl2.PickedDate = DateTime.Now.AddDays(-1);

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.SplitOutboundByOwnership(jobno, "USERCODE3");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                // there should be 2 outbounds: 1 with EHP ownership, and a new outbound with Supplier ownership
                var outbounds = context.Outbounds.OrderBy(o => o.JobNo).ToList();
                Assert.AreEqual(2, outbounds.Count);

                var ehpOutbound = outbounds[0];
                var supplierOutbound = outbounds[1];
                var ehpOutboundDetail = context.OutboundDetails.Where(o => o.JobNo == ehpOutbound.JobNo).ToList();
                var supplierOutboundDetail = context.OutboundDetails.Where(o => o.JobNo == supplierOutbound.JobNo).ToList();

                Assert.IsTrue(ehpOutbound.Remark.StartsWith("(EHP)"));
                Assert.IsTrue(supplierOutbound.Remark.StartsWith("(Supplier)"));

                var ehpList = context.PickingLists.Where(p => p.JobNo == ehpOutbound.JobNo && p.LineItem == ehpOutboundDetail.First().LineItem).ToList();
                var supplierList = context.PickingLists.Where(p => p.JobNo == supplierOutbound.JobNo && p.LineItem == supplierOutboundDetail.First().LineItem).ToList();

                Assert.IsTrue(ehpList.All(i => i.PID == pid));
                Assert.IsTrue(supplierList.All(i => i.PID == pid2));
                Assert.AreEqual(1, ehpList.Count);
                Assert.AreEqual(1, supplierList.Count);
            }
        }

        [TestMethod]
        public async Task SplitOutbound_NonOwnershipSplit_AlreadyPicked()
        {
            string firstJobNo = await AddSplitTestData();
            using (var context = new Context(options, appSettings.Object))
            {
                foreach (var pl in context.PickingLists.ToList())
                {
                    var storage = context.StorageDetails.Where(s => s.LocationCode == pl.LocationCode).First();
                    pl.PickedBy = "PICKUSER";
                    pl.PID = storage.PID;
                    pl.PickedDate = DateTime.Now.AddDays(-1);
                    context.SaveChanges();
                }
            }
            var splitOutboundDto = new SplitOutboundDto()
            {
                JobNo = firstJobNo,
                OwnershipSplit = false,
                PickingListItemIds = new List<PickingListItemId>()
                {
                    new PickingListItemId { JobNo = firstJobNo, LineItem = 1, SeqNo = 1 }
                }
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.SplitOutbound(splitOutboundDto, "USERCODE3");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.OrderBy(o => o.JobNo).Last();
                var outboundDetail = context.OutboundDetails.Where(o => o.JobNo == outbound.JobNo).Single();
                Assert.AreEqual(OutboundStatus.Picked, outbound.Status);
                Assert.AreEqual((int)OutboundDetailStatus.Picked, outboundDetail.Status);
            }
        }

        [TestMethod]
        public async Task SplitOutbound_NonOwnershipSplit_EOrdersExist()
        {
            string firstJobNo = await AddSplitTestData();
            using (var context = new Context(options, appSettings.Object))
            {
                foreach (var pl in context.PickingListEKanbans.ToList())
                {
                    var eorder = new Core.Entities.EOrder()
                    {
                        PartNo = productCode,
                        PurchaseOrderNo = refno,
                        CardSerial = pl.SerialNo,
                        FactoryID = factoryID,
                        UnitOfMeasure = uom,
                        OrderQuantity = "2000",
                        StoreDropPoint = "01",
                        EHPFilledInDate = DateTime.Now.ToString()
                    };
                    context.EOrders.Add(eorder);
                    context.SaveChanges();
                }
            } 
            var splitOutboundDto = new SplitOutboundDto()
            {
                JobNo = firstJobNo,
                OwnershipSplit = false,
                PickingListItemIds = new List<PickingListItemId>()
                {
                    new PickingListItemId { JobNo = firstJobNo, LineItem = 1, SeqNo = 1 }
                }
            };

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.SplitOutbound(splitOutboundDto, "USERCODE3");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var newOrderNo = context.EKanbanHeaders.OrderBy(e => e.OrderNo).Last().OrderNo;
                var eorders = context.EOrders.ToList();
                Assert.AreEqual(2, eorders.Count());
                Assert.IsNotNull(eorders.Where(e => e.PurchaseOrderNo == newOrderNo).SingleOrDefault());
            }
        }

        [TestMethod]
        public async Task UndoPicking_IncorrectStorageStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });
                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = refno,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType = OutboundType.EKanban,
                    Remark = "Remark",
                    Status = 0,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });

                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    InJobNo = injobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ParentID = "",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    InboundDate = DateTime.Now.Date.AddDays(-30),
                    OriginalQty = 150,
                    Qty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 100,
                    OutJobNo = jobno,
                    WHSCode = whscode,
                    LocationCode = "L4",
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    InJobNo = injobno,
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
                    PID = pid3,
                    InJobNo = injobno,
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
                    Qty = 100,
                    PickedQty = 100,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 640,
                    PickedQty = 0,
                    Pkg = 0,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 3,
                    ProductCode = "132732025",
                    SupplierID = "504163",
                    Qty = 640,
                    PickedQty = 640,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });

                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 1,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 3,
                    SeqNo = 1,
                    ProductCode = "132732025",
                    SupplierID = supplierID,
                    Qty = 50,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid3
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
                    UOM = uom,
                    PackageType = "PKG0002",
                    SPQ = 144,
                    CPartSPQ = 1,
                    IsCPart = 0
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UndoPicking(jobno, new string[] { pid });
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task UndoPicking()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });
                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = refno,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType =  OutboundType.EKanban,
                    Remark = "Remark",
                    Status = 0,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });

                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    InJobNo = injobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ParentID = "",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    InboundDate = DateTime.Now.Date.AddDays(-30),
                    OriginalQty = 150,
                    Qty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 100,
                    OutJobNo = jobno,
                    WHSCode = whscode,
                    LocationCode = "L4",
                    Status = StorageStatus.Picked,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    InJobNo = injobno,
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
                    Status = StorageStatus.Packed,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid3,
                    InJobNo = injobno,
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
                    Status = StorageStatus.Picked,
                    Ownership = Ownership.EHP
                });

                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    PickedQty = 100,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 640,
                    PickedQty = 0,
                    Pkg = 0,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 3,
                    ProductCode = "132732025",
                    SupplierID = "504163",
                    Qty = 640,
                    PickedQty = 640,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });

                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 1,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 3,
                    SeqNo = 1,
                    ProductCode = "132732025",
                    SupplierID = supplierID,
                    Qty = 50,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid3
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
                    UOM = uom,
                    PackageType = "PKG0002",
                    SPQ = 144,
                    CPartSPQ = 1,
                    IsCPart = 0
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.UndoPicking(jobno,new string[] { pid });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var pickingLists = context.PickingLists.OrderBy(l => l.LineItem).ToList();
                Assert.AreEqual("", pickingLists[0].PID);
                Assert.AreEqual("", pickingLists[0].PickedBy);
                Assert.IsNull(pickingLists[0].PickedDate);
                Assert.AreEqual("", pickingLists[0].PackedBy);
                Assert.IsNull(pickingLists[0].PackedDate);

                Assert.AreEqual(pid2, pickingLists[1].PID);
                Assert.AreNotEqual("", pickingLists[1].PickedBy);
                Assert.IsNotNull(pickingLists[1].PickedDate);

                Assert.AreEqual(pid3, pickingLists[2].PID);
                Assert.AreNotEqual("", pickingLists[2].PickedBy);
                Assert.IsNotNull(pickingLists[2].PickedDate);

                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(StorageStatus.Allocated, storageDetail.Status);
                Assert.AreEqual("", storageDetail.OutJobNo);

                var outboundDetails = context.OutboundDetails.Where(d => d.JobNo == jobno).OrderBy(o => o.LineItem).ToList();
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetails[0].Status);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetails[1].Status);
                Assert.AreEqual((int)OutboundDetailStatus.Picking, outboundDetails[2].Status);

                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual(OutboundStatus.PartialPicked, outbound.Status);
            }
        }

        [TestMethod]
        public async Task CompleteWHSTransfer()
        {
            string newWHSCode = "NEWWHS";
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context);
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.WHSTransfer;
                outbound.NewWHSCode = newWHSCode;
                var od = context.OutboundDetails.Find(jobno, 1);
                od.PickedPkg = 1;
                od.PickedQty = 100;
                string inJobNo = "INJOB1";

                context.StorageDetails.Add(new Core.Entities.StorageDetail
                {
                    PID = pid,
                    LocationCode = "P4",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 100,
                    OutJobNo = "",
                    InJobNo = inJobNo,
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Picked,
                    Ownership = Ownership.EHP
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    DropPoint = "ZZ99",
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid,
                });
                context.Inbounds.Add(new Core.Entities.Inbound()
                {
                    JobNo = inJobNo,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = "ELPS-HT2020072502-507188",
                    Status = InboundStatus.Completed
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = newWHSCode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = newWHSCode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS { 
                    JobNo = jobno + "1",
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    WHSCode = whscode,
                    BalanceQty = 15000,
                    BalancePkg = 100,
                });
                context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS
                {
                    JobNo = jobno + "1",
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    WHSCode = newWHSCode,
                    BalanceQty = 15000,
                    BalancePkg = 100,
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteWHSTransfer(new string[] { jobno },"USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
      
            using (var context = new Context(options, appSettings.Object))
            {
                // whs Transfer record created
                var whsTransfer = context.WHSTransfers.Single();
                Assert.AreEqual(jobno, whsTransfer.JobNo);
                Assert.AreEqual(factoryID, whsTransfer. CustomerCode);
                Assert.AreEqual(whscode, whsTransfer. WHSCode);
                Assert.AreEqual(newWHSCode, whsTransfer. NewWHSCode);
                Assert.AreEqual((int)WHSTransferStatus.Completed, whsTransfer.Status);
                Assert.AreEqual("USERCODE2", whsTransfer.CreatedBy);
                Assert.AreEqual("USERCODE2", whsTransfer.ConfirmedBy);
                
                // whs Transfer detail record created
                var whsTranseferDetail = context.WHSTransferDetails.Single();
                Assert.AreEqual(jobno, whsTranseferDetail.JobNo);
                Assert.AreEqual(pid, whsTranseferDetail.PID);
                Assert.AreEqual(100, whsTranseferDetail.Qty);
                Assert.AreEqual(whscode, whsTranseferDetail.OldWHSCode);
                Assert.AreEqual("P4", whsTranseferDetail.OldLocationCode);
                Assert.AreEqual(newWHSCode, whsTranseferDetail.NewWHSCode);
                Assert.AreEqual("RECEIVING", whsTranseferDetail.NewLocationCode);
                Assert.AreEqual("USERCODE2", whsTranseferDetail.TransferredBy);
                Assert.AreEqual(Ownership.EHP, whsTranseferDetail.Ownership);

                // whs RECEIVING location added
                Assert.IsNotNull(context.Locations.Where(l => l.Code == "RECEIVING" && l.WHSCode == whscode).FirstOrDefault());

                // storage changed
                var storageDetail = context.StorageDetails.Find(pid);
                Assert.AreEqual(pid, storageDetail.PID);
                Assert.AreEqual(StorageStatus.Putaway, storageDetail.Status);
                Assert.AreEqual(0, storageDetail.AllocatedQty);
                Assert.AreEqual("", storageDetail.OutJobNo);
                Assert.AreEqual("RECEIVING", storageDetail.LocationCode);
                Assert.AreEqual(newWHSCode, storageDetail.WHSCode);

                // inventory qties changed
                var inventories = context.Inventory.ToList();
                var inventoryEHP = inventories.Where(i => i.Ownership == Ownership.EHP && i.WHSCode == whscode).First();//1 
                Assert.AreEqual(9900, inventoryEHP.OnHandQty);
                Assert.AreEqual(4900, inventoryEHP.AllocatedQty);
                Assert.AreEqual(9, inventoryEHP.OnHandPkg);
                Assert.AreEqual(4, inventoryEHP.AllocatedPkg);

                var inventoryEHPNewWHS = inventories.Where(i => i.Ownership == Ownership.EHP && i.WHSCode == newWHSCode).First();//1 
                Assert.AreEqual(10100, inventoryEHPNewWHS.OnHandQty);
                //Assert.AreEqual(11, inventoryEHPNewWHS.OnHandPkg);TODO this requires some investigation
                //Assert.AreEqual(5100, inventoryEHPNewWHS.AllocatedQty); // this is not set
                //Assert.AreEqual(6, inventoryEHPNewWHS.AllocatedPkg); // this is not set

                // new ekanban detail line created
                var ekanbanDetail = context.EKanbanDetails.Single();
                Assert.AreEqual(refno, ekanbanDetail.OrderNo);
                Assert.AreEqual(productCode, ekanbanDetail.ProductCode);
                Assert.AreEqual("1", ekanbanDetail.SerialNo);
                Assert.AreEqual(supplierID, ekanbanDetail.SupplierID);
                Assert.AreEqual(100, ekanbanDetail.Quantity);
                Assert.AreEqual(100, ekanbanDetail.QuantitySupplied);
                Assert.AreEqual(100, ekanbanDetail.QuantityReceived);
                Assert.AreEqual("ZZ99", ekanbanDetail.DropPoint);

                // new picking list ekanban detail line created
                var ekanbanpl = context.PickingListEKanbans.Single();
                Assert.AreEqual(jobno, ekanbanpl.JobNo);
                Assert.AreEqual(1, ekanbanpl.LineItem);
                Assert.AreEqual(1, ekanbanpl.SeqNo);
                Assert.AreEqual(refno, ekanbanpl.OrderNo);
                Assert.AreEqual("1", ekanbanpl.SerialNo);
                Assert.AreEqual(productCode, ekanbanpl.ProductCode);

                // pickingList.PID updated;
                var pl = context.PickingLists.Single();
                Assert.AreEqual("x" + pid, pl.PID);

                // inv transactions added
                var invTransactionsOldWHS = context.InvTransactionsPerWHS.Where(i => i.WHSCode == whscode).OrderBy(i => i.SystemDate).ToList();
                Assert.AreEqual(2, invTransactionsOldWHS.Count());
                var t1whs = invTransactionsOldWHS.Last();
                Assert.AreEqual(100, t1whs.Qty);
                Assert.AreEqual(1, t1whs.Pkg);
                Assert.AreEqual(14900, t1whs.BalanceQty);
                Assert.AreEqual(99, t1whs.BalancePkg);

                var invTransactionsNewWHS = context.InvTransactionsPerWHS.Where(i => i.WHSCode == newWHSCode).OrderBy(i => i.SystemDate).ToList();
                Assert.AreEqual(2, invTransactionsNewWHS.Count());
                var t1newwhs = invTransactionsNewWHS.Last();
                Assert.AreEqual(100, t1newwhs.Qty);
                Assert.AreEqual(1, t1newwhs.Pkg);
                Assert.AreEqual(15100, t1newwhs.BalanceQty);
                Assert.AreEqual(101, t1newwhs.BalancePkg);

                #region Step 3.2 : Update Outbound Status
                var outbound = context.Outbounds.Single();
                Assert.AreEqual(OutboundStatus.Completed, outbound.Status);
                Assert.AreEqual("USERCODE2", outbound.DispatchedBy);
                Assert.IsNotNull(outbound.DispatchedDate);
                #endregion

                #region Step 4 : Update EkanbanHeader
                var header = context.EKanbanHeaders.Find(refno);
                Assert.AreEqual((int)EKanbanStatus.Completed, header.Status);
                Assert.IsNotNull(header.ConfirmationDate);
                #endregion
            }
        }

        [TestMethod]
        public async Task ReleaseBondedStock_NoBondedStock()
        {
            AddDataForBonded();
            using (var context = new Context(options, appSettings.Object))
            {
                var storage = context.StorageDetails.Find(pid);
                storage.BondedStatus = (int)BondedStatus.NonBonded;
                context.SaveChanges();
            }

            var nowDate = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetOutboundService(context).ReleaseBondedStock(new OutboundDto
                {
                    JobNo = jobno,
                    Remark = "XRemark",
                    RefNo = "XRef",
                    ETD = nowDate,
                    Status = OutboundStatus.Downloaded,
                    CommInvNo = "XINV",
                    NoOfPallet = 10,
                    DeliveryTo = "XDeliveryTo"
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task ReleaseBondedStock_NoCommInvNo()
        {
            AddDataForBonded();
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                outbound.CommInvNo = "XXX";
                context.SaveChanges();
            }

            var nowDate = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetOutboundService(context).ReleaseBondedStock(new OutboundDto
                {
                    JobNo = jobno,
                    Remark = "XRemark",
                    RefNo = "XRef",
                    ETD = nowDate,
                    Status = OutboundStatus.Downloaded,
                    CommInvNo = "",
                    NoOfPallet = 10,
                    DeliveryTo = "XDeliveryTo"
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task ReleaseBondedStock_InvalidTransType()
        {
            AddDataForBonded();
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                outbound.TransType = OutboundType.ManualEntry;
                context.SaveChanges();
            }

            var nowDate = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetOutboundService(context).ReleaseBondedStock(new OutboundDto
                {
                    JobNo = jobno,
                    Remark = "XRemark",
                    RefNo = "XRef",
                    ETD = nowDate,
                    Status = OutboundStatus.Downloaded,
                    CommInvNo = "",
                    NoOfPallet = 10,
                    DeliveryTo = "XDeliveryTo"
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task ReleaseBondedStock_NotPicked()
        {
            AddDataForBonded();
            using (var context = new Context(options, appSettings.Object))
            {
                var pl = context.PickingLists.Find(jobno,1,1);
                pl.PickedBy = "";
                context.SaveChanges();
            }

            var nowDate = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetOutboundService(context).ReleaseBondedStock(new OutboundDto
                {
                    JobNo = jobno,
                    Remark = "XRemark",
                    RefNo = "XRef",
                    ETD = nowDate,
                    Status = OutboundStatus.Downloaded,
                    CommInvNo = "",
                    NoOfPallet = 10,
                    DeliveryTo = "XDeliveryTo"
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task ReleaseBondedStock()
        {
            AddDataForBonded();
            var nowDate = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetOutboundService(context).ReleaseBondedStock(new OutboundDto {
                    JobNo = jobno,
                    Remark = "XRemark",
                    RefNo = "XRef",
                    ETD = nowDate,
                    Status = OutboundStatus.Downloaded,
                    CommInvNo = "XINV",
                    NoOfPallet = 10,
                    DeliveryTo = "XDeliveryTo"
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);

            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outbound = context.Outbounds.Find(jobno);
                Assert.AreEqual("XRemark", outbound.Remark);
                Assert.AreNotEqual("XRef", outbound.RefNo);
                Assert.AreEqual(nowDate, outbound.ETD);
                Assert.AreEqual(OutboundStatus.NewJob, outbound.Status);
                Assert.AreEqual("XINV", outbound.CommInvNo);
                Assert.AreEqual("XDeliveryTo", outbound.DeliveryTo);
                Assert.AreEqual(10, outbound.NoOfPallet);

                var sd = context.StorageDetails.Find(pid);
                Assert.AreEqual((int)BondedStatus.NonBonded, sd.BondedStatus);

                var rbs = context.OutboundReleaseBondedLogs.Find(jobno, pid);
                Assert.IsNotNull(rbs);
                Assert.AreEqual("USERCODE2", rbs.ReleasedBy);
            }
        }

        [TestMethod]
        public async Task CompleteDiscrepancyOutbound()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = refno,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType =  OutboundType.EKanban,
                    Remark = "Remark",
                    Status = OutboundStatus.NewJob,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.CompleteDiscrepancyOutbound(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object)) 
            {
                Assert.AreEqual(OutboundStatus.Completed, context.Outbounds.Find(jobno).Status);
            }
        }

        Mock<IRawSqlExecutor> sqlExecutor;

        private IOutboundService GetOutboundService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, sqlExecutor.Object);
            var utilityService = new UtilityService(repository, appSettings.Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var eKanbanService = new EKanbanService(repository, utilityService, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var reportService = new Mock<IReportService>().Object;
            var labelProvider = new Mock<ILabelProvider>().Object;
            var billingService = new BillingService(repository);
            var storageService = new StorageService(repository, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var iLogConnect = new Mock<IILogConnect>().Object;
            return new OutboundService(repository, appSettings.Object, mapper, utilityService, eKanbanService, 
                reportService, logger.Object, storageService, locker, iLogConnect, new Logger<OutboundService>(loggerFactory), billingService, null);
        }

        private async Task<string> AddSplitTestData(bool mixOwnership = false)
        {
            var productCode2 = productCode + "2";
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.UOMs.Add(new Core.Entities.UOM()
                {
                    Code = uom,
                    Name = uom,
                    Status = 1
                });
                context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
                {
                    UOM = uom,
                    CustomerCode = factoryID,
                    Status = 1,
                    DecimalNum = 3
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = refno,
                    FactoryID = factoryID,
                    IssuedDate = DateTime.Now.Date.AddDays(-50),
                    CreatedDate = DateTime.Now.Date.AddDays(-50),
                    //OutJobNo = jobno,
                    Status = (byte)EKanbanStatus.New,
                    //Instructions = "EHP"
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "1",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail()
                {
                    OrderNo = refno,
                    ProductCode = productCode,
                    SerialNo = "2",
                    SupplierID = supplierID,
                    DropPoint = "01",
                    Quantity = 100
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    Description = "Desc1",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.PartMasters.Add(new Core.Entities.PartMaster()
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode2,
                    Description = "Desc2",
                    UOM = uom,
                    SPQ = 100,
                    IsCPart = 0
                });
                context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
                {
                    SupplierID = supplierID,
                    FactoryID = factoryID,
                    CompanyName = "Company1"
                });
                string locationCode = "LOC1";
                context.Locations.Add(new Core.Entities.Location()
                {
                    Code = locationCode,
                    WHSCode = whscode,
                    AreaCode = "AC1",
                    Name = locationCode,
                    Status = 1,
                    Type = LocationType.Normal
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-5),
                    Status = StorageStatus.Putaway,
                    Ownership = Ownership.Supplier
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    LocationCode = locationCode,
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    Qty = 100,
                    OriginalQty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 0,
                    OutJobNo = "",
                    InboundDate = DateTime.Now.AddDays(-15),
                    Status = StorageStatus.Putaway,
                    Ownership = mixOwnership ? Ownership.EHP : Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = (byte)Ownership.Supplier
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    OnHandQty = 10000,
                    OnHandPkg = 10,
                    AllocatedQty = 5000,
                    AllocatedPkg = 5,
                    TransitQty = 100,
                    TransitPkg = 1,
                    QuarantineQty = 10,
                    QuarantinePkg = 1,
                    Ownership = Ownership.EHP
                });
                context.SaveChanges();
            }
            string firstJobNo;
            using (var context = new Context(options, appSettings.Object))
            {
                var outboundService = GetOutboundService(context);
                var result = await outboundService.ImportEKanbanEUCPart(refno, factoryID, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                firstJobNo = result.Data;
            }
            return firstJobNo;
        }

        private void AddTestData(Context context)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer", WHSCode = whscode });
            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status =  OutboundStatus.NewJob,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            {
                JobNo = jobno,
                LineItem = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 500,
                PickedQty = 500,
                Pkg = 4,
                PickedPkg = 4,
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
                Qty = 640,
                PickedQty = 640,
                Pkg = 1,
                PickedPkg = 1,
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
                Qty = 640,
                PickedQty = 640,
                Pkg = 1,
                PickedPkg = 1,
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

            context.SaveChanges();
        }

        private void AddTestDataSingle(Context context)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer", WHSCode = whscode });
            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status = OutboundStatus.NewJob,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            {
                JobNo = jobno,
                LineItem = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 500,
                PickedQty = 500,
                Pkg = 4,
                PickedPkg = 4,
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

            context.SaveChanges();
        }

        private void AddDeleteOutboundDetailTestData(Context context)
        {
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                OnHandQty = 1000,
                OnHandPkg = 10,
                AllocatedQty = 1300,
                AllocatedPkg = 5,
                TransitQty = 100,
                TransitPkg = 1,
                QuarantineQty = 10,
                QuarantinePkg = 1,
                Ownership = (byte)Ownership.Supplier
            });
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                OnHandQty = 1000,
                OnHandPkg = 10,
                AllocatedQty = 1300,
                AllocatedPkg = 5,
                TransitQty = 100,
                TransitPkg = 1,
                QuarantineQty = 10,
                QuarantinePkg = 1,
                Ownership = Ownership.EHP
            });
        }

        private void AddCancelOutboundData(Context context)
        {
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                OnHandQty = 1000,
                OnHandPkg = 10,
                AllocatedQty = 1300,
                AllocatedPkg = 5,
                TransitQty = 100,
                TransitPkg = 1,
                QuarantineQty = 10,
                QuarantinePkg = 1,
                Ownership = (byte)Ownership.Supplier
            });
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                Ownership = Ownership.EHP
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = refno,
                FactoryID = factoryID,
                IssuedDate = DateTime.Now.Date.AddDays(-50),
                CreatedDate = DateTime.Now.Date.AddDays(-50),
                OutJobNo = jobno,
                Status = (byte)EKanbanStatus.Imported
            });

            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SS" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 100,
                QuantitySupplied = 100,
                QuantityReceived = 0                
            });
            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SY" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 0,
                QuantitySupplied = 100,
                QuantityReceived = 0
            });

            context.EOrders.Add(new Core.Entities.EOrder
            {
                PartNo = "1",
                PurchaseOrderNo = refno,
                CardSerial = "1",
                OrderQuantity = "0"
            });
            context.EOrders.Add(new Core.Entities.EOrder
            {
                PartNo = "1",
                PurchaseOrderNo = refno,
                CardSerial = "2",
                OrderQuantity = "100"
            });

            context.LoadingDetails.Add(new Core.Entities.LoadingDetail { 
                OrderNo = refno,
                JobNo = jobno,
                ETD = DateTime.Now,
                OutJobNo = "",
                SupplierID = supplierID
            });

            context.Loadings.Add(new Core.Entities.Loading
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                ETD = DateTime.Now,
                WHSCode = whscode,
                Status = LoadingStatus.Completed
            });

            context.SaveChanges();
        }

        private void AddCancelAllocationData(Context context)
        {
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                OnHandQty = 1000,
                OnHandPkg = 10,
                AllocatedQty = 1000,
                AllocatedPkg = 5,
                TransitQty = 100,
                TransitPkg = 1,
                QuarantineQty = 10,
                QuarantinePkg = 1,
                Ownership = (byte)Ownership.Supplier
            });
            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                OnHandQty = 1000,
                OnHandPkg = 10,
                AllocatedQty = 1000,
                AllocatedPkg = 5,
                TransitQty = 100,
                TransitPkg = 1,
                QuarantineQty = 10,
                QuarantinePkg = 1,
                Ownership = Ownership.EHP
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = refno,
                FactoryID = factoryID,
                IssuedDate = DateTime.Now.Date.AddDays(-50),
                CreatedDate = DateTime.Now.Date.AddDays(-50),
                OutJobNo = jobno,
                Status = (byte)EKanbanStatus.Imported,
                Instructions = "EHP"
            });

            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SS" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 100,
                QuantitySupplied = 100,
                QuantityReceived = 0
            });
            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SY" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 150,
                QuantitySupplied = 150,
                QuantityReceived = 0
            });
            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SV" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 200,
                QuantitySupplied = 200,
                QuantityReceived = 0
            });
            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SX" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 50,
                QuantitySupplied = 50,
                QuantityReceived = 0
            });

            context.EOrders.Add(new Core.Entities.EOrder
            {
                PartNo = "1",
                PurchaseOrderNo = refno,
                CardSerial = "1",
                OrderQuantity = "0"
            });
            context.EOrders.Add(new Core.Entities.EOrder
            {
                PartNo = "1",
                PurchaseOrderNo = refno,
                CardSerial = "2",
                OrderQuantity = "100"
            });

            context.LoadingDetails.Add(new Core.Entities.LoadingDetail
            {
                OrderNo = refno,
                JobNo = jobno,
                ETD = DateTime.Now,
                OutJobNo = "",
                SupplierID = supplierID
            });

            context.Loadings.Add(new Core.Entities.Loading
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                ETD = DateTime.Now,
                WHSCode = whscode,
                Status = LoadingStatus.Completed
            });

            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 100,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                DropPoint = "ZZ99",
                //PickedBy = "USER1",
                //PickedDate = DateTime.Now.AddDays(-5),
                //PID = pid,
            });
            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 2,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 150,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                DropPoint = "ZZ99",
                //PickedBy = "USER2",
                //PickedDate = DateTime.Now.AddDays(-2),
                //PID = pid
            });
            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 3,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 200,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                DropPoint = "ZZ99",
                //PickedBy = "USER3",
                //PickedDate = DateTime.Now.AddDays(-1),
                //PID = pid
            });
            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 4,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 50,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                DropPoint = "ZZ99",
                //PickedBy = "USER4",
                //PickedDate = DateTime.Now.AddDays(-1),
                //PID = pid
            });

            context.PartMasters.Add(new Core.Entities.PartMaster()
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                Description = "HOS PASS C346 VEVA ADP LH ACTUAL",
                UOM = uom,
                PackageType = "PKG0002",
                SPQ = 144,
                CPartSPQ = 1,
                IsCPart = 0
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid,
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 1,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 100,
                Qty = 100,
                QtyPerPkg = 100,
                AllocatedQty = 100,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Allocated,
                Ownership = Ownership.EHP
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "2",
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 2,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 150,
                Qty = 150,
                QtyPerPkg = 150,
                AllocatedQty = 150,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Allocated,
                Ownership = (int)Ownership.Supplier
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "3",
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 3,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 200,
                Qty = 200,
                QtyPerPkg = 200,
                AllocatedQty = 200,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Allocated,
                Ownership = Ownership.EHP
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "4",
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 4,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 50,
                Qty = 50,
                QtyPerPkg = 50,
                AllocatedQty = 50,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Allocated,
                Ownership = Ownership.EHP
            });

            context.PickingListEKanbans.Add(new Core.Entities.PickingListEKanban()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 1,
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SS" + productCode,
            });
            context.PickingListEKanbans.Add(new Core.Entities.PickingListEKanban()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 2,
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SY" + productCode,
            });
            context.PickingListEKanbans.Add(new Core.Entities.PickingListEKanban()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 3,
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SV" + productCode,
            });
            context.PickingListEKanbans.Add(new Core.Entities.PickingListEKanban()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 4,
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SX" + productCode,
            });
            context.SaveChanges();
        }

        private void AddAutoAllocateTestData(Context context, decimal allocatedQty, decimal sdQty)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });

            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status = 0,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            /*
             * new { header.OutJobNo, header.FactoryID, detail.SupplierID, detail.ProductCode, Ownership = Ownership.EHP } equals
               new { s.OutJobNo, FactoryID = s.CustomerCode, s.SupplierID, s.ProductCode, s.Ownership } into sds
*/
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid,
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 1,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 150,
                Qty = sdQty,
                QtyPerPkg = sdQty,
                AllocatedQty = allocatedQty,
                OutJobNo = jobno,
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.EHP
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "TESAP202009000EC7",
                InJobNo = injobno,
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
                PID = pid2,
                InJobNo = injobno,
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
                Qty = allocatedQty,
                PickedQty = 0,
                Pkg = 0,
                PickedPkg = 0,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            });

            //context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            //{
            //    JobNo = jobno,
            //    LineItem = 2,
            //    ProductCode = "132732024",
            //    SupplierID = supplierID,
            //    Qty = 640,
            //    PickedQty = 0,
            //    Pkg = 0,
            //    PickedPkg = 1,
            //    Status = 1,
            //    CreatedBy = "00013",
            //    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            //});

            //context.OutboundDetails.Add(new Core.Entities.OutboundDetail
            //{
            //    JobNo = jobno,
            //    LineItem = 3,
            //    ProductCode = "132732025",
            //    SupplierID = "504163",
            //    Qty = 640,
            //    PickedQty = 640,
            //    Pkg = 1,
            //    PickedPkg = 1,
            //    Status = 1,
            //    CreatedBy = "00013",
            //    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            //});

            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = allocatedQty,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                PickedBy = "USER1",
                PickedDate = DateTime.Now.AddDays(-5),
                PID = pid
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
                UOM = uom,
                PackageType = "PKG0002",
                SPQ = 144,
                CPartSPQ = 1,
                IsCPart = 0
            });

            context.Inbounds.Add(new Core.Entities.Inbound()
            {
                JobNo = injobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = "ELPS-HT2020072502-507188",
                Status = InboundStatus.Completed
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

            context.UOMs.Add(new Core.Entities.UOM()
            {
                Code = uom,
                Name = uom,
                Status = 1
            });

            context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
            {
                UOM = uom,
                CustomerCode = factoryID,
                Status = 1,
                DecimalNum = 3
            });

            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                Ownership = Ownership.EHP,
                OnHandQty = 1000,
                AllocatedQty = 1000,
                OnHandPkg = 5,
                AllocatedPkg = 5
            });

            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                Ownership = (int)Ownership.Supplier,
                OnHandQty = 2000,
                AllocatedQty = 2000,             
                OnHandPkg = 10,
                AllocatedPkg = 10
            });


            context.InvTransactions.Add(new Core.Entities.InvTransaction
            {
                JobNo = jobno.Substring(0, jobno.Length - 1) + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                Qty = 10,
                Pkg = 1,
                Act = (int)InventoryTransactionType.Outbound,
                BalancePkg = 1000,
                BalanceQty = 5000
            });

            context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS
            {
                JobNo = jobno.Substring(0, jobno.Length -1) + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                WHSCode = whscode,
                Qty = 10,
                Pkg = 1,
                Act = (int)InventoryTransactionType.Outbound,
                BalancePkg = 1000,
                BalanceQty = 5000
            });

            context.InvTransactionsPerSupplier.Add(new Core.Entities.InvTransactionPerSupplier
            {
                JobNo = jobno.Substring(0, jobno.Length - 1) + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                SupplierID = supplierID,
                Ownership = Ownership.EHP,
                Qty = 10,
                Act = (int)InventoryTransactionType.Outbound,
                BalanceQty = 5000
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = refno,
                FactoryID = factoryID,
                IssuedDate = DateTime.Now.Date.AddDays(-50),
                CreatedDate = DateTime.Now.Date.AddDays(-50),
                OutJobNo = jobno,
                Status = (byte)EKanbanStatus.New
            });

            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail 
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SS"+productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 100,
                QuantitySupplied = 100,
                QuantityReceived = 0
            });

            context.SupplierItemMasters.Add(new Core.Entities.SupplierItemMaster
            {
                FactoryID = factoryID,
                SupplierID = supplierID,
                ProductCode = productCode,
                PastCost = 100,
                CurrentCost = 200,
                FutureCost = 300,
                PastCostCurrency = "EUR1",
                CurrentCostCurrency = "EUR2",
                FutureCostCurrency = "EUR3",
                PastCostEffectiveDate = DateTime.Now.Date.AddDays(-100),
                CurrentCostEffectiveDate = DateTime.Now.Date,
                FutureCostEffectiveDate = DateTime.Now.Date.AddDays(100),

                DTL = "X",
                SupplierPartNo = "Y",
                TargetMinStockQtyStatus = "Z"
            });

            context.SaveChanges();
        }

        private void AddCargoInTransitTestData(Context context, decimal allocatedQty, decimal sdQty)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });

            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status = 0,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid,
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 1,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 150,
                Qty = sdQty,
                QtyPerPkg = sdQty,
                AllocatedQty = allocatedQty,
                OutJobNo = jobno,
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.EHP
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "TESAP202009000EC7",
                InJobNo = injobno,
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
                PID = pid2,
                InJobNo = injobno,
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
                Qty = allocatedQty,
                PickedQty = allocatedQty,
                Pkg = 1,
                PickedPkg = 1,
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
                Qty = 640,
                PickedQty = 0,
                Pkg = 0,
                PickedPkg = 1,
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
                Qty = 640,
                PickedQty = 640,
                Pkg = 1,
                PickedPkg = 1,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            });

            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = jobno,
                LineItem = 1,
                SeqNo = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = allocatedQty,
                WHSCode = whscode,
                LocationCode = "P4",
                InboundDate = DateTime.Now.AddDays(-100),
                PickedBy = "USER1",
                PickedDate = DateTime.Now.AddDays(-5),
                PID = pid
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
                UOM = uom,
                PackageType = "PKG0002",
                SPQ = 144,
                CPartSPQ = 1,
                IsCPart = 0
            });

            context.Inbounds.Add(new Core.Entities.Inbound()
            {
                JobNo = injobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = "ELPS-HT2020072502-507188",
                Status = InboundStatus.Completed
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

            context.UOMs.Add(new Core.Entities.UOM()
            {
                Code = uom,
                Name = uom,
                Status = 1
            });

            context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
            {
                UOM = uom,
                CustomerCode = factoryID,
                Status = 1,
                DecimalNum = 3
            });

            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                Ownership = Ownership.EHP,
                OnHandQty = 1000,
                AllocatedQty = 1000,
                OnHandPkg = 5,
                AllocatedPkg = 5
            });

            context.Inventory.Add(new Core.Entities.Inventory
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = productCode,
                WHSCode = whscode,
                Ownership = (int)Ownership.Supplier,
                OnHandQty = 2000,
                AllocatedQty = 2000,
                OnHandPkg = 10,
                AllocatedPkg = 10
            });

            context.InvTransactions.Add(new Core.Entities.InvTransaction
            {
                JobNo = jobno[0..^1] + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                Qty = 10,
                Pkg = 1,
                Act = (int)InventoryTransactionType.Outbound,
                BalancePkg = 100,
                BalanceQty = 5000
            });

            context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS
            {
                JobNo = jobno.Substring(0, jobno.Length - 1) + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                WHSCode = whscode,
                Qty = 10,
                Pkg = 1,
                Act = (int)InventoryTransactionType.Outbound,
                BalancePkg = 100,
                BalanceQty = 5000
            });

            context.InvTransactionsPerSupplier.Add(new Core.Entities.InvTransactionPerSupplier
            {
                JobNo = jobno.Substring(0, jobno.Length - 1) + "1",
                JobDate = DateTime.Now.AddDays(-2),
                CustomerCode = factoryID,
                ProductCode = productCode,
                SupplierID = supplierID,
                Ownership = Ownership.EHP,
                Qty = 10,
                Act = (int)InventoryTransactionType.Outbound,
                BalanceQty = 5000
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = refno,
                FactoryID = factoryID,
                IssuedDate = DateTime.Now.Date.AddDays(-50),
                CreatedDate = DateTime.Now.Date.AddDays(-50),
                OutJobNo = jobno,
                Status = (byte)EKanbanStatus.New
            });

            context.EKanbanDetails.Add(new Core.Entities.EKanbanDetail
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "SS" + productCode,
                SupplierID = supplierID,
                DropPoint = "ZZ99",
                Quantity = 100,
                QuantitySupplied = 100,
                QuantityReceived = 100
            });
            context.SaveChanges();
        }


        private void AddDataForBonded()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Outbound, Prefix = "OUT", Name = "Outbound", Status = 1 });
                context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });
                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = refno,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType = OutboundType.EKanban,
                    Remark = "Remark",
                    Status = 0,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });

                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid,
                    InJobNo = injobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ParentID = "",
                    ProductCode = productCode,
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    InboundDate = DateTime.Now.Date.AddDays(-30),
                    OriginalQty = 150,
                    Qty = 100,
                    QtyPerPkg = 100,
                    AllocatedQty = 100,
                    OutJobNo = jobno,
                    WHSCode = whscode,
                    LocationCode = "L4",
                    Status = StorageStatus.Allocated,
                    BondedStatus = (int)BondedStatus.Bonded,
                    Ownership = Ownership.EHP
                });
                context.StorageDetails.Add(new Core.Entities.StorageDetail()
                {
                    PID = pid2,
                    InJobNo = injobno,
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
                    PID = pid3,
                    InJobNo = injobno,
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
                    Qty = 100,
                    PickedQty = 100,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 640,
                    PickedQty = 0,
                    Pkg = 0,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = jobno,
                    LineItem = 3,
                    ProductCode = "132732025",
                    SupplierID = "504163",
                    Qty = 640,
                    PickedQty = 640,
                    Pkg = 1,
                    PickedPkg = 1,
                    Status = (int)OutboundDetailStatus.Picked,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                });

                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 1,
                    SeqNo = 1,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 2,
                    SeqNo = 1,
                    ProductCode = "132732024",
                    SupplierID = supplierID,
                    Qty = 100,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid2
                });
                context.PickingLists.Add(new Core.Entities.PickingList()
                {
                    JobNo = jobno,
                    LineItem = 3,
                    SeqNo = 1,
                    ProductCode = "132732025",
                    SupplierID = supplierID,
                    Qty = 50,
                    WHSCode = whscode,
                    LocationCode = "P4",
                    InboundDate = DateTime.Now.AddDays(-100),
                    PickedBy = "USER1",
                    PickedDate = DateTime.Now.AddDays(-5),
                    PID = pid3
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
                    UOM = uom,
                    PackageType = "PKG0002",
                    SPQ = 144,
                    CPartSPQ = 1,
                    IsCPart = 0
                });
                context.Inbounds.Add(new Core.Entities.Inbound()
                {
                    JobNo = injobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = "ELPS-HT2020072502-507188",
                    Status = InboundStatus.Completed
                });
                context.SaveChanges();
            }
        }


        private readonly string jobno = "OUT20200900626";
        private readonly string productCode = "PRODCODE1";
        private readonly string productCode2 = "PRODCODE2";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";
        private readonly string uom = "UOM0000";
        private readonly string pid = "TESAP202009000EC6";
        private readonly string pid2 = "TESAP202009000EC8";
        private readonly string pid3 = "TESAP202009000EC9";
        private readonly string whscode = "PL";
        private readonly string injobno = "INB20200900125";
        private readonly string refno = "9200909056";
    }
}
