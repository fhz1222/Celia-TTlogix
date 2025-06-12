using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;

namespace TT.Services.Tests
{
    [TestClass]
    public class UserManagementServiceTests : TestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task GetPrivilegesTree()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddData(context);
                context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L2A", Status = 1 });
                context.SaveChanges();
            }
            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetUserManagementService(context).GetPrivilegesTree("G1");

                Assert.AreEqual(2, result.Children.Count());
                Assert.IsTrue(new string[] { "L1A", "L1B" }.Intersect(result.Children.Select(c => c.Code)).Count() == 2);
                var L1B = result.Children.Where(c => c.Code == "L1B").Single();
                Assert.IsFalse(L1B.IsChecked);
                Assert.AreEqual(0, L1B.Children.Count());
                var L1A = result.Children.Where(c => c.Code == "L1A").Single();
                Assert.IsFalse(L1A.IsChecked);
                Assert.AreEqual(2, L1A.Children.Count());
                var L2A = L1A.Children.Where(c => c.Code == "L2A").Single();
                Assert.IsTrue(L2A.IsChecked);
                var L2B = L1A.Children.Where(c => c.Code == "L2B").Single();
                Assert.IsFalse(L2B.IsChecked);
                Assert.AreEqual(0, L2A.Children.Count());
                Assert.AreEqual(1, L2B.Children.Count());
                var L3B = L2B.Children.Where(c => c.Code == "L3B").Single();
                Assert.IsFalse(L3B.IsChecked);
                Assert.AreEqual(0, L3B.Children.Count());
            }
        }

        [TestMethod]
        public async Task UpdatePrivilegesTree()
        {
            using (var context = new Context(options, appSettings.Object))
            {
                AddData(context);
                context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L1A", Status = 1 });
                context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L1B", Status = 1 });
                context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L2A", Status = 1 });
                context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L2B", Status = 1 });

                context.SaveChanges();
            }
            SystemModuleTreeDto tree = null;
            using (var context = new Context(options, appSettings.Object))
            {
                tree = await GetUserManagementService(context).GetPrivilegesTree("G1");
                tree.Children.First().IsChecked = false;
                tree.Children.Last().IsChecked = false;
            }

            using (var context = new Context(options, appSettings.Object))
            {
                var result = await GetUserManagementService(context).UpdatePrivilegesTree("G1", tree);
                Assert.AreEqual(ServiceResult.ResultType.Ok, result.ResultType);
            }

            using (var context = new Context(options, appSettings.Object))
            {
                Assert.AreEqual(6, context.AccessRights.Count());
                Assert.AreEqual(3, context.AccessRights.Where(a => a.Status == 0).Count());
                var actives = context.AccessRights.Where(a => a.Status == 1).ToList();
                Assert.IsTrue(actives.All(a => a.GroupCode == "G1"));
                Assert.AreEqual(3, actives.Select(a => a.ModuleCode).Intersect(new string[] { "L0", "L2A", "L2B" }).Count());
            }
        }

        private IUserManagementService GetUserManagementService(Context context)
        {
            var repository = new SqlTTLogixRepository(context, new Mock<IRawSqlExecutor>().Object);
            var reportService = new Mock<IReportService>().Object;
            var locker = new Locker();
            var loggerFactory = new LoggerFactory();
            return new UserManagementService(repository, mapper, reportService, locker, new Logger<UserManagementService>(loggerFactory));
        }
        private void AddData(Context context)
        {
            //              L0
            //      L1A             L1B
            //  L2A     L2B
            //          L3B
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L0", ParentCode = "", ModuleName = "L0", ShortName = "L0" });
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L1A", ParentCode = "L0", ModuleName = "L1A", ShortName = "L1A" });
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L1B", ParentCode = "L0", ModuleName = "L1B", ShortName = "L1B" });
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L2A", ParentCode = "L1A", ModuleName = "L2A", ShortName = "L2A" });
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L2B", ParentCode = "L1A", ModuleName = "L2B", ShortName = "L2B" });
            context.SystemModules.Add(new Core.Entities.SystemModule { Code = "L3B", ParentCode = "L2B", ModuleName = "L3B", ShortName = "L3B" });

            context.AccessGroups.Add(new Core.Entities.AccessGroup { Code = "G1", Description = "G1", Status = Core.Enums.ValueStatus.Active, CreatedBy = "User", CreatedDate = DateTime.Now });
            context.AccessRights.Add(new Core.Entities.AccessRight { GroupCode = "G1", ModuleCode = "L0", Status = 1 });

            context.SaveChanges();
        }


    }
}
