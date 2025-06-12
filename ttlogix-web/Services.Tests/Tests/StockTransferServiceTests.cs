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
using TT.Services.Services.Utilities;

namespace TT.Services.Tests
{
    [TestClass]
    public class StockTransferServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task CreateStockTransfer()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
            }
            string newjobno;
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.CreateStockTransfer(factoryID, "USERCODE1", whscode);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                newjobno = result.Data.JobNo;
                Assert.IsNotNull(newjobno);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var st = context.StockTransfers.Find(newjobno);
                Assert.IsNotNull(st);
                Assert.AreEqual(newjobno, st.JobNo);
                Assert.AreEqual(factoryID, st.CustomerCode);
                Assert.AreEqual(whscode, st.WHSCode);
                Assert.AreEqual(StockTransferStatus.New, st.Status);
                Assert.AreEqual(StockTransferType.Over90Days, st.TransferType);
                Assert.AreEqual("USERCODE1", st.CreatedBy);
                Assert.IsTrue(now < st.CreatedDate);
                Assert.AreEqual(null, st.CommInvDate);
                Assert.AreEqual(null, st.DESADV);
                Assert.AreEqual(newjobno, st.RefNo);
                Assert.AreEqual(string.Empty, st.Remark);
                Assert.AreEqual(null, st.CommInvNo);
                Assert.AreEqual(string.Empty, st.CancelledBy);
                Assert.AreEqual(null, st.CancelledDate);
                Assert.AreEqual(string.Empty, st.ConfirmedBy);
                Assert.AreEqual(null, st.ConfirmedDate);
                Assert.AreEqual(string.Empty, st.RevisedBy);
                Assert.AreEqual(null, st.RevisedDate);
            }
        }

        [TestMethod]
        public async Task UpdateStockTransfer()
        {
            var stOriginal = new StockTransfer
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                CreatedBy = "USERCODE1",
                TransferType = StockTransferType.Over90Days,
                Status = StockTransferStatus.New,
                CreatedDate = DateTime.Now,
                RefNo = "",
                Remark = ""
            };
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(stOriginal);
                context.SaveChanges();
            }
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.UpdateStockTransfer(stOriginal.JobNo, new Models.StockTransferDto
                {
                    JobNo = stOriginal.JobNo,
                    CustomerCode = "XX",
                    WHSCode = "YY",
                    CreatedBy = "ABC",
                    TransferType = StockTransferType.Damaged,
                    Status = StockTransferStatus.Processing,
                    CancelledBy = "XX",
                    CancelledDate = now,
                    CommInvDate = now,
                    CommInvNo = "XX",
                    ConfirmedBy = "XX",
                    ConfirmedDate = now,
                    CreatedDate = now,
                    DESADV = true,
                    RefNo = "XX",
                    Remark ="XX",
                    RevisedBy = "XX",
                    RevisedDate = now
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var st = context.StockTransfers.Find(stOriginal.JobNo);
                Assert.IsNotNull(st);

                // properties changed
                Assert.AreEqual("XX", st.RefNo);
                Assert.AreEqual("XX", st.Remark);
                Assert.AreEqual("XX", st.CommInvNo);
                Assert.AreEqual(now, st.CommInvDate);
                Assert.AreEqual(StockTransferType.Damaged, st.TransferType);

                // properties not changed
                Assert.AreEqual("USERCODE1", st.CreatedBy);
                Assert.AreEqual(string.Empty, st.CancelledBy);
                Assert.AreEqual(null, st.CancelledDate);
                Assert.AreEqual(string.Empty, st.ConfirmedBy);
                Assert.AreEqual(null, st.ConfirmedDate);
                Assert.AreEqual(string.Empty, st.RevisedBy);
                Assert.AreEqual(null, st.RevisedDate);
                Assert.AreEqual(stOriginal.JobNo, st.JobNo);
                Assert.AreEqual(factoryID, st.CustomerCode);
                Assert.AreEqual(whscode, st.WHSCode);
                Assert.AreEqual(StockTransferStatus.New, st.Status);
                Assert.IsTrue(now > st.CreatedDate);
                Assert.AreEqual(null, st.DESADV);
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart_AlreadyImported()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddEKanbanTestData(context);

                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = refno,
                    Remark = ""
                });

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.ImportEKanbanEUCPart(refno, whscode, "UserCode2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("UnableToImportOrderImported__", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task ImportEKanbanEUCPart()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddEKanbanTestData(context);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.ImportEKanbanEUCPart(refno, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var st = context.StockTransfers.Single();
                Assert.AreEqual($"STK{now.Year}{now.Month:00}00001", st.JobNo);
                Assert.AreEqual(StockTransferStatus.Processing, st.Status);
                Assert.AreEqual("USERCODE2", st.CreatedBy);
                Assert.AreEqual("", st.CancelledBy);
                Assert.AreEqual("", st.ConfirmedBy);
                Assert.AreEqual(null, st.CancelledDate);
                Assert.AreEqual(null, st.ConfirmedDate);
                Assert.AreEqual(null, st.CommInvDate);
                Assert.AreEqual(null, st.CommInvNo);
                Assert.AreEqual("USERCODE2", st.CreatedBy);
                Assert.IsTrue(now < st.CreatedDate);
                Assert.AreEqual(factoryID, st.CustomerCode);
                Assert.AreEqual(refno, st.RefNo);
                Assert.AreEqual(whscode, st.WHSCode);
                Assert.AreEqual(StockTransferType.Over90Days, st.TransferType);

                var std = context.StockTransferDetails.OrderBy(s => s.LineItem).ToList();
                Assert.AreEqual(2, std.Count());

                var line1 = std[0];
                var line2 = std[1];

                Assert.AreEqual(1, line1.LineItem);
                Assert.AreEqual(2, line2.LineItem);
                Assert.AreEqual(st.JobNo, line1.JobNo);
                Assert.AreEqual(st.JobNo, line2.JobNo);
                Assert.AreEqual(pid2, line1.PID);
                Assert.AreEqual(pid1, line2.PID);
                Assert.AreEqual("L2", line1.LocationCode);
                Assert.AreEqual("L1", line2.LocationCode);
                Assert.AreEqual(100, line1.Qty);
                Assert.AreEqual(100, line2.Qty);
                Assert.AreEqual("L2", line1.OriginalLocationCode);
                Assert.AreEqual("L1", line2.OriginalLocationCode);
                Assert.AreEqual("SUPPLIER1", line1.OriginalSupplierID);
                Assert.AreEqual("SUPPLIER1", line2.OriginalSupplierID);
                Assert.AreEqual(whscode, line1.OriginalWHSCode);
                Assert.AreEqual(whscode, line2.OriginalWHSCode);
                Assert.AreEqual(whscode, line1.WHSCode);
                Assert.AreEqual(whscode, line2.WHSCode);
                Assert.AreEqual("USERCODE2", line1.TransferredBy);
                Assert.AreEqual("USERCODE2", line2.TransferredBy);
                Assert.IsTrue(now < line1.TransferredDate);
                Assert.IsTrue(now < line2.TransferredDate);

                var storage1 = context.StorageDetails.Find(line1.PID);
                var storage2 = context.StorageDetails.Find(line1.PID);
                Assert.AreEqual(StorageStatus.Transferring, storage1.Status);
                Assert.AreEqual(StorageStatus.Transferring, storage2.Status);

                var ekanban = context.EKanbanHeaders.Where(e => e.OrderNo == refno).Single();
                Assert.AreEqual((int)EKanbanStatus.Imported, ekanban.Status);
                Assert.AreEqual(st.JobNo, ekanban.OutJobNo);
            }
        }

        [TestMethod]
        public async Task ImportEStockTransfer_AlreadyImported()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddEStockTransferTestData(context);
                context.SunsetExpiredAlerts.Add(new SunsetExpiredAlert
                {
                    FactoryID = factoryID,
                    SupplierID = supplierID,
                    ProductCode = productCode,
                    SunsetPeriod = 10,
                });
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.EStockTransfer,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = refno,
                    Remark = ""
                });

                context.SaveChanges();
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.ImportEStockTransfer(refno, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("UnableToImportOrderImported__", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task ImportEStockTransfer()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddEStockTransferTestData(context);
                context.SunsetExpiredAlerts.Add(new SunsetExpiredAlert
                {
                    FactoryID = factoryID,
                    SupplierID = supplierID,
                    ProductCode = productCode,
                    SunsetPeriod = 10,
                });

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.ImportEStockTransfer(refno, whscode, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var st = context.StockTransfers.Single();
                Assert.AreEqual($"STK{now.Year}{now.Month:00}00001", st.JobNo);
                Assert.AreEqual(StockTransferStatus.Processing, st.Status);
                Assert.AreEqual("USERCODE2", st.CreatedBy);
                Assert.AreEqual("", st.CancelledBy);
                Assert.AreEqual("", st.ConfirmedBy);
                Assert.AreEqual(null, st.CancelledDate);
                Assert.AreEqual(null, st.ConfirmedDate);
                Assert.AreEqual(null, st.CommInvDate);
                Assert.AreEqual(null, st.CommInvNo);
                Assert.AreEqual("USERCODE2", st.CreatedBy);
                Assert.IsTrue(now < st.CreatedDate);
                Assert.AreEqual(factoryID, st.CustomerCode);
                Assert.AreEqual(refno, st.RefNo);
                Assert.AreEqual(whscode, st.WHSCode);
                Assert.AreEqual(StockTransferType.EStockTransfer, st.TransferType);

                var std = context.StockTransferDetails.OrderBy(s => s.LineItem).ToList();
                Assert.AreEqual(2, std.Count());

                var line1 = std[0];
                var line2 = std[1];

                Assert.AreEqual(1, line1.LineItem);
                Assert.AreEqual(2, line2.LineItem);
                Assert.AreEqual(st.JobNo, line1.JobNo);
                Assert.AreEqual(st.JobNo, line2.JobNo);
                Assert.AreEqual(pid1, line1.PID);
                Assert.AreEqual(pid2, line2.PID);
                Assert.AreEqual("L1", line1.LocationCode);
                Assert.AreEqual("L2", line2.LocationCode);
                Assert.AreEqual(100, line1.Qty);
                Assert.AreEqual(100, line2.Qty);
                Assert.AreEqual("L1", line1.OriginalLocationCode);
                Assert.AreEqual("L2", line2.OriginalLocationCode);
                Assert.AreEqual("SUPPLIER1", line1.OriginalSupplierID);
                Assert.AreEqual("SUPPLIER1", line2.OriginalSupplierID);
                Assert.AreEqual(whscode, line1.OriginalWHSCode);
                Assert.AreEqual(whscode, line2.OriginalWHSCode);
                Assert.AreEqual(whscode, line1.WHSCode);
                Assert.AreEqual(whscode, line2.WHSCode);
                Assert.AreEqual("USERCODE2", line1.TransferredBy);
                Assert.AreEqual("USERCODE2", line2.TransferredBy);
                Assert.IsTrue(now < line1.TransferredDate);
                Assert.IsTrue(now < line2.TransferredDate);

                var storage1 = context.StorageDetails.Find(line1.PID);
                var storage2 = context.StorageDetails.Find(line1.PID);
                Assert.AreEqual(StorageStatus.Transferring, storage1.Status);
                Assert.AreEqual(StorageStatus.Transferring, storage2.Status);

                var est = context.EStockTransferHeaders.Where(e => e.OrderNo == refno).Single();
                Assert.AreEqual(EStockTransferStatus.Imported, est.Status);
                Assert.AreEqual(st.JobNo, est.StockTransferJobNo);
            }
        }

        [TestMethod]
        public async Task AddStockTransferDetailByPID_StorageDetailIncorrectOwnerOrStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });
                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    LocationCode = "L2",
                    Qty = 100,
                    OriginalLocationCode = "L2",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid2,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                var sd3 = context.StorageDetails.Find(pid3);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Allocated;
                sd3.Status = StorageStatus.Putaway;
                sd3.Ownership = Ownership.EHP;

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.AddStockTransferDetailByPID(new Models.StockTransferDetailByPIDDto
                {
                    JobNo = jobno,
                    PIDs = new string[] { pid3 }
                }, "UserCode2");
                Assert.AreEqual(ServiceResult.ResultType.NotFound, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("StorageNotFoundOrNotPutawayOrNotOwnedBySupplier", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task AddStockTransferDetailByPID()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                var sd3 = context.StorageDetails.Find(pid3);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Putaway;
                sd3.Status = StorageStatus.Putaway;
                sd2.Ownership = Ownership.Supplier;
                sd3.Ownership = Ownership.Supplier;

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.AddStockTransferDetailByPID(new Models.StockTransferDetailByPIDDto
                {
                    JobNo = jobno,
                    PIDs = new string[] { pid2, pid3 }
                }, "UserCode2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(3, context.StockTransferDetails.Count());
                var st = context.StockTransfers.Single();
                var std2 = context.StockTransferDetails.Find(jobno, 2);
                var std3 = context.StockTransferDetails.Find(jobno, 3);
                CollectionAssert.AreEquivalent(new string[] { pid2, pid3 }, new string[] { std2.PID, std3.PID });
         
                Assert.AreEqual(StockTransferStatus.Processing, st.Status);

                var sd2 = context.StorageDetails.Where(s => s.PID == pid2).Single();
                var sd3 = context.StorageDetails.Where(s => s.PID == pid3).Single();
                Assert.AreEqual(StorageStatus.Transferring, sd2.Status);
                Assert.AreEqual(StorageStatus.Transferring, sd3.Status);
            }
        }

        [TestMethod]
        public async Task DeleteStockTransferDetailByPID_DeleteOneLineOnly()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });
                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    LocationCode = "L2",
                    Qty = 100,
                    OriginalLocationCode = "L2",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid2,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Allocated;

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.DeleteStockTransferDetailByPID(new Models.StockTransferDetailByPIDDto
                {
                    JobNo = jobno,
                    PIDs = new string[] { pid1 }
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(1, context.StockTransferDetails.Count());
                var st = context.StockTransfers.Single();
                Assert.AreEqual(StockTransferStatus.Processing, st.Status);
                var sd = context.StorageDetails.Where(s => s.PID == pid1).Single();
                Assert.AreEqual(StorageStatus.Putaway, sd.Status);
            }
        }

        [TestMethod]
        public async Task DeleteStockTransferDetailByPID_DeleteAllLines()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });
                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    LocationCode = "L2",
                    Qty = 100,
                    OriginalLocationCode = "L2",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid2,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Allocated;

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.DeleteStockTransferDetailByPID(new Models.StockTransferDetailByPIDDto
                {
                    JobNo = jobno,
                    PIDs = new string[] { pid1, pid2 }
                });
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(0, context.StockTransferDetails.Count());
                var st = context.StockTransfers.Single();
                Assert.AreEqual(StockTransferStatus.New, st.Status);
                var sd = context.StorageDetails.Where(s => s.PID == pid1).Single();
                Assert.AreEqual(StorageStatus.Putaway, sd.Status);
                var sd2 = context.StorageDetails.Where(s => s.PID == pid2).Single();
                Assert.AreEqual(StorageStatus.Putaway, sd2.Status);
            }
        }

        [TestMethod]
        public async Task DeleteStockTransferDetail_HasSingleLineOnly()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Allocated;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.DeleteStockTransferDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.IsFalse(context.StockTransferDetails.Any());
                var st = context.StockTransfers.Single();
                Assert.AreEqual(StockTransferStatus.New, st.Status);
                var sd = context.StorageDetails.Where(s => s.PID == pid1).Single();
                Assert.AreEqual(StorageStatus.Putaway, sd.Status);
            }
        }

        [TestMethod]
        public async Task DeleteStockTransferDetail_HasMultipleLines()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                context.StockTransfers.Add(new StockTransfer
                {
                    JobNo = jobno,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    CreatedBy = "USERCODE1",
                    TransferType = StockTransferType.Over90Days,
                    Status = StockTransferStatus.Processing,
                    CreatedDate = DateTime.Now,
                    RefNo = "",
                    Remark = ""
                });

                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 1,
                    LocationCode = "L1",
                    Qty = 100,
                    OriginalLocationCode = "L1",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid1,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });
                context.StockTransferDetails.Add(new StockTransferDetail
                {
                    JobNo = jobno,
                    LineItem = 2,
                    LocationCode = "L2",
                    Qty = 100,
                    OriginalLocationCode = "L2",
                    OriginalSupplierID = supplierID,
                    OriginalWHSCode = whscode,
                    PID = pid2,
                    TransferredBy = "UserCode0",
                    TransferredDate = DateTime.Now.AddDays(-100),
                    WHSCode = whscode
                });

                var sd1 = context.StorageDetails.Find(pid1);
                var sd2 = context.StorageDetails.Find(pid2);
                sd1.Status = StorageStatus.Allocated;
                sd2.Status = StorageStatus.Allocated;

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.DeleteStockTransferDetail(jobno, 1);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(1, context.StockTransferDetails.Count());
                var st = context.StockTransfers.Single();
                Assert.AreEqual(StockTransferStatus.Processing, st.Status);
                var sd = context.StorageDetails.Where(s => s.PID == pid1).Single();
                Assert.AreEqual(StorageStatus.Putaway, sd.Status);
            }
        }
     
        [TestMethod]
        public async Task Cancel_WrongStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
            }
            foreach (var status in new List<StockTransferStatus>() { StockTransferStatus.Cancelled, StockTransferStatus.Completed, StockTransferStatus.Processing })
            { 
                using (var context = new Context(options, appSettings.Object))
                {
                    var st = context.StockTransfers.Find(jobno);
                    st.Status = status;
                    context.SaveChanges();
                }
                using (var context = new Context(options, appSettings.Object))
                {
                    var service = GetStockTransferService(context);
                    var result = await service.Cancel(jobno, "USERCODE2");
                    Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                    var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                    Assert.AreEqual("StockTransferCannotBeCancelled" + status.ToString(), resultError.MessageKey);
                }
            }
        }

        [TestMethod]
        public async Task Cancel_DetailsExist()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);

                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Outstanding;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Cancel(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("FailToCancelStockTransferRemovePIDs", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task Cancel()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Outstanding;
                foreach (var std in context.StockTransferDetails.ToList())
                    context.StockTransferDetails.Remove(std);
                context.SaveChanges();
            }
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Cancel(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Find(jobno);
                Assert.AreEqual(StockTransferStatus.Cancelled, stockTransfer.Status);
                Assert.AreEqual("USERCODE2", stockTransfer.CancelledBy);
                Assert.IsTrue(now < stockTransfer.CancelledDate);
            }
        }

        [TestMethod]
        public async Task Cancel_ChangeStatusOfEKanban()
        {
            var now = DateTime.Now;
            await ImportEKanbanEUCPart();
            var importedJobNo = $"STK{now.Year}{now.Month:00}00001";
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Find(importedJobNo);
                stockTransfer.Status = StockTransferStatus.Outstanding;
                var std = context.StockTransferDetails.Where(s => s.JobNo == importedJobNo).ToList();
                foreach (var d in std)
                {
                    context.StockTransferDetails.Remove(d);
                }
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Cancel(importedJobNo, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Find(importedJobNo);
                Assert.AreEqual(StockTransferStatus.Cancelled, stockTransfer.Status);
                Assert.AreEqual("USERCODE2", stockTransfer.CancelledBy);
                Assert.IsTrue(now < stockTransfer.CancelledDate);

                var ekanban = context.EKanbanHeaders.Find(stockTransfer.RefNo);
                Assert.AreEqual((int)EKanbanStatus.New, ekanban.Status);
                Assert.AreEqual(string.Empty, ekanban.OutJobNo);
            }
        }

        [TestMethod]
        public async Task Cancel_ChangeStatusOfEStockTransfer()
        {
            var now = DateTime.Now;
            await ImportEStockTransfer();
            var importedJobNo = $"STK{now.Year}{now.Month:00}00001";
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Find(importedJobNo);
                stockTransfer.Status = StockTransferStatus.Outstanding;
                var std = context.StockTransferDetails.Where(s => s.JobNo == importedJobNo).ToList();
                foreach (var d in std)
                {
                    context.StockTransferDetails.Remove(d);
                }
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Cancel(importedJobNo, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Find(importedJobNo);
                Assert.AreEqual(StockTransferStatus.Cancelled, stockTransfer.Status);
                Assert.AreEqual("USERCODE2", stockTransfer.CancelledBy);
                Assert.IsTrue(now < stockTransfer.CancelledDate);

                var estocktransfer = context.EStockTransferHeaders.Find(stockTransfer.RefNo);
                Assert.AreEqual(EStockTransferStatus.New, estocktransfer.Status);
                Assert.AreEqual(string.Empty, estocktransfer.StockTransferJobNo);
            }
        }

        [TestMethod]
        public async Task Complete_StatusNew()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.New;
                foreach (var std in context.StockTransferDetails.ToList())
                    context.StockTransferDetails.Remove(std);
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("StockTransferCannotBeCompletedDetailsEmpty", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task Complete_StatusCompleted()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Completed;
                foreach (var std in context.StockTransferDetails.ToList())
                    context.StockTransferDetails.Remove(std);
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("StockTransferCannotBeCompletedConfirmed", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task Complete_StatusCancelled()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Cancelled;
                foreach (var std in context.StockTransferDetails.ToList())
                    context.StockTransferDetails.Remove(std);
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("StockTransferCannotBeCompletedCancelled", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task Complete_InboundIsNotCompleted()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                AddEKanbanTestData(context);
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandPkg = 100,
                    OnHandQty = 10000,

                });
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandPkg = 100,
                    OnHandQty = 10000,
                });
                context.Inbounds.Add(new Inbound
                {
                    JobNo = injobno,
                    Currency = "EUR",
                    Status = InboundStatus.Outstanding
                });

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("CannotCompleteStockTranferInboundNotCompleted", resultError.MessageKey);
            }
        }

        [TestMethod]
        public async Task Complete_StorageIsIncoming()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                AddEKanbanTestData(context);
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandPkg = 100,
                    OnHandQty = 10000,

                });
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandPkg = 100,
                    OnHandQty = 10000,
                });
                context.Inbounds.Add(new Inbound
                {
                    JobNo = injobno,
                    Currency = "EUR",
                    Status = InboundStatus.Completed
                });
                var sd = repository.StorageDetails().Where(s => s.PID == pid1).First();
                sd.Status = StorageStatus.Incoming;
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                var resultError = JsonSerializer.Deserialize<JsonResultError>(result.Errors[0]);
                Assert.AreEqual("CannotCompleteStockTranferStorageIsInStatusIncoming", resultError.MessageKey);
            }
        }


        [TestMethod]
        public async Task Complete()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                AddEKanbanTestData(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Outstanding;

                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandPkg = 100,
                    OnHandQty = 10000,

                });
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandPkg = 100,
                    OnHandQty = 10000,
                });

                context.Inbounds.Add(new Inbound
                {
                    JobNo = injobno,
                    Currency = "EUR",
                    Status = InboundStatus.Completed
                });

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.Complete(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var stockTransfer = context.StockTransfers.Single();
                Assert.AreEqual(StockTransferStatus.Completed, stockTransfer.Status);
                Assert.AreEqual("USERCODE2", stockTransfer.ConfirmedBy);
                Assert.IsTrue(now < stockTransfer.ConfirmedDate);

                var inventorySupplier = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.Supplier);
                Assert.AreEqual(9800, inventorySupplier.OnHandQty);
                Assert.AreEqual(98, inventorySupplier.OnHandPkg);
                var inventoryEHP = context.Inventory.Find(factoryID, supplierID, productCode, whscode, Ownership.EHP);
                Assert.AreEqual(10200, inventoryEHP.OnHandQty);
                Assert.AreEqual(102, inventoryEHP.OnHandPkg);

                Assert.IsNotNull(context.InvTransactionsPerSupplier.Where(s => s.Ownership == Ownership.Supplier).SingleOrDefault());
                Assert.IsNotNull(context.InvTransactionsPerSupplier.Where(s => s.Ownership == Ownership.EHP).SingleOrDefault());

                var storage1 = context.StorageDetails.Find(pid1);
                var storage2 = context.StorageDetails.Find(pid2);

                Assert.AreEqual(Ownership.EHP, storage1.Ownership);
                Assert.AreEqual(Ownership.EHP, storage2.Ownership);
            }
        }

        [TestMethod]
        public async Task SplitByInboundDate()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
                AddBasicTestData(context);
                AddStockTransferTestRecords(context);
                AddEKanbanTestData(context);
                var st = context.StockTransfers.Find(jobno);
                st.Status = StockTransferStatus.Processing;

                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.Supplier,
                    OnHandPkg = 100,
                    OnHandQty = 10000,

                });
                context.Inventory.Add(new Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = Ownership.EHP,
                    OnHandPkg = 100,
                    OnHandQty = 10000,
                });

                context.Inbounds.Add(new Inbound
                {
                    JobNo = injobno,
                    Currency = "EUR",
                    CreatedDate = DateTime.Now.AddDays(-20)
                });

                context.Inbounds.Add(new Inbound
                {
                    JobNo = "injobno2",
                    Currency = "EUR",
                    CreatedDate = DateTime.Now.AddDays(-30)
                });

                var sd2 = context.StorageDetails.Find(pid2);
                sd2.InJobNo = "injobno2";
                sd2.InboundDate = DateTime.Now.AddDays(-30);

                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var service = GetStockTransferService(context);
                var result = await service.SplitByInboundDate(jobno, "USERCODE2");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var st = context.StockTransfers.OrderBy(s=>s.CreatedDate).ToList();
                var stCancelled = st[0]; 
                var stnew1 = st[1]; 
                var stnew2 = st[2];
                Assert.AreEqual(jobno, stCancelled.JobNo);
                Assert.AreEqual($"STK{now.Year}{now.Month:00}00001", stnew1.JobNo);
                Assert.AreEqual($"STK{now.Year}{now.Month:00}00002", stnew2.JobNo);
                Assert.AreEqual(StockTransferStatus.Cancelled, stCancelled.Status);
                Assert.AreEqual(StockTransferStatus.Processing, stnew1.Status);
                Assert.AreEqual(StockTransferStatus.Processing, stnew2.Status);

                var std = context.StockTransferDetails.ToList();
                Assert.AreEqual(2, std.Count);
                var std1 = std.Where(s => s.JobNo == stnew1.JobNo).FirstOrDefault();
                var std2 = std.Where(s => s.JobNo == stnew2.JobNo).FirstOrDefault();
                Assert.IsNotNull(std1);
                Assert.IsNotNull(std2);

                Assert.AreEqual(1, std1.LineItem);
                Assert.AreEqual(1, std2.LineItem);
            }
        }

        private void AddBasicTestData(Context context)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.StockTransfer, Prefix = "STK", Name = "StockTransfer", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer" });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid4,
                InJobNo = injobno,
                LineItem = 3,
                SeqNo = 1,
                ParentID = "",
                ProductCode = productCode,
                CustomerCode = factoryID,
                SupplierID = supplierID,
                InboundDate = DateTime.Now.Date.AddDays(-1),
                OriginalQty = 200,
                Qty = 200,
                QtyPerPkg = 200,
                AllocatedQty = 0,
                OutJobNo = "",
                WHSCode = whscode,
                LocationCode = "L4",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                PutawayDate = DateTime.Now.AddDays(-1)
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid1,
                InJobNo = injobno,
                LineItem = 1,
                SeqNo = 1,
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
                LocationCode = "L1",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                PutawayDate = DateTime.Now.AddDays(-100)
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
                InboundDate = DateTime.Now.Date.AddDays(-30),
                OriginalQty = 150,
                Qty = 100,
                QtyPerPkg = 100,
                AllocatedQty = 100,
                OutJobNo = jobno,
                WHSCode = whscode,
                LocationCode = "L2",
                Status = StorageStatus.Putaway,
                Ownership = Ownership.Supplier,
                PutawayDate = DateTime.Now.AddDays(-100)
            });
            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = pid3,
                InJobNo = injobno,
                LineItem = 2,
                SeqNo = 1,
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
                LocationCode = "L3",
                Status = StorageStatus.Allocated,
                BondedStatus = (int)BondedStatus.Bonded,
                Ownership = Ownership.Supplier,
                PutawayDate = DateTime.Now.AddDays(-100)
            });

            context.SaveChanges();
        }

        private void AddStockTransferTestRecords(Context context)
        {
            context.StockTransfers.Add(new StockTransfer
            {
                JobNo = jobno,
                Status = StockTransferStatus.Processing,
                CreatedBy = "USERCODE1",
                CreatedDate = DateTime.Now,
                TransferType = StockTransferType.Over90Days,
                WHSCode = whscode,
                CustomerCode = factoryID
            });
            context.StockTransferDetails.Add(new StockTransferDetail
            {
                JobNo = jobno,
                LineItem = 1,
                LocationCode = "L1",
                Qty = 100,
                PID = pid1,
                OriginalLocationCode = "L1",
                OriginalSupplierID = supplierID,
                OriginalWHSCode = whscode,
                TransferredBy = "USERCODE1",
                TransferredDate = DateTime.Now,
                WHSCode = whscode
            });
            context.StockTransferDetails.Add(new StockTransferDetail
            {
                JobNo = jobno,
                LineItem = 2,
                LocationCode = "L1",
                Qty = 100,
                PID = pid2,
                OriginalLocationCode = "L1",
                OriginalSupplierID = supplierID,
                OriginalWHSCode = whscode,
                TransferredBy = "USERCODE1",
                TransferredDate = DateTime.Now,
                WHSCode = whscode
            });
            context.SaveChanges();
        }

        private void AddEKanbanTestData(Context context)
        {
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
            context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
            {
                UOM = uom,
                CustomerCode = factoryID,
                Status = 1,
                DecimalNum = 3
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
            context.SupplierMasters.Add(new Core.Entities.SupplierMaster()
            {
                SupplierID = supplierID,
                FactoryID = factoryID,
                CompanyName = "Company1",
                SupplyParadigm = "LV"
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

        private void AddEStockTransferTestData(Context context)
        {
            context.EStockTransferHeaders.Add(new Core.Entities.EStockTransferHeader
            {
                OrderNo = refno,
                FactoryID = factoryID,
                IssuedDate = DateTime.Now.Date.AddDays(-50),
                CreatedDate = DateTime.Now.Date.AddDays(-50),
                //OutJobNo = jobno,
                Status = (byte)EKanbanStatus.New,
                //Instructions = "EHP"
            });
            context.EStockTransferDetails.Add(new Core.Entities.EStockTransferDetail()
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "1",
                SupplierID = supplierID,
                DropPoint = "01",
                Quantity = 100
            });
            context.EStockTransferDetails.Add(new Core.Entities.EStockTransferDetail()
            {
                OrderNo = refno,
                ProductCode = productCode,
                SerialNo = "2",
                SupplierID = supplierID,
                DropPoint = "01",
                Quantity = 100
            });
            context.UOMDecimals.Add(new Core.Entities.UOMDecimal()
            {
                UOM = uom,
                CustomerCode = factoryID,
                Status = 1,
                DecimalNum = 3
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

        private IStockTransferService GetStockTransferService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var utilityService = new UtilityService(repository, appSettings.Object);
            var eKanbanService = new EKanbanService(repository, utilityService, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var eStockTransferService = new EStockTransferService(repository, mapper, locker, new Logger<EStockTransferService>(loggerFactory));
            var billingService = new BillingService(repository);
            var reportService = new Mock<IReportService>().Object;
            var ilogConnect = new Mock<IILogConnect>().Object;

            return new StockTransferService(repository, utilityService, eKanbanService, eStockTransferService, billingService, reportService, appSettings.Object, locker, mapper, new Logger<StockTransferService>(loggerFactory), ilogConnect);
        }

        private readonly string jobno = "STK20200900626";
        private readonly string productCode = "PRODCODE1";
        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";
        private readonly string uom = "UOM0000";
        private readonly string pid1 = "TESAP202009000EC6";
        private readonly string pid2 = "TESAP202009000EC8";
        private readonly string pid3 = "TESAP202009000EC9";
        private readonly string pid4 = "TESAP202009000ED0";
        private readonly string whscode = "PL";
        private readonly string injobno = "INB20200900125";
        private readonly string refno = "9200909056";
    }
}
