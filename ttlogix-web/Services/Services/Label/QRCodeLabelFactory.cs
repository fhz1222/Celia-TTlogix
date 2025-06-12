using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Interfaces;
using TT.Services.Interfaces;

namespace TT.Services.Label
{
    public class QRCodeLabelFactory : ILabelFactory
    {
        Action<string[]> print;
        private readonly ITTLogixRepository repository;
        private List<string> data = new List<string>();

        public QRCodeLabelFactory(ITTLogixRepository repository, Action<string[]> print)
        {
            this.print = print;
            this.repository = repository;
        }

        private const char DELIMITER = '\u0005';

        public async Task AddLabel(StorageDetail pid, ILabelFactory.LabelType type)
        {
            var partMaster = await repository.PartMasters().Where(x => x.ProductCode1 == pid.ProductCode && x.SupplierID == pid.SupplierID && x.CustomerCode == pid.CustomerCode).FirstAsync();

            data.Add(
                new QRCodeBuilder("<S1>", DELIMITER.ToString())
                .Append(pid.PID, 20)
                .Append(pid.ProductCode, 30)
                .Append(partMaster != null ? partMaster.ProductCode2 : "", 30)
                .Append(pid.InboundDate.ToString("ddMMyyyy"), 8)
                .Append(pid.Qty.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")), 25)
                .Append(pid.ControlDate.HasValue ? pid.ControlDate.Value.ToString("ddMMyyyy") : "", 8)
                .Append(pid.ControlCode1, 30)
                .Append(pid.ControlCode2, 30)
                .Append(pid.ControlCode3, 30)
                .Append(pid.Version.ToString(), 4)
                .Append(pid.SupplierID, 10)
                .Append(pid.CustomerCode, 10)
                .ToString());
        }

        public Task AddLabel(StorageDetailGroup group, ILabelFactory.LabelType type)
        {
            data.Add(
                new QRCodeBuilder("<SG>", DELIMITER.ToString())
               .Append(group.GroupID, 20)
               .Append(group.Name, 30)
               .Append("", 30)
               .Append(group.CreatedDate.ToString("ddMMyyyy"), 8)
               .ToString());
            return Task.CompletedTask;
        }

        public Task AddLabel(Location location)
        {
            var locTypeInt = location.Type switch
            {
                Core.Enums.LocationType.Normal => 0,
                Core.Enums.LocationType.Quarantine => 1,
                Core.Enums.LocationType.CrossDock => 2,
                Core.Enums.LocationType.Standby => 3,
                _ => 0,
            };

            data.Add(
                new QRCodeBuilder("<L1>", DELIMITER.ToString())
               .Append(location.Code, 15)
               .Append(location.WHSCode, 7)
               .Append(locTypeInt.ToString(), 1)
               .ToString());
            return Task.CompletedTask;
        }

        public void Init(LabelPrinter printer)
        {
        }

        public Task Print(int copies)
        {
            print(data.ToArray());
            return Task.CompletedTask;
        }
    }
}
