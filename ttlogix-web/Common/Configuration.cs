using System;
using System.Collections.Generic;
using System.Linq;

namespace TT.Common
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string OwnerCode { get; set; }

        public string[] SAPFactories { get; set; }

        public int MaxLogResponseLength { get; set; }

        public bool IsILogEnabled {  get; set; }

        public IDictionary<string, WarehouseSettings> warehouse {get; set;}

        public WarehouseSettings defaultWarehouse { get; set;} = new WarehouseSettings();

        public ModuleSettings EnabledModules {get; set;} = new ModuleSettings();

        public WarehouseSettings Warehouse(string Code) {
            if (warehouse.ContainsKey(Code)) {
                return warehouse[Code];
            }

            return defaultWarehouse;
        }

        public bool IsSAPFactory(string factory) => SAPFactories.Contains(factory);
        public bool IsNotSAPFactory(string factory) => !IsSAPFactory(factory);
    }

    public class ModuleSettings {
        public bool Inbound {get; set;} = true;
        
        public bool Outbound {get; set;} = true;

        public bool PartMaster {get; set;} = true;

        public bool Loading {get; set ;} = true;
    }

    public class WarehouseSettings {
        public bool FIFO {get; set;} = true;

        public bool EKanban {get; set;} = true;

        public bool OverdueReport {get; set;} = false;

        public bool ASN {get; set;} = true;


    }
}
