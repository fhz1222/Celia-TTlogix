using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT.DB;
using TT.Services.Services;
using TT.Core.Enums;
using TT.Services.Interfaces;
using Moq;
using System.Threading;
using System.Diagnostics;
using TT.Core.Interfaces;
using TT.Services.Services.Utilities;

namespace TT.Services.Tests
{
    [TestClass]
    public class LoadingServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task GetLoadingList_NewJobStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.NewJob, OutboundStatus.NewJob);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoadingList(new Core.QueryFilters.LoadingListQueryFilter()
                {
                    //JobNo = jobno,
                    UserWHSCode = whscode,
                    PageNo = 1,
                    PageSize = 20
                });
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(1, result.Total);
                Assert.AreEqual(1, result.PageNo);
                Assert.AreEqual(20, result.PageSize);
                var row = result.Data.First();
                Assert.AreEqual(jobno, row.JobNo);
                Assert.AreEqual(LoadingStatus.NewJob, row.Status);
                Assert.AreEqual("TruckLicencePlate", row.TruckLicencePlate);
                Assert.AreEqual("TrailerNo", row.TrailerNo);
                Assert.AreEqual("Seq1", row.TruckSeqNo);
                Assert.AreEqual("001", row.DockNo);
                Assert.AreEqual(10, row.NoOfPallet);
            }
        }

        [TestMethod]
        public async Task GetLoadingList_PickedStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.InTransit, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoadingList(new Core.QueryFilters.LoadingListQueryFilter()
                {
                    //JobNo = jobno,
                    UserWHSCode = whscode,
                    PageNo = 1,
                    PageSize = 20
                });
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(1, result.Total);
                Assert.AreEqual(1, result.PageNo);
                Assert.AreEqual(20, result.PageSize);
                var row = result.Data.First();
                Assert.AreEqual(jobno, row.JobNo);
                Assert.AreEqual(LoadingStatus.NewJob, row.Status);
            }
        }

        [TestMethod]
        public async Task GetLoadingList_ProcessingStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoadingList(new Core.QueryFilters.LoadingListQueryFilter()
                {
                    //JobNo = jobno,
                    UserWHSCode = whscode,
                    PageNo = 1,
                    PageSize = 20
                });
                Assert.AreEqual(1, result.Data.Count());
                Assert.AreEqual(1, result.Total);
                Assert.AreEqual(1, result.PageNo);
                Assert.AreEqual(20, result.PageSize);
                var row = result.Data.First();
                Assert.AreEqual(jobno, row.JobNo);
                Assert.AreEqual(LoadingStatus.NewJob, row.Status);
            }
        }

        [TestMethod]
        public async Task GetLoading()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoading(jobno);
                Assert.IsNotNull(result);
                Assert.AreEqual(whscode, result.WHSCode);
                Assert.AreEqual(LoadingStatus.NewJob, result.Status);
                Assert.AreEqual(refno, result.RefNo);
                Assert.AreEqual(10, result.NoOfPallet);
                Assert.AreEqual(jobno, result.JobNo);
                Assert.AreEqual("Customer", result.CustomerName);
                Assert.AreEqual(factoryID, result.CustomerCode);
                Assert.AreEqual("", result.Currency);
                Assert.IsTrue(result.MixedCurrencies);

            }
        }

        [TestMethod]
        public async Task GetLoadingDetails()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoadingDetails(jobno);
                Assert.AreEqual(2, result.Count());
                var row1 = result.First();
                var row2 = result.Last();
                Assert.AreEqual(supplierID, row1.SupplierID);
                Assert.AreEqual(supplierID, row2.SupplierID);

                Assert.AreEqual(orderNo1, row1.OrderNo);
                Assert.AreEqual(orderNo2, row2.OrderNo);

                Assert.AreEqual(outjobno1, row1.OutJobNo);
                Assert.AreEqual(outjobno2, row2.OutJobNo);

                Assert.AreEqual(OutboundStatus.PartialPicked, row1.OutboundStatus);
                Assert.AreEqual(OutboundStatus.Completed, row2.OutboundStatus);

                Assert.AreEqual("GBP", row1.Currency);
                Assert.AreEqual("PLN", row2.Currency);

                Assert.AreEqual("Company1", row1.CompanyName);
                Assert.AreEqual("Company1", row2.CompanyName);
             
                Assert.AreEqual(supplierID, row1.SupplierID);
                Assert.AreEqual(supplierID, row2.SupplierID);
            }
        }

        [TestMethod]
        public async Task CreateLoading()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.CreateLoading(new Models.LoadingAddDto { 
                    CustomerCode = factoryID,
                    ETD = now,
                    RefNo = "123",
                    Remark = "remark",
                    WHSCode = whscode
                }, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual($"LDG{now.Date.Year}{now.Date.Month.ToString("00")}00001" , result.Data.JobNo);
                Assert.AreEqual(factoryID, result.Data.CustomerCode);
                Assert.AreEqual("Customer", result.Data.CustomerName);
                Assert.AreEqual(factoryID, result.Data.CustomerCode);
                Assert.AreEqual(now, result.Data.ETD);
                Assert.AreEqual("123", result.Data.RefNo);
                Assert.AreEqual("remark", result.Data.Remark);
                Assert.AreEqual(whscode, result.Data.WHSCode);
                Assert.AreEqual("user1", result.Data.CreatedBy);
                Assert.IsTrue(result.Data.CreatedDate >= now);
                Assert.AreEqual(null, result.Data.Currency);
                Assert.IsFalse(result.Data.MixedCurrencies);
                Assert.IsNull(result.Data.TruckSeqNo);
                Assert.IsNull(result.Data.TruckLicencePlate);
                Assert.IsNull(result.Data.DockNo);
                Assert.IsNull(result.Data.TrailerNo);

                Assert.AreEqual(LoadingStatus.NewJob, result.Data.Status);
            }
        }

        [TestMethod]
        public async Task UpdateLoading()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.UpdateLoading(jobno, new Models.LoadingDto
                {
                    RefNo = "refChanged",
                    Remark = "remarkChaned",
                    NoOfPallet = 1000,
                    ETD = now,
                    ETA = now.AddDays(1),
                    // these properties should not be overwritten
                    Status = LoadingStatus.InTransit,
                    CustomerCode = "xxxx",
                    CustomerName = "yyy",
                    WHSCode = "vvv",
                    TruckSeqNo = "TruckSeqNo",
                    TruckLicencePlate = "TruckLicencePlate",
                    DockNo = "DockNo",
                    TrailerNo = "TrailerNo"
                }, "user1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data);

                Assert.AreEqual("refChanged", result.Data.RefNo);
                Assert.AreEqual("remarkChaned", result.Data.Remark);
                Assert.AreEqual(1000, result.Data.NoOfPallet);
                Assert.AreEqual(now, result.Data.ETD);
                Assert.AreEqual(now.AddDays(1), result.Data.ETA);

                Assert.AreEqual(jobno, result.Data.JobNo);
                Assert.AreEqual(factoryID, result.Data.CustomerCode);
                Assert.AreEqual("Customer", result.Data.CustomerName);
                Assert.AreEqual(factoryID, result.Data.CustomerCode);
                Assert.AreEqual(whscode, result.Data.WHSCode);
                Assert.AreEqual(LoadingStatus.NewJob, result.Data.Status);
                Assert.AreEqual("", result.Data.Currency);
                Assert.IsTrue(result.Data.MixedCurrencies);

                Assert.AreEqual("TruckSeqNo", result.Data.TruckSeqNo);
                Assert.AreEqual("TruckLicencePlate", result.Data.TruckLicencePlate);
                Assert.AreEqual("DockNo", result.Data.DockNo);
                Assert.AreEqual("TrailerNo", result.Data.TrailerNo);

                var details = context.LoadingDetails.Where(l => l.JobNo == jobno).ToList();
                foreach (var ld in details)
                    Assert.AreEqual(now, ld.ETD);

                foreach(var ob in context.Outbounds.Where(o => details.Select(d => d.OutJobNo).Contains(o.JobNo)).ToList())
                    Assert.AreEqual(now, ob.ETD);
            }
        }

        [TestMethod]
        public async Task GetLoadingEntryList()
        {
            var loadingJobNo = "LDGJOB1";
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });
                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER_TAKEN",
                    FactoryID = factoryID,
                    OutJobNo = "OUT_TAKEN",
                    Status = (int)EKanbanStatus.Imported
                });
                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT_TAKEN",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER_TAKEN",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.Loadings.Add(new Core.Entities.Loading
                {
                    JobNo = loadingJobNo,
                    CustomerCode = factoryID,
                    ETD = etd,
                    RefNo = "123",
                    Remark = "remark",
                    WHSCode = whscode,
                    CreatedBy = "user1",
                    CreatedDate = now
                });
                context.LoadingDetails.Add(new Core.Entities.LoadingDetail
                {
                    JobNo = loadingJobNo,
                    ETD = etd,
                    OrderNo = "ORDER_TAKEN",
                    AddedBy = "user1",
                    AddedDate = now,
                    OutJobNo = "OUT_TAKEN",
                    SupplierID = supplierID
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.GetLoadingEntryList(factoryID, whscode);
                Assert.AreEqual(1, result.Count());

                var ld = result.Single();
                Assert.AreEqual("ORDER1", ld.OrderNo);
                Assert.AreEqual(etd, ld.ETD);
                Assert.AreEqual(OutboundStatus.OutStanding, ld.OutboundStatus);
                Assert.AreEqual("OUT1", ld.OutboundJobNo);
                Assert.AreEqual(supplierID, ld.SupplierID);
                Assert.AreEqual("Company1", ld.SupplierName);
            }
        }

        [TestMethod]
        public async Task CreateLoadingDetail()
        {
            var loadingJobNo = "LDGJOB1";
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });

                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.Loadings.Add(new Core.Entities.Loading
                {
                    JobNo = loadingJobNo,
                    CustomerCode = factoryID,
                    ETD = etd,
                    RefNo = "123",
                    Remark = "remark",
                    WHSCode = whscode,
                    CreatedBy = "user1",
                    CreatedDate = now,
                    AllowedForDispatch = true,
                    AllowedForDispatchModifiedDate = DateTime.Now.Date.AddDays(-10),
                    AllowedForDispatchModifiedBy = "otherUser"
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).CreateLoadingDetails(new string[] { "ORDER1" }, loadingJobNo, "user1", whscode);

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var lds = context.LoadingDetails.Where(l => l.JobNo == loadingJobNo).ToList();
                Assert.AreEqual(1, lds.Count());
                var ld = lds[0];
                Assert.AreEqual("ORDER1", ld.OrderNo);
                Assert.AreEqual(etd, ld.ETD);
                Assert.AreEqual(supplierID, ld.SupplierID);
                Assert.AreEqual("OUT1", ld.OutJobNo);
                Assert.AreEqual("user1", ld.AddedBy);
                Assert.IsTrue(now < ld.AddedDate);

                var loading = context.Loadings.Where(l => l.JobNo == loadingJobNo).Single();
                Assert.AreEqual(LoadingStatus.Picked, loading.Status);
                Assert.AreEqual("user1", loading.RevisedBy);
                Assert.IsTrue(now < loading.RevisedDate);

                Assert.AreNotEqual(DateTime.Now.Date.AddDays(-10), loading.AllowedForDispatchModifiedDate);
                Assert.AreEqual("user1", loading.AllowedForDispatchModifiedBy);
                Assert.IsFalse(loading.AllowedForDispatch);

                var outbound = context.Outbounds.Where(o => o.JobNo == "OUT1").Single();
                Assert.AreEqual(etd, outbound.ETD);
            }
        }

        [TestMethod]
        public async Task CreateLoadingFromOutbound_AlreadyUsed()
        {
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });

                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).CreateLoadingFromOutbound(new Models.AddLoadingFromOutboundDto()
                {
                    Loading = new Models.LoadingAddDto
                    {
                        CustomerCode = factoryID,
                        ETD = etd,
                        RefNo = "123",
                        Remark = "remark",
                        WHSCode = whscode,
                    },
                    OutJobNos = new string[] { outjobno1 }
                }, "user1", whscode);

                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task CreateLoadingFromOutbound_TransactionTypeReturn()
        {
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });

                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = OutboundType.Return,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).CreateLoadingFromOutbound(new Models.AddLoadingFromOutboundDto()
                {
                    Loading = new Models.LoadingAddDto
                    {
                        CustomerCode = factoryID,
                        ETD = etd,
                        RefNo = "123",
                        Remark = "remark",
                        WHSCode = whscode,
                    },
                    OutJobNos = new string[] { "OUT1" }
                }, "user1", whscode);

                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task CreateLoadingFromOutbound_StatusCancelled()
        {
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });

                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.Cancelled,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).CreateLoadingFromOutbound(new Models.AddLoadingFromOutboundDto()
                {
                    Loading = new Models.LoadingAddDto
                    {
                        CustomerCode = factoryID,
                        ETD = etd,
                        RefNo = "123",
                        Remark = "remark",
                        WHSCode = whscode,
                    },
                    OutJobNos = new string[] { "OUT1" }
                }, "user1", whscode);

                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task CreateLoadingFromOutbound()
        {
            string loadingJobNo = null;
            var now = DateTime.Now;
            var etd = now.Date.AddDays(10);
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = "ORDER1",
                    FactoryID = factoryID,
                    OutJobNo = "OUT1",
                    Status = (int)EKanbanStatus.Imported
                });

                context.Outbounds.Add(new Core.Entities.Outbound
                {
                    JobNo = "OUT1",
                    Status = OutboundStatus.OutStanding,
                    WHSCode = whscode,
                    TransType = (int)OutboundType.ManualEntry,
                    RefNo = "ORDER1",
                    ETD = etd
                });
                context.OutboundDetails.Add(new Core.Entities.OutboundDetail
                {
                    JobNo = "OUT1",
                    SupplierID = supplierID
                });

                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).CreateLoadingFromOutbound(new Models.AddLoadingFromOutboundDto()
                {
                    Loading = new Models.LoadingAddDto
                    {
                        CustomerCode = factoryID,
                        ETD = etd,
                        RefNo = "123",
                        Remark = "remark",
                        WHSCode = whscode,
                    },
                    OutJobNos = new string[] { "OUT1" }
                }, "user1", whscode);

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsNotNull(result.Data);
                loadingJobNo = result.Data.JobNo;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var loading = context.Loadings.Where(l => l.JobNo == loadingJobNo).Single();
                Assert.AreEqual(LoadingStatus.Picked, loading.Status);
                Assert.AreEqual("user1", loading.RevisedBy);
                Assert.IsTrue(now < loading.RevisedDate);

                var lds = context.LoadingDetails.Where(l => l.JobNo == loadingJobNo).ToList();
                Assert.AreEqual(1, lds.Count());
                var ld = lds[0];
                Assert.AreEqual("ORDER1", ld.OrderNo);
                Assert.AreEqual(etd, ld.ETD);
                Assert.AreEqual(supplierID, ld.SupplierID);
                Assert.AreEqual("OUT1", ld.OutJobNo);
                Assert.AreEqual("user1", ld.AddedBy);
                Assert.IsTrue(now < ld.AddedDate);

                var outbound = context.Outbounds.Where(o => o.JobNo == "OUT1").Single();
                Assert.AreEqual(etd, outbound.ETD);
            }
        }

        [TestMethod]
        public async Task SetTruckArrival()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).SetTruckArrival(jobno);

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                Assert.IsNotNull(ld.TruckArrivalDate);
                Assert.IsTrue(now < ld.TruckArrivalDate);
            }
        }

        [TestMethod]
        public async Task SetTruckDeparture()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).SetTruckDeparture(jobno);

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                Assert.IsNotNull(ld.TruckDepartureDate);
                Assert.IsTrue(now < ld.TruckDepartureDate);
            }
        }

        [TestMethod]
        public async Task SetAllowForDispatch_True()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                ld.AllowedForDispatch = false;
                ld.AllowedForDispatchModifiedDate = DateTime.Now.Date.AddDays(-10);
                ld.AllowedForDispatchModifiedBy = "otherUser";
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).SetAllowForDispatch(jobno, true, "user1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                Assert.IsNotNull(ld.AllowedForDispatchModifiedDate);
                Assert.AreNotEqual(DateTime.Now.Date.AddDays(-10), ld.AllowedForDispatchModifiedDate);
                Assert.AreEqual("user1", ld.AllowedForDispatchModifiedBy);
                Assert.IsTrue(ld.AllowedForDispatch);
            }
        }

        [TestMethod]
        public async Task SetAllowForDispatch_True_NotChanged()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                ld.AllowedForDispatch = true;
                ld.AllowedForDispatchModifiedDate = DateTime.Now.Date.AddDays(-10);
                ld.AllowedForDispatchModifiedBy = "otherUser";
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).SetAllowForDispatch(jobno, true, "user1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var ld = context.Loadings.Where(l => l.JobNo == jobno).Single();
                Assert.AreEqual(DateTime.Now.Date.AddDays(-10), ld.AllowedForDispatchModifiedDate);
                Assert.AreEqual("otherUser", ld.AllowedForDispatchModifiedBy);
                Assert.IsTrue(ld.AllowedForDispatch);
            }
        }

        [TestMethod]
        public async Task GetBondedStockJobNosWithoutCommInv()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);

                var sdBonded = context.StorageDetails.Where(sd => sd.PID == "pid1").Single();
                sdBonded.BondedStatus = (int)BondedStatus.Bonded;
                sdBonded.OutJobNo = outjobno1;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).GetBondedStockJobNosWithoutCommInv(jobno);
                Assert.AreEqual(1, result.Count());
                var ld = result.Single();
                Assert.AreEqual(outjobno1, ld);
            }
        }

        [TestMethod]
        public async Task DeleteLoadingDetail()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetLoadingService(context).DeleteLoadingDetails(new Models.DeleteLoadingDetailDto { JobNo = jobno, OrderNos = new string[] { orderNo1} }, "user1");

                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Assert.IsTrue(result.Data);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var lds = context.LoadingDetails.Where(l => l.JobNo == jobno).ToList();
                Assert.AreEqual(1, lds.Count());
                var ld = lds[0];
                Assert.AreEqual(orderNo2, ld.OrderNo);

                var loading = context.Loadings.Where(l => l.JobNo == jobno).Single();
                Assert.AreEqual(LoadingStatus.Picked, loading.Status);
                Assert.AreEqual("user1", loading.RevisedBy);
                Assert.IsTrue(now < loading.RevisedDate);
            }
        }

        [TestMethod]
        public async Task ConfirmLoading_OutboundJobNotPicked()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var ou = context.Outbounds.ToList().First();
                ou.Status = OutboundStatus.PartialPicked;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.ConfirmLoading(jobno, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task ConfirmLoading()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var ou = context.Outbounds.ToList().First();
                ou.Status = OutboundStatus.Picked;

                // add inventory
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = (int)Ownership.Supplier,
                    OnHandQty = 200,
                    AllocatedQty = 300,
                    OnHandPkg = 2,
                    AllocatedPkg = 3,
                });

                context.InvTransactions.Add(new Core.Entities.InvTransaction
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalancePkg = 100,
                    BalanceQty = 1000
                });

                context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    WHSCode = whscode,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalancePkg = 100,
                    BalanceQty = 1000
                });

                context.InvTransactionsPerSupplier.Add(new Core.Entities.InvTransactionPerSupplier
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Ownership = (int)Ownership.Supplier,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalanceQty = 1000
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.ConfirmLoading(jobno, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var loading = context.Loadings.Find(jobno);
                Assert.AreEqual(LoadingStatus.Confirmed,  loading.Status);
                Assert.AreEqual("user1", loading.ConfirmedBy);
                Assert.IsTrue(now < loading.ConfirmedDate);
            }
        }

        [TestMethod]
        public async Task ConfirmLoading_PerformanceTest()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData_StressTest(context, OutboundStatus.Picked, OutboundStatus.Picked, 100, 100);
                var ou = context.Outbounds.ToList().First();
                ou.Status = OutboundStatus.Picked;

                // add inventory
                context.Inventory.Add(new Core.Entities.Inventory
                {
                    CustomerCode = factoryID,
                    SupplierID = supplierID,
                    ProductCode1 = productCode,
                    WHSCode = whscode,
                    Ownership = (int)Ownership.Supplier,
                    OnHandQty = 20000,
                    AllocatedQty = 30000,
                    OnHandPkg = 2,
                    AllocatedPkg = 3,
                });

                context.InvTransactions.Add(new Core.Entities.InvTransaction
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalancePkg = 100,
                    BalanceQty = 1000000
                });

                context.InvTransactionsPerWHS.Add(new Core.Entities.InvTransactionPerWHS
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    WHSCode = whscode,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalancePkg = 100000,
                    BalanceQty = 1000000
                });

                context.InvTransactionsPerSupplier.Add(new Core.Entities.InvTransactionPerSupplier
                {
                    CustomerCode = factoryID,
                    ProductCode = productCode,
                    SupplierID = supplierID,
                    Ownership = (int)Ownership.Supplier,
                    JobNo = "jobno",
                    SystemDateTime = DateTime.Now,
                    BalanceQty = 1000000
                });
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                Stopwatch sw = new Stopwatch();

                sw.Start();
                var result = await svc.ConfirmLoading(jobno, "user1");
                sw.Stop();
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
                Console.WriteLine($"Took {sw.ElapsedMilliseconds / 1000}s to confirm the loading.");
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var loading = context.Loadings.Find(jobno);
                Assert.AreEqual(LoadingStatus.Confirmed, loading.Status);
                Assert.AreEqual("user1", loading.ConfirmedBy);
                Assert.IsTrue(now < loading.ConfirmedDate);
            }
        }

        [TestMethod]
        public async Task CancelLoading_IncorrectStatus()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var loading = context.Loadings.Find(jobno);
                loading.Status = LoadingStatus.Confirmed;
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.CancelLoading(jobno, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task CancelLoading_LoadingDetailsExist()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.CancelLoading(jobno, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Invalid, result.ResultType);
            }
        }

        [TestMethod]
        public async Task CancelLoading()
        {
            var now = DateTime.Now;
            using (var context = new Context(options, appSettings.Object))
            {
                AddTestData(context, OutboundStatus.PartialPicked, OutboundStatus.Completed);
                var details = context.LoadingDetails.Where(l => l.JobNo == jobno).ToList();
                foreach(var  l in details)
                {
                    context.LoadingDetails.Remove(l);
                }
                context.SaveChanges();
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var svc = GetLoadingService(context);
                var result = await svc.CancelLoading(jobno, "user1");
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var loading = context.Loadings.Find(jobno);
                Assert.AreEqual(LoadingStatus.Cancelled, loading.Status);
                Assert.AreEqual("user1", loading.CancelledBy);
                Assert.IsTrue(now < loading.CancelledDate);
            }
        }

        private void AddTestData(Context context, OutboundStatus status1, OutboundStatus status2)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Loading, Prefix = "LDG", Name = "Loading", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer", WHSCode = whscode });

            context.Loadings.Add(new Core.Entities.Loading
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                Remark = "remark",
                Status = LoadingStatus.NewJob,
                CreatedBy = "user1",
                CreatedDate = DateTime.Now.AddDays(-1),
                ETA = DateTime.Now.Date.AddDays(1),
                ETD = DateTime.Now.Date.AddDays(1).AddHours(4),
                NoOfPallet = 10,
                TruckArrivalDate = DateTime.Now.Date.AddDays(1),
                TruckDepartureDate = DateTime.Now.Date.AddDays(1).AddHours(4),
                TruckLicencePlate = "TruckLicencePlate",
                TrailerNo = "TrailerNo",
                DockNo = "001",
                TruckSeqNo = "Seq1",
                AllowedForDispatch = true
            });

            context.LoadingDetails.Add(new Core.Entities.LoadingDetail
            {
                JobNo = jobno,
                OrderNo = orderNo1,
                SupplierID = supplierID,
                ETD = DateTime.Now.Date,
                AddedBy = "addedBy",
                AddedDate = DateTime.Now,
                OutJobNo = outjobno1
            });

            context.LoadingDetails.Add(new Core.Entities.LoadingDetail
            {
                JobNo = jobno,
                OrderNo = orderNo2,
                SupplierID = supplierID,
                ETD = DateTime.Now.Date,
                AddedBy = "addedBy",
                AddedDate = DateTime.Now,
                OutJobNo = outjobno2
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = orderNo1,
                OutJobNo = outjobno1
            });

            context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
            {
                OrderNo = orderNo2,
                OutJobNo = outjobno2
            });

            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = outjobno1,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = orderNo1,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status = status1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.Outbounds.Add(new Core.Entities.Outbound()
            {
                JobNo = outjobno2,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = orderNo2,
                ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                TransType = OutboundType.EKanban,
                Remark = "Remark",
                Status = status2,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                NoOfPallet = 5
            });

            context.OutboundDetails.Add(new Core.Entities.OutboundDetail()
            {
                JobNo = outjobno1,
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

            context.OutboundDetails.Add(new Core.Entities.OutboundDetail()
            {
                JobNo = outjobno2,
                LineItem = 1,
                ProductCode = productCode,
                SupplierID = supplierID,
                Qty = 100,
                PickedQty = 100,
                Pkg = 2,
                PickedPkg = 2,
                Status = 1,
                CreatedBy = "00013",
                CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
            });

            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = outjobno1,
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
                PID = "pid1"
            });

            context.PickingLists.Add(new Core.Entities.PickingList()
            {
                JobNo = outjobno2,
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
                PID = "pid2"
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "pid1",
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
                InboundDate = DateTime.Now.AddDays(-5),
                Status = StorageStatus.Putaway,
                InJobNo = "injob1"
            });

            context.StorageDetails.Add(new Core.Entities.StorageDetail()
            {
                PID = "pid2",
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
                InboundDate = DateTime.Now.AddDays(-5),
                Status = StorageStatus.Putaway,
                InJobNo = "injob2"
            });

            context.Inbounds.Add(new Core.Entities.Inbound()
            {
                JobNo = "injob1",
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = "ELPS-HT2020072502-507188",
                Status = InboundStatus.Completed,
                Currency = "GBP"
            });

            context.Inbounds.Add(new Core.Entities.Inbound()
            {
                JobNo = "injob2",
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = "ELPS-HT2020072502-507188",
                Status = InboundStatus.Completed,
                Currency = "PLN"
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

        private void AddTestData_StressTest(Context context, OutboundStatus status1, OutboundStatus status2, int count, int count2)
        {
            context.JobCodes.Add(new Core.Entities.JobCode { Code = (int)CodePrefix.Loading, Prefix = "LDG", Name = "Loading", Status = 1 });
            context.Customers.Add(new Core.Entities.Customer() { Code = factoryID, Name = "Customer", WHSCode = whscode });

            context.Loadings.Add(new Core.Entities.Loading
            {
                JobNo = jobno,
                CustomerCode = factoryID,
                WHSCode = whscode,
                RefNo = refno,
                Remark = "remark",
                Status = LoadingStatus.NewJob,
                CreatedBy = "user1",
                CreatedDate = DateTime.Now.AddDays(-1),
                ETA = DateTime.Now.Date.AddDays(1),
                ETD = DateTime.Now.Date.AddDays(1).AddHours(4),
                NoOfPallet = 10,
                TruckArrivalDate = DateTime.Now.Date.AddDays(1),
                TruckDepartureDate = DateTime.Now.Date.AddDays(1).AddHours(4),
                TruckLicencePlate = "TruckLicencePlate",
                TrailerNo = "TrailerNo",
                DockNo = "001",
                TruckSeqNo = "Seq1",
                AllowedForDispatch = true
            });

            for (int i = 0; i < count; i++)
            {
                context.LoadingDetails.Add(new Core.Entities.LoadingDetail
                {
                    JobNo = jobno,
                    OrderNo = orderNo1 + i,
                    SupplierID = supplierID,
                    ETD = DateTime.Now.Date,
                    AddedBy = "addedBy",
                    AddedDate = DateTime.Now,
                    OutJobNo = outjobno1 + i
                });

                context.LoadingDetails.Add(new Core.Entities.LoadingDetail
                {
                    JobNo = jobno,
                    OrderNo = orderNo2 + i,
                    SupplierID = supplierID,
                    ETD = DateTime.Now.Date,
                    AddedBy = "addedBy",
                    AddedDate = DateTime.Now,
                    OutJobNo = outjobno2 + i
                });

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = orderNo1 + i,
                    OutJobNo = outjobno1 + i
                });

                context.EKanbanHeaders.Add(new Core.Entities.EKanbanHeader
                {
                    OrderNo = orderNo2 + i,
                    OutJobNo = outjobno2 + i
                });

                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = outjobno1 + i,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = orderNo1 + i,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType = OutboundType.EKanban,
                    Remark = "Remark",
                    Status = status1,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });

                context.Outbounds.Add(new Core.Entities.Outbound()
                {
                    JobNo = outjobno2 + i,
                    CustomerCode = factoryID,
                    WHSCode = whscode,
                    RefNo = orderNo2 + i,
                    ETD = DateTime.Parse("2020-09-08 17:00:00.000"),
                    TransType = OutboundType.EKanban,
                    Remark = "Remark",
                    Status = status2,
                    CreatedBy = "00013",
                    CreatedDate = DateTime.Parse("2020-09-06 22:13:57.000"),
                    NoOfPallet = 5
                });

                for (int j = 0; j < count2; j++)
                {
                    context.OutboundDetails.Add(new Core.Entities.OutboundDetail()
                    {
                        JobNo = outjobno1 + i,
                        LineItem = j + 1,
                        ProductCode = productCode,
                        SupplierID = supplierID,
                        Qty = 500 + j,
                        PickedQty = 500 + j,
                        Pkg = 4,
                        PickedPkg = 4,
                        Status = 1,
                        CreatedBy = "00013",
                        CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                    });

                    context.OutboundDetails.Add(new Core.Entities.OutboundDetail()
                    {
                        JobNo = outjobno2 + i,
                        LineItem = j + 1,
                        ProductCode = productCode,
                        SupplierID = supplierID,
                        Qty = 100 + j,
                        PickedQty = 100 + j,
                        Pkg = 2,
                        PickedPkg = 2,
                        Status = 1,
                        CreatedBy = "00013",
                        CreatedDate = DateTime.Parse("2020-09-06 22:13:21.000")
                    });

                    context.PickingLists.Add(new Core.Entities.PickingList()
                    {
                        JobNo = outjobno1 + i,
                        LineItem = 1 + j,
                        SeqNo = 1,
                        ProductCode = productCode,
                        SupplierID = supplierID,
                        Qty = 100 + j,
                        WHSCode = whscode,
                        LocationCode = "P4",
                        InboundDate = DateTime.Now.AddDays(-100),
                        PickedBy = "USER1",
                        PickedDate = DateTime.Now.AddDays(-5),
                        PID = $"pid1_{i}_{j}"
                    });

                    context.PickingLists.Add(new Core.Entities.PickingList()
                    {
                        JobNo = outjobno2 + i,
                        LineItem = 1 + j,
                        SeqNo = 1,
                        ProductCode = productCode,
                        SupplierID = supplierID,
                        Qty = 100 + j,
                        WHSCode = whscode,
                        LocationCode = "P4",
                        InboundDate = DateTime.Now.AddDays(-100),
                        PickedBy = "USER1",
                        PickedDate = DateTime.Now.AddDays(-5),
                        PID = $"pid2_{i}_{j}"
                    });

                    context.StorageDetails.Add(new Core.Entities.StorageDetail()
                    {
                        PID = $"pid1_{i}_{j}",
                        LocationCode = "P4",
                        ProductCode = productCode,
                        CustomerCode = factoryID,
                        SupplierID = supplierID,
                        WHSCode = whscode,
                        Qty = 100 + j,
                        OriginalQty = 100 + j,
                        QtyPerPkg = 100 + j,
                        AllocatedQty = 100 + j,
                        OutJobNo = "",
                        InboundDate = DateTime.Now.AddDays(-5),
                        Status = StorageStatus.Putaway,
                        InJobNo = $"injob1_{i}_{j}"
                    });

                    context.StorageDetails.Add(new Core.Entities.StorageDetail()
                    {
                        PID = $"pid2_{i}_{j}",
                        LocationCode = "P4",
                        ProductCode = productCode,
                        CustomerCode = factoryID,
                        SupplierID = supplierID,
                        WHSCode = whscode,
                        Qty = 100 + j,
                        OriginalQty = 100 + j,
                        QtyPerPkg = 100 + j,
                        AllocatedQty = 100 + j,
                        OutJobNo = "",
                        InboundDate = DateTime.Now.AddDays(-5),
                        Status = StorageStatus.Putaway,
                        InJobNo = $"injob2_{i}_{j}"
                    });

                    context.Inbounds.Add(new Core.Entities.Inbound()
                    {
                        JobNo = $"injob1_{i}_{j}",
                        CustomerCode = factoryID,
                        WHSCode = whscode,
                        RefNo = "ELPS-HT2020072502-507188",
                        Status = InboundStatus.Completed,
                        Currency = "GBP"
                    });

                    context.Inbounds.Add(new Core.Entities.Inbound()
                    {
                        JobNo = $"injob2_{i}_{j}",
                        CustomerCode = factoryID,
                        WHSCode = whscode,
                        RefNo = "ELPS-HT2020072502-507188",
                        Status = InboundStatus.Completed,
                        Currency = "PLN"
                    });
                }
            }

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

        private ILoadingService GetLoadingService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            var utilityService = new UtilityService(repository, appSettings.Object);
            var eKanbanService = new EKanbanService(repository, utilityService, appSettings.Object, mapper, locker, new Logger<EKanbanService>(loggerFactory));
            var reportService = new Mock<IReportService>().Object;
            var labelProvider = new Mock<ILabelProvider>().Object;
            var loggerService = new Mock<ILoggerService>().Object;
            var billingService = new BillingService(repository);
            var storageService = new StorageService(repository, labelProvider, appSettings.Object, locker, mapper, new Logger<StorageService>(loggerFactory));
            var iLogConnect = new Mock<IILogConnect>().Object;
            var outboundService = new OutboundService(repository, appSettings.Object, mapper, utilityService, eKanbanService, 
                reportService, logger.Object, storageService, locker, iLogConnect, new Logger<OutboundService>(loggerFactory), billingService);
            return new LoadingService(repository, locker, new Logger<LoadingService>(loggerFactory), mapper, utilityService, 
                appSettings.Object, outboundService, reportService, loggerService);
        }

        private readonly string jobno = "LDG20200900626";

        private readonly string outjobno1 = "OUT20200900626";
        private readonly string outjobno2 = "OUT20200900627";
        private readonly string orderNo1 = "9200906006";
        private readonly string orderNo2 = "9200906007";

        private readonly string supplierID = "SUPPLIER1";
        private readonly string factoryID = "PL1";
        private readonly string whscode = "PL";
        private readonly string refno = "9200909056";
        private readonly string productCode = "PRODCODE1";

    }
}
