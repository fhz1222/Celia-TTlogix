using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Services;

namespace TT.Services.Tests
{
    [TestClass]
    public class InboundServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task CreateInboundManual()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).CreateInboundManual(new Models.InboundManualDto
                {
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    IRNo = "IRNo",
                    RefNo = "RefNo",
                    ETA = now.Date,
                    TransType = InboundType.ManualEntry,
                    Remark = "Remark",
                    SupplierID = supplierID                    
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual($"INB{now.Date.Year}{now.Date.Month:00}00001", inbound.JobNo);
                Assert.AreEqual(factoryID, inbound.CustomerCode);
                Assert.AreEqual(whscode, inbound.WHSCode);
                Assert.AreEqual("IRNo", inbound.IRNo);
                Assert.AreEqual("RefNo", inbound.RefNo);
                Assert.AreEqual(now.Date, inbound.ETA);
                Assert.AreEqual(InboundType.ManualEntry, inbound.TransType);
                Assert.AreEqual("Remark", inbound.Remark);
                Assert.AreEqual(InboundStatus.NewJob, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.CreatedBy);
                Assert.IsTrue(now < inbound.CreatedDate);
                Assert.AreEqual("", inbound.RevisedBy);
                Assert.AreEqual(null, inbound.RevisedDate);
                Assert.AreEqual("", inbound.CancelledBy);
                Assert.AreEqual(null, inbound.CancelledDate);
                Assert.AreEqual("", inbound.PutawayBy);
                Assert.AreEqual(null, inbound.PutawayDate);
                Assert.AreEqual(supplierID, inbound.SupplierID);
                Assert.AreEqual("", inbound.Currency);
                Assert.AreEqual("", inbound.IM4No);
            }
        }

        [TestMethod]
        public async Task UpdateInbound()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).UpdateInbound(jobNo, new Models.InboundDto 
                {
                    JobNo = jobNo,
                    CustomerCode = "CustomerCodeChanged",
                    WHSCode = "WHSCodeChanged",
                    IRNo = "IRNoChanged",
                    RefNo = "RefNoChanged",
                    ETA = DateTime.Now.Date,
                    TransType = InboundType.Excess,
                    Charged = 0,
                    Remark = "RemarkChanged",
                    Status = InboundStatus.PartialDownload,
                    CreatedBy = "CreatedByChanged",
                    CreatedDate = DateTime.Now.Date,
                    RevisedBy = "RevisedByChanged",
                    RevisedDate = DateTime.Now.Date,
                    CancelledBy = "CancelledByChanged",
                    CancelledDate = DateTime.Now.Date,
                    PutawayBy = "PutawayByChanged",
                    PutawayDate = DateTime.Now.Date,
                    SupplierID = "SupplierIDChanged",
                    Currency = "EUR",
                    IM4No = "IM4NoChanged",
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual(jobNo, inbound.JobNo);
                Assert.AreEqual(factoryID, inbound.CustomerCode);
                Assert.AreEqual(whscode, inbound.WHSCode);
                Assert.AreEqual(asnNo, inbound.IRNo);
                Assert.AreEqual("RefNoChanged", inbound.RefNo);
                Assert.AreEqual(now.Date, inbound.ETA);
                Assert.AreEqual(InboundType.ManualEntry, inbound.TransType);
                Assert.AreEqual(1, inbound.Charged);
                Assert.AreEqual("RemarkChanged", inbound.Remark);
                Assert.AreEqual(InboundStatus.PartialPutaway, inbound.Status);
                Assert.AreEqual("CreatedBy", inbound.CreatedBy);
                Assert.AreEqual(now.Date.AddDays(-10), inbound.CreatedDate);
                Assert.AreEqual("USERCODE2", inbound.RevisedBy);
                Assert.AreNotEqual(now.Date.AddDays(-10), inbound.RevisedDate);
                Assert.AreEqual("CancelledBy", inbound.CancelledBy);
                Assert.AreEqual(now.Date.AddDays(-10), inbound.CancelledDate);
                Assert.AreEqual("PutawayBy", inbound.PutawayBy);
                Assert.AreEqual(now.Date.AddDays(-10), inbound.PutawayDate);
                Assert.AreEqual(supplierID, inbound.SupplierID);
                Assert.AreEqual("EUR", inbound.Currency);
                Assert.AreEqual("IM4NoChanged", inbound.IM4No);
            }
        }

        [TestMethod]
        public async Task UpdateInbound_Completed()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.Completed;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).UpdateInbound(jobNo, new Models.InboundDto
                {
                    JobNo = jobNo,
                    CustomerCode = "CustomerCodeChanged",
                    WHSCode = "WHSCodeChanged",
                    IRNo = "IRNoChanged",
                    RefNo = "RefNoChanged",
                    ETA = DateTime.Now.Date,
                    TransType = InboundType.Excess,
                    Charged = 0,
                    Remark = "RemarkChanged",
                    Status = InboundStatus.PartialDownload,
                    CreatedBy = "CreatedByChanged",
                    CreatedDate = DateTime.Now.Date,
                    RevisedBy = "RevisedByChanged",
                    RevisedDate = DateTime.Now.Date,
                    CancelledBy = "CancelledByChanged",
                    CancelledDate = DateTime.Now.Date,
                    PutawayBy = "PutawayByChanged",
                    PutawayDate = DateTime.Now.Date,
                    SupplierID = "SupplierIDChanged",
                    Currency = "EUR",
                    IM4No = "IM4NoChanged",
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual(asnNo, inbound.RefNo);
                Assert.AreEqual(now.Date.AddDays(-10), inbound.ETA);
                Assert.AreEqual("Remark", inbound.Remark);

                Assert.AreEqual("USERCODE2", inbound.RevisedBy);
                Assert.AreNotEqual(now.Date.AddDays(-10), inbound.RevisedDate);
                Assert.AreEqual("EUR", inbound.Currency);
                Assert.AreEqual("IM4NoChanged", inbound.IM4No);
            }
        }

        [TestMethod]
        public async Task ImportASN_IncorrectHeaderStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                var asnHeader = context.ASNHeaders.Single();
                asnHeader.Status = "REC";
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).ImportASN(asnNo, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNull(inbound);
            }
        }

        [TestMethod]
        public async Task ImportASN_IncorrectDetailStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                var asnDetail = context.ASNHeaders.First();
                asnDetail.Status = "REC";
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).ImportASN(asnNo, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNull(inbound);
            }
        }

        [TestMethod]
        public async Task ImportASN()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).ImportASN(asnNo, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual($"INB{now.Date.Year}{now.Date.Month:00}00001", inbound.JobNo);
                Assert.AreEqual(factoryID, inbound.CustomerCode);
                Assert.AreEqual(whscode, inbound.WHSCode);
                Assert.AreEqual("80665633-500027", inbound.IRNo);
                Assert.AreEqual("80665633-500027", inbound.RefNo);
                Assert.IsTrue(now < inbound.ETA);
                Assert.AreEqual(InboundType.ASN, inbound.TransType);
                Assert.AreEqual(InboundStatus.NewJob, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.CreatedBy);
                Assert.IsTrue(now < inbound.CreatedDate);
                Assert.AreEqual("", inbound.RevisedBy);
                Assert.AreEqual(null, inbound.RevisedDate);
                Assert.AreEqual("", inbound.CancelledBy);
                Assert.AreEqual(null, inbound.CancelledDate);
                Assert.AreEqual("", inbound.PutawayBy);
                Assert.AreEqual(null, inbound.PutawayDate);
                Assert.AreEqual(supplierID, inbound.SupplierID);
                Assert.AreEqual("", inbound.Currency);
               
                var inboundDetails = context.InboundDetails.OrderBy(i => i.LineItem).ToList();

                Assert.AreEqual($"INB{now.Date.Year}{now.Date.Month:00}00001", inboundDetails[0].JobNo);
                Assert.AreEqual($"INB{now.Date.Year}{now.Date.Month:00}00001", inboundDetails[1].JobNo);

                Assert.AreEqual(1, inboundDetails[0].LineItem);
                Assert.AreEqual(2, inboundDetails[1].LineItem);

                Assert.AreEqual(0, inboundDetails[0].PkgLineItem);
                Assert.AreEqual(0, inboundDetails[1].PkgLineItem);

                Assert.AreEqual("A00944301", inboundDetails[0].ProductCode);
                Assert.AreEqual("A00944301", inboundDetails[1].ProductCode);

                Assert.AreEqual(168, inboundDetails[0].ImportedQty);
                Assert.AreEqual(166, inboundDetails[1].ImportedQty);

                Assert.AreEqual(168, inboundDetails[0].Qty);
                Assert.AreEqual(166, inboundDetails[1].Qty);

                Assert.AreEqual(1, inboundDetails[0].NoOfPackage);
                Assert.AreEqual(1, inboundDetails[1].NoOfPackage);

                Assert.AreEqual(1, inboundDetails[0].NoOfLabel);
                Assert.AreEqual(1, inboundDetails[1].NoOfLabel);

                Assert.AreEqual(910, inboundDetails[0].Length);
                Assert.AreEqual(910, inboundDetails[1].Length);
                Assert.AreEqual(811, inboundDetails[0].Width);
                Assert.AreEqual(811, inboundDetails[1].Width);
                Assert.AreEqual(1212, inboundDetails[0].Height);
                Assert.AreEqual(1212, inboundDetails[1].Height);
                Assert.AreEqual(1.1M, inboundDetails[0].NetWeight);
                Assert.AreEqual(1.1M, inboundDetails[1].NetWeight);
                Assert.AreEqual(1.1M, inboundDetails[0].GrossWeight);
                Assert.AreEqual(1.1M, inboundDetails[1].GrossWeight);

                Assert.AreEqual("PKG0001", inboundDetails[0].PackageType);
                Assert.AreEqual("PKG0001", inboundDetails[1].PackageType);

                Assert.AreEqual("01", inboundDetails[0].ControlCode1);
                Assert.AreEqual("01", inboundDetails[1].ControlCode1);

                Assert.AreEqual("", inboundDetails[0].ControlCode3);
                Assert.AreEqual("", inboundDetails[1].ControlCode3);
                Assert.AreEqual("", inboundDetails[0].ControlCode4);
                Assert.AreEqual("", inboundDetails[1].ControlCode4);
                Assert.AreEqual("", inboundDetails[0].ControlCode5);
                Assert.AreEqual("", inboundDetails[1].ControlCode5);
                Assert.AreEqual("", inboundDetails[0].ControlCode6);
                Assert.AreEqual("", inboundDetails[1].ControlCode6);

                Assert.IsTrue(now < inboundDetails[0].ControlDate);
                Assert.IsTrue(now < inboundDetails[1].ControlDate);
                Assert.IsTrue(now < inboundDetails[0].CreatedDate);
                Assert.IsTrue(now < inboundDetails[1].CreatedDate);

                Assert.AreEqual("USERCODE2", inboundDetails[0].CreatedBy);
                Assert.AreEqual("USERCODE2", inboundDetails[1].CreatedBy);

                Assert.AreEqual("", inboundDetails[0].RevisedBy);
                Assert.AreEqual("", inboundDetails[1].RevisedBy);
                Assert.AreEqual(null, inboundDetails[0].RevisedDate);
                Assert.AreEqual(null, inboundDetails[1].RevisedDate);

                Assert.AreEqual(asnNo, inboundDetails[0].ASNNo);
                Assert.AreEqual(asnNo, inboundDetails[1].ASNNo);

                Assert.AreEqual(1, inboundDetails[0].ASNLineItem);
                Assert.AreEqual(2, inboundDetails[1].ASNLineItem);

                var pids = context.PIDCodes.OrderBy(i => i.PIDNo).ToList();
                Assert.AreEqual(2, pids.Count);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000001", pids[0].PIDNo);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000002", pids[1].PIDNo);

                var sd = context.StorageDetails.OrderBy(s => s.PID).ToList();
                Assert.AreEqual(2, sd.Count);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000001", sd[0].PID);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000002", sd[1].PID);
                Assert.AreEqual(inbound.JobNo, sd[0].InJobNo);
                Assert.AreEqual(inbound.JobNo, sd[1].InJobNo);
            }
        }

        [TestMethod]
        public async Task ImportASN_NullBatchNumber()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                foreach(var asnd in context.ASNDetails)
                {
                    asnd.BatchNo = null;
                }
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).ImportASN(asnNo, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inboundDetails = context.InboundDetails.OrderBy(i => i.LineItem).ToList();

                Assert.AreEqual("", inboundDetails[0].ControlCode1);
                Assert.AreEqual("", inboundDetails[1].ControlCode1);
            }
        }

        [TestMethod]
        public async Task CreateInboundDetail_CycleCheckNotPassed()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;

                context.CycleCounts.Add(new Core.Entities.CycleCount {
                    JobNo = "CCJobNo",
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "CreatedBy",
                    CreatedDate = DateTime.Now,
                    Status = CycleCountStatus.New
                });
                context.CycleCountDetails.Add(new Core.Entities.CycleCountDetail { 
                    JobNo = "CCJobNo",
                    LineItem = 1,
                    SeqNo = 1,
                    PID = "pid",
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    WHSCode = whscode,
                    LocationCode = "loccode",
                    Status = 0
                });
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).CreateInboundDetail(new Models.InboundDetailEntryAddDto
                {
                    JobNo = jobNo,
                    ProductCode = productCode,
                    ControlCode1 = "cc1",
                    ControlCode2 = "cc2",
                    ControlCode3 = "cc3",
                    ControlCode4 = "cc4",
                    ControlCode5 = "cc5",
                    ControlCode6 = "cc6",
                    GrossWeight = 120,
                    NetWeight = 100,
                    Height = 10,
                    Width = 20,
                    Length = 30,
                    ImportedQty = 500,
                    Qty = 600,
                    PackageType = "packageType",
                    PartMasterDecimalNum = 2,
                }, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task CreateInboundDetail()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).CreateInboundDetail(new Models.InboundDetailEntryAddDto {
                    JobNo = jobNo,
                    QtyPerPkg = 300,
                    Qty = 600,
                    ProductCode = productCode,
                    ControlCode1 = "cc1",
                    ControlCode2 = "cc2",
                    ControlCode3 = "cc3",
                    ControlCode4 = "cc4",
                    ControlCode5 = "cc5",
                    ControlCode6 = "cc6",
                    GrossWeight = 120,
                    NetWeight = 100,
                    Height = 10,
                    Width = 20,
                    Length = 30,
                    ImportedQty = 500,
                    PackageType = "packageType",
                    PartMasterDecimalNum = 2,
                }, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 2).SingleOrDefault();
                Assert.IsNotNull(inboundDetail);
                Assert.AreEqual("USERCODE2", inboundDetail.CreatedBy);
                Assert.AreEqual(jobNo, inboundDetail.JobNo);
                Assert.AreEqual(2, inboundDetail.LineItem);
                Assert.AreEqual(productCode, inboundDetail.ProductCode);
                Assert.AreEqual("cc1", inboundDetail.ControlCode1);
                Assert.AreEqual("cc2", inboundDetail.ControlCode2);
                Assert.AreEqual("cc3", inboundDetail.ControlCode3);
                Assert.AreEqual("cc4", inboundDetail.ControlCode4);
                Assert.AreEqual("cc5", inboundDetail.ControlCode5);
                Assert.AreEqual("cc6", inboundDetail.ControlCode6);
                Assert.AreEqual(120, inboundDetail.GrossWeight);
                Assert.AreEqual(100, inboundDetail.NetWeight);
                Assert.AreEqual(10, inboundDetail.Height);
                Assert.AreEqual(20, inboundDetail.Width);
                Assert.AreEqual(30, inboundDetail.Length);
                Assert.AreEqual(500, inboundDetail.ImportedQty);
                Assert.AreEqual(600, inboundDetail.Qty);
                Assert.AreEqual(2, inboundDetail.NoOfLabel);
                Assert.AreEqual(2, inboundDetail.NoOfPackage);
                Assert.AreEqual("packageType", inboundDetail.PackageType);
                Assert.IsTrue(now < inboundDetail.CreatedDate);
                Assert.IsTrue(now < inboundDetail.ControlDate);
                Assert.AreEqual("", inboundDetail.Remark);

                // storage details
                var sds = context.StorageDetails.Where(sd => sd.LineItem == 2).OrderBy(s => s.LineItem).ThenBy(s => s.SeqNo).ToList();
                Assert.AreEqual(2, sds.Count);
                var sd1 = sds[0];
                var sd2 = sds[1];
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000001", sd1.PID);
                Assert.AreEqual(2, sd1.LineItem);
                Assert.AreEqual(1, sd1.SeqNo);
                Assert.AreEqual(jobNo, sd1.InJobNo);
                Assert.AreEqual(factoryID, sd1.CustomerCode);
                Assert.AreEqual(productCode, sd1.ProductCode);
                Assert.AreEqual(context.Inbounds.First().ETA, sd1.InboundDate);
                Assert.AreEqual(300, sd1.Qty);
                Assert.AreEqual(300, sd1.OriginalQty);
                Assert.AreEqual(2, sd1.NoOfLabel);
                Assert.AreEqual(120, sd1.GrossWeight);
                Assert.AreEqual(100, sd1.NetWeight);
                Assert.AreEqual(10, sd1.Height);
                Assert.AreEqual(20, sd1.Width);
                Assert.AreEqual(30, sd1.Length);
                Assert.AreEqual("cc1", sd1.ControlCode1);
                Assert.AreEqual("cc2", sd1.ControlCode2);
                Assert.AreEqual("cc3", sd1.ControlCode3);
                Assert.AreEqual("cc4", sd1.ControlCode4);
                Assert.AreEqual("cc5", sd1.ControlCode5);
                Assert.AreEqual("cc6", sd1.ControlCode6);
                Assert.IsTrue(now < sd1.ControlDate);
                Assert.AreEqual(300, sd1.QtyPerPkg);
                Assert.AreEqual(whscode, sd1.WHSCode);
                Assert.AreEqual(StorageStatus.Incoming, sd1.Status);
                Assert.AreEqual(supplierID, sd1.SupplierID);
                Assert.AreEqual(0, sd1.IsVMI);
                Assert.AreEqual(0, sd1.BondedStatus);
                Assert.AreEqual(0, sd1.BuyingPrice);
                Assert.AreEqual(0, sd1.SellingPrice);

                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000002", sd2.PID);
                Assert.AreEqual(2, sd2.LineItem);
                Assert.AreEqual(2, sd2.SeqNo);
                Assert.AreEqual(jobNo, sd2.InJobNo);
                Assert.AreEqual(factoryID, sd2.CustomerCode);
                Assert.AreEqual(productCode, sd2.ProductCode);
                Assert.AreEqual(context.Inbounds.First().ETA, sd2.InboundDate);
                Assert.AreEqual(300, sd2.Qty);
                Assert.AreEqual(300, sd2.OriginalQty);
                Assert.AreEqual(2, sd2.NoOfLabel);
                Assert.AreEqual(120, sd2.GrossWeight);
                Assert.AreEqual(100, sd2.NetWeight);
                Assert.AreEqual(10, sd2.Height);
                Assert.AreEqual(20, sd2.Width);
                Assert.AreEqual(30, sd2.Length);
                Assert.AreEqual("cc1", sd2.ControlCode1);
                Assert.AreEqual("cc2", sd2.ControlCode2);
                Assert.AreEqual("cc3", sd2.ControlCode3);
                Assert.AreEqual("cc4", sd2.ControlCode4);
                Assert.AreEqual("cc5", sd2.ControlCode5);
                Assert.AreEqual("cc6", sd2.ControlCode6);
                Assert.IsTrue(now < sd2.ControlDate);
                Assert.AreEqual(300, sd2.QtyPerPkg);
                Assert.AreEqual(whscode, sd2.WHSCode);
                Assert.AreEqual(StorageStatus.Incoming, sd2.Status);
                Assert.AreEqual(supplierID, sd2.SupplierID);
                Assert.AreEqual(0, sd2.IsVMI);
                Assert.AreEqual(0, sd2.BondedStatus);
                Assert.AreEqual(0, sd2.BuyingPrice);
                Assert.AreEqual(0, sd2.SellingPrice);
            }
        }

        [TestMethod]
        public async Task UpdateInboundDetail_IncorrectQtyPerPackage()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).UpdateInboundDetail(new Models.InboundDetailEntryModifyDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    ProductCode = productCode,
                    ControlCode3 = productCode,
                    QtyPerPkg = 7,
                    PartMasterDecimalNum = 1,
                }, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var id = context.InboundDetails.Single();
                Assert.AreEqual(100, id.Qty);
                Assert.AreEqual(1, id.NoOfPackage);
                Assert.AreEqual(1, id.NoOfLabel);
            }
        }
     
        [TestMethod]
        public async Task UpdateInboundDetail()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).UpdateInboundDetail(new Models.InboundDetailEntryModifyDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    ProductCode = productCode,
                    ControlCode3 = productCode,
                    QtyPerPkg = 50,
                    PartMasterDecimalNum = 2,
                }, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var id = context.InboundDetails.Single();
                Assert.AreEqual(jobNo, id.JobNo);
                Assert.AreEqual(1, id.LineItem);
                Assert.AreEqual(productCode, id.ProductCode);
                Assert.AreEqual(100, id.Qty);
                Assert.AreEqual(2, id.NoOfPackage);
                Assert.AreEqual(2, id.NoOfLabel);

                var storageDetails = context.StorageDetails.ToList();
                Assert.AreEqual(3, storageDetails.Count);
                var sdCancelled = storageDetails.Where(s => s.Status == StorageStatus.Cancelled).SingleOrDefault();
                Assert.AreEqual(0, sdCancelled.Qty);
                var sds = storageDetails.Where(s => s.Status == StorageStatus.Incoming).OrderBy(s => s.SeqNo).ToList();
                Assert.AreEqual(2, sds.Count);

                var sd1 = sds[0];
                var sd2 = sds[1];
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000001", sd1.PID);
                Assert.AreEqual(1, sd1.LineItem);
                Assert.AreEqual(1, sd1.SeqNo);
                Assert.AreEqual(jobNo, sd1.InJobNo);
                Assert.AreEqual(factoryID, sd1.CustomerCode);
                Assert.AreEqual(productCode, sd1.ProductCode);
                Assert.AreEqual(context.Inbounds.First().ETA, sd1.InboundDate);
                Assert.AreEqual(50, sd1.Qty);
                Assert.AreEqual(50, sd1.OriginalQty);
                Assert.AreEqual(2, sd1.NoOfLabel);
                Assert.AreEqual(1120, sd1.GrossWeight);
                Assert.AreEqual(1100, sd1.NetWeight);
                Assert.AreEqual(110, sd1.Height);
                Assert.AreEqual(120, sd1.Width);
                Assert.AreEqual(130, sd1.Length);
                Assert.AreEqual("cc1", sd1.ControlCode1);
                Assert.AreEqual("cc2", sd1.ControlCode2);
                Assert.AreEqual("cc3", sd1.ControlCode3);
                Assert.AreEqual("cc4", sd1.ControlCode4);
                Assert.AreEqual("cc5", sd1.ControlCode5);
                Assert.AreEqual("cc6", sd1.ControlCode6);
                Assert.AreEqual(50, sd1.QtyPerPkg);
                Assert.AreEqual(whscode, sd1.WHSCode);
                Assert.AreEqual(StorageStatus.Incoming, sd1.Status);
                Assert.AreEqual(supplierID, sd1.SupplierID);
                Assert.AreEqual(0, sd1.IsVMI);
                Assert.AreEqual(0, sd1.BondedStatus);
                Assert.AreEqual(0, sd1.BuyingPrice);
                Assert.AreEqual(0, sd1.SellingPrice);

                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000002", sd2.PID);
                Assert.AreEqual(1, sd2.LineItem);
                Assert.AreEqual(2, sd2.SeqNo);
                Assert.AreEqual(jobNo, sd2.InJobNo);
                Assert.AreEqual(factoryID, sd2.CustomerCode);
                Assert.AreEqual(productCode, sd2.ProductCode);
                Assert.AreEqual(context.Inbounds.First().ETA, sd2.InboundDate);
                Assert.AreEqual(50, sd2.Qty);
                Assert.AreEqual(50, sd2.OriginalQty);
                Assert.AreEqual(2, sd2.NoOfLabel);
                Assert.AreEqual(1120, sd2.GrossWeight);
                Assert.AreEqual(1100, sd2.NetWeight);
                Assert.AreEqual(110, sd2.Height);
                Assert.AreEqual(120, sd2.Width);
                Assert.AreEqual(130, sd2.Length);
                Assert.AreEqual("cc1", sd2.ControlCode1);
                Assert.AreEqual("cc2", sd2.ControlCode2);
                Assert.AreEqual("cc3", sd2.ControlCode3);
                Assert.AreEqual("cc4", sd2.ControlCode4);
                Assert.AreEqual("cc5", sd2.ControlCode5);
                Assert.AreEqual("cc6", sd2.ControlCode6);
                Assert.AreEqual(50, sd2.QtyPerPkg);
                Assert.AreEqual(whscode, sd2.WHSCode);
                Assert.AreEqual(StorageStatus.Incoming, sd2.Status);
                Assert.AreEqual(supplierID, sd2.SupplierID);
                Assert.AreEqual(0, sd2.IsVMI);
                Assert.AreEqual(0, sd2.BondedStatus);
                Assert.AreEqual(0, sd2.BuyingPrice);
                Assert.AreEqual(0, sd2.SellingPrice);
            }
        }

        [TestMethod]
        public async Task CancelInbound_Manual()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.TransType = InboundType.ManualEntry;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).CancelInbound(jobNo, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var storageDetails = context.StorageDetails.SingleOrDefault();
                Assert.IsNotNull(storageDetails);
                Assert.AreEqual(StorageStatus.Cancelled, storageDetails.Status);
                Assert.AreEqual(0, storageDetails.Qty);

                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual(InboundStatus.Cancelled, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.CancelledBy);
                Assert.IsTrue(now < inbound.CancelledDate);
            }
        }

        [TestMethod]
        public async Task CancelInbound_ASN()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.Single();
                inbound.Status = InboundStatus.NewJob;
                inbound.TransType = InboundType.ASN;
                inbound.CancelledBy = "";
                inbound.CancelledDate = null;
                var asnDetails = context.ASNDetails.FirstOrDefault();
                asnDetails.InJobNo = jobNo;
                asnDetails.Status = "IMP";
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).CancelInbound(jobNo, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var storageDetails = context.StorageDetails.SingleOrDefault();
                Assert.IsNotNull(storageDetails);
                Assert.AreEqual(StorageStatus.Cancelled, storageDetails.Status);
                Assert.AreEqual(0, storageDetails.Qty);

                var inbound = context.Inbounds.SingleOrDefault();
                Assert.IsNotNull(inbound);
                Assert.AreEqual(InboundStatus.Cancelled, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.CancelledBy);
                Assert.IsTrue(now < inbound.CancelledDate);

                var asnHeader = context.ASNHeaders.SingleOrDefault();
                Assert.AreEqual("NEW", asnHeader.Status);
                var asnDetail = context.ASNDetails.ToList();
                Assert.IsTrue( asnDetail.All(i => i.InJobNo == ""));
                Assert.IsTrue(asnDetail.All(i => i.Status == "NEW"));
            }
        }
     
        [TestMethod]
        public async Task IncreasePkgQty_InvalidQty()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).IncreasePkgQty(jobNo, 1, 50, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task IncreasePkgQty()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.AreEqual(300, inboundDetail.Qty);
                Assert.AreEqual(3, inboundDetail.NoOfLabel);
                Assert.AreEqual(3, inboundDetail.NoOfPackage);
                Assert.AreEqual("USERCODE2", inboundDetail.RevisedBy);
                Assert.IsTrue(now < inboundDetail.RevisedDate);

                var storageDetail = context.StorageDetails.Where(i => i.InJobNo == jobNo && i.LineItem == 1).OrderBy(i => i.SeqNo).ToList();
                Assert.AreEqual(1, storageDetail[0].SeqNo);
                Assert.AreEqual(2, storageDetail[1].SeqNo);
                Assert.AreEqual(3, storageDetail[2].SeqNo);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000001", storageDetail[1].PID);
                Assert.AreEqual($"TESAP{now.Date.Year}{now.Date.Month:00}000002", storageDetail[2].PID);

                Assert.AreEqual(jobNo, storageDetail[1].InJobNo);
                Assert.AreEqual(jobNo, storageDetail[2].InJobNo);
                Assert.AreEqual(100, storageDetail[1].Qty);
                Assert.AreEqual(100, storageDetail[2].Qty);
                Assert.AreEqual(100, storageDetail[1].QtyPerPkg);
                Assert.AreEqual(100, storageDetail[2].QtyPerPkg);

                Assert.AreEqual(3, storageDetail[1].NoOfLabel);
                Assert.AreEqual(3, storageDetail[2].NoOfLabel);

                Assert.AreEqual(100, storageDetail[1].BuyingPrice);
                Assert.AreEqual(120, storageDetail[1].SellingPrice);
            }
        }
   
        [TestMethod]
        public async Task RemovePIDs_RemoveOnePID_NothingPutAway()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.First();
                inbound.Status = InboundStatus.NewJob;
                context.SaveChanges();
                await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).RemovePIDs(new Models.RemovePIDsDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    PIDs = new string[] { "pid1" },
                    RemoveAll = false
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var sd = context.StorageDetails.Where(s => s.Status == StorageStatus.Cancelled && s.Qty == 0 && s.PID == "pid1").SingleOrDefault();
                Assert.IsNotNull(sd);
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.IsNotNull(inboundDetail);
                Assert.AreEqual(2, inboundDetail.NoOfLabel);
                Assert.AreEqual(2, inboundDetail.NoOfPackage);
                Assert.AreEqual(200, inboundDetail.Qty);
                Assert.AreEqual("USERCODE2", inboundDetail.RevisedBy);
                Assert.IsTrue(now < inboundDetail.RevisedDate);

                var sdUpdated = context.StorageDetails.Where(i => i.InJobNo == jobNo && i.LineItem == 1).OrderBy(s => s.SeqNo).ToList();
                Assert.AreEqual(3, sdUpdated.Count);
                Assert.AreEqual(StorageStatus.Cancelled, sdUpdated[0].Status);
                Assert.AreEqual(0, sdUpdated[0].Qty);
                Assert.AreEqual(2, sdUpdated[1].NoOfLabel);
                Assert.AreEqual(2, sdUpdated[2].NoOfLabel);

                //CompleteInboud - no status change on inbound
                var inbound = context.Inbounds.First();
                Assert.AreEqual(InboundStatus.NewJob, inbound.Status);

                // no transactions created
                Assert.AreEqual(0, context.InvTransactions.Count());
                Assert.AreEqual(0, context.InvTransactionsPerSupplier.Count());
                Assert.AreEqual(0, context.InvTransactionsPerWHS.Count());
            }
        }

        [TestMethod]
        public async Task RemovePIDs_RemoveOnePID_OneItemPutAway()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.First();
                inbound.Status = InboundStatus.PartialPutaway;
                context.SaveChanges();
                await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");
                var storageDetail = context.StorageDetails.Where(s => s.PID != "pid1").First();
                storageDetail.Status = StorageStatus.Putaway;
                storageDetail.LocationCode = "locCode";
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).RemovePIDs(new Models.RemovePIDsDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    PIDs = new string[] { "pid1" },
                    RemoveAll = false
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var sd = context.StorageDetails.Where(s => s.Status == StorageStatus.Cancelled && s.Qty == 0 && s.PID == "pid1").SingleOrDefault();
                Assert.IsNotNull(sd);
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.IsNotNull(inboundDetail);
                Assert.AreEqual(2, inboundDetail.NoOfLabel);
                Assert.AreEqual(2, inboundDetail.NoOfPackage);
                Assert.AreEqual(200, inboundDetail.Qty);
                Assert.AreEqual("USERCODE2", inboundDetail.RevisedBy);
                Assert.IsTrue(now < inboundDetail.RevisedDate);

                var sdUpdated = context.StorageDetails.Where(i => i.InJobNo == jobNo && i.LineItem == 1).OrderBy(s => s.SeqNo).ToList();
                Assert.AreEqual(3, sdUpdated.Count);
                Assert.AreEqual(StorageStatus.Cancelled, sdUpdated[0].Status);
                Assert.AreEqual(0, sdUpdated[0].Qty);
                Assert.AreEqual(2, sdUpdated[1].NoOfLabel);
                Assert.AreEqual(2, sdUpdated[2].NoOfLabel);

                //CompleteInboud -  status change on inbound
                var inbound = context.Inbounds.First();
                Assert.AreEqual(InboundStatus.PartialPutaway, inbound.Status);

                // no transactions created
                Assert.AreEqual(0, context.InvTransactions.Count());
                Assert.AreEqual(0, context.InvTransactionsPerSupplier.Count());
                Assert.AreEqual(0, context.InvTransactionsPerWHS.Count());
            }
        }

        [TestMethod]
        public async Task RemovePIDs_RemoveOnePID_AllItemsPutAway()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.First();
                inbound.TransType = InboundType.ASN;
                inbound.Status = InboundStatus.PartialPutaway;
                foreach (var asn in context.ASNDetails.ToList())
                    asn.InJobNo = jobNo;

                context.SaveChanges();

                await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");

                foreach (var storageDetail in context.StorageDetails)
                {
                    storageDetail.Status = StorageStatus.Putaway;
                    storageDetail.LocationCode = "locCode";
                }
                // add inventory
                context.Inventory.Add(new Core.Entities.Inventory { 
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                     ProductCode1 = productCode,
                     WHSCode = whscode, 
                     Ownership = Ownership.Supplier,
                     OnHandPkg = 300,
                     OnHandQty = 30000,
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).RemovePIDs(new Models.RemovePIDsDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    PIDs = new string[] { "pid1" },
                    RemoveAll = false
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var sd = context.StorageDetails.Where(s => s.Status == StorageStatus.Cancelled && s.Qty == 0 && s.PID == "pid1").SingleOrDefault();
                Assert.IsNotNull(sd);
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.IsNotNull(inboundDetail);
                Assert.AreEqual(2, inboundDetail.NoOfLabel);
                Assert.AreEqual(2, inboundDetail.NoOfPackage);
                Assert.AreEqual(200, inboundDetail.Qty);
                Assert.AreEqual("USERCODE2", inboundDetail.RevisedBy);
                Assert.IsTrue(now < inboundDetail.RevisedDate);

                var sdUpdated = context.StorageDetails.Where(i => i.InJobNo == jobNo && i.LineItem == 1).OrderBy(s => s.SeqNo).ToList();
                Assert.AreEqual(3, sdUpdated.Count);
                Assert.AreEqual(StorageStatus.Cancelled, sdUpdated[0].Status);
                Assert.AreEqual(0, sdUpdated[0].Qty);
                Assert.AreEqual(2, sdUpdated[1].NoOfLabel);
                Assert.AreEqual(2, sdUpdated[2].NoOfLabel);
               
                Assert.AreEqual(StorageStatus.Putaway, sdUpdated[1].Status);
                Assert.AreEqual(StorageStatus.Putaway, sdUpdated[2].Status);

                var inbound = context.Inbounds.First();
                Assert.AreEqual(InboundStatus.Completed, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.PutawayBy);
                Assert.IsTrue(now < inbound.PutawayDate);

                var inventory = context.Inventory.Single();
                Assert.AreEqual(302, inventory.OnHandPkg);
                Assert.AreEqual(30200, inventory.OnHandQty);

                // transactions are added
                Assert.AreEqual(1, context.InvTransactions.Count());
                Assert.AreEqual(1, context.InvTransactionsPerSupplier.Count());
                Assert.AreEqual(1, context.InvTransactionsPerWHS.Count());

                // ASN status set
                Assert.IsTrue(context.ASNDetails.All(a => a.Status == "REC"));
                Assert.IsTrue(context.ASNHeaders.All(a => a.Status == "REC"));
            }
        }

        [TestMethod]
        public async Task RemovePIDs_RemoveOnePID_AllItemsPutAway_StockQuarantined()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                var inbound = context.Inbounds.First();
                inbound.TransType = InboundType.ASN;
                inbound.Status = InboundStatus.PartialPutaway;

                context.JobCodes.Add(new Core.Entities.JobCode
                {
                    Code = 21,
                    Prefix = "QRT",
                    Status = 1,
                    CreatedBy = "CreatedBy",
                    CreatedDate = DateTime.Now,
                    Name = "Quarantine"
                });

                context.Locations.Add(new Core.Entities.Location {
                    Code = "QNT",
                    Type = LocationType.Quarantine,
                    WHSCode = whscode,
                    AreaCode = "Rack",
                    Name = "Quarantine",
                    M3 = 0.1m,
                    Status = 1,
                    CreatedBy = "CreatedBy",
                    CreatedDate = DateTime.Now,
                });

                foreach (var asn in context.ASNDetails.ToList())
                    asn.InJobNo = jobNo;

                context.SaveChanges();

                await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");

                foreach (var storageDetail in context.StorageDetails)
                {
                    storageDetail.Status = StorageStatus.Putaway;
                    if (storageDetail.PID != "pid1")
                        storageDetail.LocationCode = "QNT";
                }
                // add inventory
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandPkg = 300,
                    OnHandQty = 30000,
                });
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandPkg = 600,
                    OnHandQty = 60000,
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).RemovePIDs(new Models.RemovePIDsDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    PIDs = new string[] { "pid1" },
                    RemoveAll = false
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var sd = context.StorageDetails.Where(s => s.Status == StorageStatus.Cancelled && s.Qty == 0 && s.PID == "pid1").SingleOrDefault();
                Assert.IsNotNull(sd);
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.IsNotNull(inboundDetail);
                Assert.AreEqual(2, inboundDetail.NoOfLabel);
                Assert.AreEqual(2, inboundDetail.NoOfPackage);
                Assert.AreEqual(200, inboundDetail.Qty);
                Assert.AreEqual("USERCODE2", inboundDetail.RevisedBy);
                Assert.IsTrue(now < inboundDetail.RevisedDate);

                var sdUpdated = context.StorageDetails.Where(i => i.InJobNo == jobNo && i.LineItem == 1).OrderBy(s => s.SeqNo).ToList();
                Assert.AreEqual(3, sdUpdated.Count);
                Assert.AreEqual(StorageStatus.Cancelled, sdUpdated[0].Status);
                Assert.AreEqual(0, sdUpdated[0].Qty);
                Assert.AreEqual(2, sdUpdated[1].NoOfLabel);
                Assert.AreEqual(2, sdUpdated[2].NoOfLabel);

                Assert.AreEqual(StorageStatus.Quarantine, sdUpdated[1].Status);
                Assert.AreEqual(StorageStatus.Quarantine, sdUpdated[2].Status);

                var inbound = context.Inbounds.First();
                Assert.AreEqual(InboundStatus.Completed, inbound.Status);
                Assert.AreEqual("USERCODE2", inbound.PutawayBy);
                Assert.IsTrue(now < inbound.PutawayDate);

                var inventoryS = context.Inventory.Where(i => i.Ownership == Ownership.Supplier).Single();
                Assert.AreEqual(302, inventoryS.OnHandPkg);
                Assert.AreEqual(30200, inventoryS.OnHandQty);

                var inventoryEHP = context.Inventory.Where(i => i.Ownership == Ownership.EHP).Single();
                Assert.AreEqual(600, inventoryEHP.OnHandPkg);
                Assert.AreEqual(60000, inventoryEHP.OnHandQty);
                Assert.AreEqual(2, inventoryEHP.QuarantinePkg);
                Assert.AreEqual(200, inventoryEHP.QuarantineQty);
                // transactions are added
                Assert.AreEqual(1, context.InvTransactions.Count());
                Assert.AreEqual(1, context.InvTransactionsPerSupplier.Count());
                Assert.AreEqual(1, context.InvTransactionsPerWHS.Count());

                var quarantineLog = context.QuarantineLogs.OrderBy(q => q.JobNo).ThenBy(q => q.LineItem).ToList();
                Assert.AreEqual(2, quarantineLog.Count);
                Assert.AreEqual($"QRT{now.Date.Year}{now.Date.Month:00}00001", quarantineLog[0].JobNo);
                Assert.AreEqual($"QRT{now.Date.Year}{now.Date.Month:00}00001", quarantineLog[1].JobNo);
                Assert.AreEqual(1, quarantineLog[0].LineItem);
                Assert.AreEqual(2, quarantineLog[1].LineItem);
                Assert.AreEqual(sdUpdated[1].PID, quarantineLog[0].PID);
                Assert.AreEqual(sdUpdated[2].PID, quarantineLog[1].PID);
                Assert.AreEqual((int)QuarantineType.Onhold, quarantineLog[0].Act);
                Assert.AreEqual((int)QuarantineType.Onhold, quarantineLog[1].Act);
                Assert.AreEqual("USERCODE2", quarantineLog[0].CreatedBy);
                Assert.AreEqual("USERCODE2", quarantineLog[1].CreatedBy);
                Assert.IsTrue(now < quarantineLog[0].CreatedDate);
                Assert.IsTrue(now < quarantineLog[1].CreatedDate);

                // ASN status set
                Assert.IsTrue(context.ASNDetails.All(a => a.Status == "REC"));
                Assert.IsTrue(context.ASNHeaders.All(a => a.Status == "REC"));
            }
        }

        [TestMethod]
        public async Task RemovePIDs_RemoveAll_NoItemsPutAway()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddASNTestData(context);
                AddManualInboundTestData(context);
                await GetInboundService(context).IncreasePkgQty(jobNo, 1, 200, "USERCODE2");
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetInboundService(context).RemovePIDs(new Models.RemovePIDsDto
                {
                    JobNo = jobNo,
                    LineItem = 1,
                    RemoveAll = true
                }, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.IsTrue(context.StorageDetails.All(s => s.Status == StorageStatus.Cancelled && s.Qty == 0));
                var inboundDetail = context.InboundDetails.Where(i => i.JobNo == jobNo && i.LineItem == 1).FirstOrDefault();
                Assert.IsNull(inboundDetail);
            }
        }

        private IInboundService GetInboundService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var utilityService = new UtilityService(repository, appSettings.Object);
            var reportService = new Mock<IReportService>().Object;
            var ilogConnect = new Mock<IILogConnect>().Object;

            var billingManager = new BillingService(repository);
            var xlsService = new Mock<IXlsService>().Object;
            return new InboundService(repository, locker, new Logger<InboundService>(loggerFactory), mapper, utilityService, appSettings.Object, reportService, billingManager, xlsService, ilogConnect);
        }
    
        private void AddASNTestData(Context context)
        {
            context.Users.Add(new Core.Entities.User
            {
                Code = "CreatedBy",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "1234",
                GroupCode = "G",
                WHSCode = whscode,
                Status = ValueStatus.Active               
            });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer", WHSCode = whscode });

            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Inbound, Prefix = "INB", Name = "Inbound", Status = 1 });

            context.Owners.Add(new Core.Entities.Owner
            {
                Code = "TESA",
                Site = "TESAP"
            });

            context.SupplierMasters.Add(new Core.Entities.SupplierMaster
            {
                FactoryID = factoryID,
                SupplierID = supplierID,
                CompanyName = "DIEHL AKO STIFTUNG & CO KG",
                Status = 1,
                IsBonded = 0,
            });

            context.PartMasters.Add(new Core.Entities.PartMaster 
            {
                CustomerCode = factoryID,
                SupplierID = supplierID,
                ProductCode1 = "A00944301",
                UOM = "UOM0070",
                PackageType = "PKG0001",
                SPQ = 160,
                OrderLot = 1,
                Length = 900,
                Width = 800,
                Height = 1200,
                NetWeight =1,
                GrossWeight =1,
                Status = 1,
                IsCPart = 0,
                LengthTT = 910,
                WidthTT = 811,
                HeightTT = 1212,
                NetWeightTT = 1.1M,
                GrossWeightTT = 1.1M
            });

            context.ASNHeaders.Add(new Core.Entities.ASNHeader
            {
                ASNNo = asnNo,
                FactoryID = factoryID,
                SupplierID = supplierID,
                ModeOfTransport = "ROAD",
                TotalPackages = 2,
                TotalWeight = 200,
                DirectToLine = 0,
                Status = "NEW",
                CreatedDate = DateTime.Now.AddDays(-1),//new System.DateTime("2020-09-04 12:15:29.000"),
                IsVMI = 1,
                NotifyEHP = 0,
                NotifyForwarder = 0,
                Filename = "",
                CreatedBy = "plegr",
                SupplierInvoiceNumber = "",
                ContainerNoEU = "",
                BillOfLadingEU = "",
                OrderNoEU = "",
                OrderDateEU = null,
                VesselNameEU = "",
                OriginPortEU = "",
                DestinationPortEU = "",
                ETDEU = DateTime.Now.AddDays(-2),
                ETAtoPortEU = null,
                Remark = "",
                ConfirmedETAEU = null,
                AirwayBillNo = "",
                ETAtoWHS = null
            });

            context.ASNDetails.Add(new Core.Entities.ASNDetail
            {
                ASNNo = asnNo,
                LineItem  = 1,
                ProductCode = "A00944301",
                OrderNo = "",
                ContainerNo = "",
                ContainerSize = "",
                SealNo = "",
                ManufacturedDate = DateTime.Now.AddDays(-10),
                BatchNo = "01",
                QtyPerOuter = 168,
                NoOfOuter = 1,
                Breakpoint= false,
                InJobNo = "",//INB20200900130
                BillOfLading = "",
                MaerskSONo = "",
                CreatedBy = "plegr",
                //ExSupplierDate  ShipDepartureDate   PortArrivalDate StoreArrivalDate    
                VesselName = "",
                VoyageNo = "",
                Status = "NEW",
                PreImportStatus = "NEW",
                PONo = "",
                POLineNo = ""
            });

            context.ASNDetails.Add(new Core.Entities.ASNDetail
            {
                ASNNo = asnNo,
                LineItem = 2,
                ProductCode = "A00944301",
                OrderNo = "",
                ContainerNo = "",
                ContainerSize = "",
                SealNo = "",
                ManufacturedDate = DateTime.Now.AddDays(-10),
                BatchNo = "01",
                QtyPerOuter = 166,
                NoOfOuter = 1,
                Breakpoint = false,
                InJobNo = "",//INB20200900130
                BillOfLading = "",
                MaerskSONo = "",
                CreatedBy = "plegr",
                //ExSupplierDate  ShipDepartureDate   PortArrivalDate StoreArrivalDate    
                VesselName = "",
                VoyageNo = "",
                Status = "NEW",
                PreImportStatus = "NEW",
                PONo = "",
                POLineNo = ""
            });
      
            context.SaveChanges();
        }

        private void AddManualInboundTestData(Context context)
        {
            context.Inbounds.Add(new Core.Entities.Inbound
            {
                JobNo = jobNo,
                CustomerCode = factoryID,
                WHSCode = whscode,
                IRNo = asnNo,
                RefNo = asnNo,
                ETA = DateTime.Now.Date.AddDays(-10),
                TransType = InboundType.ManualEntry,
                Charged = 1,
                Remark = "Remark",
                Status = InboundStatus.PartialPutaway,
                CreatedBy = "CreatedBy",
                CreatedDate = DateTime.Now.Date.AddDays(-10),
                RevisedBy = "RevisedBy",
                RevisedDate = DateTime.Now.Date.AddDays(-10),
                CancelledBy = "CancelledBy",
                CancelledDate = DateTime.Now.Date.AddDays(-10),
                PutawayBy = "PutawayBy",
                PutawayDate = DateTime.Now.Date.AddDays(-10),
                SupplierID = supplierID,
                Currency = "USD",
                IM4No = "IM4No",
            });
            context.InboundDetails.Add(new Core.Entities.InboundDetail
            {
                JobNo = jobNo,
                LineItem = 1,
                ProductCode = productCode,
                ControlCode1 = "cc1",
                ControlCode2 = "cc2",
                ControlCode3 = "cc3",
                ControlCode4 = "cc4",
                ControlCode5 = "cc5",
                ControlCode6 = "cc6",
                GrossWeight = 1120,
                NetWeight = 1100,
                Height = 110,
                Width = 120,
                Length = 130,
                ImportedQty = 1500,
                Qty = 100,
                NoOfLabel = 1,
                NoOfPackage = 1,
                PackageType = "packageType",
                ASNNo = "",
                ASNLineItem = null,
                BuyingPricePerLine = 777,
                ControlDate = DateTime.Now.AddDays(-10),
                CreatedBy = "CreatedBy",
                CreatedDate = DateTime.Now.AddDays(-10),
                PkgLineItem = 1,
                Remark = "Remark",
                RevisedBy = "",
                RevisedDate = null
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail
            {
                PID = "pid1",
                LineItem = 1,
                SeqNo = 1,
                ProductCode = productCode,
                SupplierID = supplierID,

                AllocatedQty = 0,
                BondedStatus = 0,
                BuyingPrice = 100,
                SellingPrice = 120,
                ChargedDate = null,
                CustomerCode = factoryID,
                InboundDate = DateTime.Now,
                InJobNo = jobNo,
                LocationCode = "loc",
                OriginalQty = 100,
                OutJobNo = "",
                Ownership = Ownership.EHP,
                QtyPerPkg = 100,
                Status = StorageStatus.Incoming,
                WHSCode = whscode,

                ControlCode1 = "cc1",
                ControlCode2 = "cc2",
                ControlCode3 = "cc3",
                ControlCode4 = "cc4",
                ControlCode5 = "cc5",
                ControlCode6 = "cc6",
                GrossWeight = 1120,
                NetWeight = 1100,
                Height = 110,
                Width = 120,
                Length = 130,
                Qty = 100,
                NoOfLabel = 1,
                ControlDate = DateTime.Now.AddDays(-10),
            });
            context.SaveChanges();
        }

        private readonly string jobNo = "INB00001";
        private readonly string asnNo = "80665633-500027";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PLY";
        private readonly string whscode = "PL";
        private readonly string productCode = "PRODCODE1";
    }

}
