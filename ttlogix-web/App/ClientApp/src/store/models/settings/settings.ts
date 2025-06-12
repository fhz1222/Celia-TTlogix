import { Area } from "./area";
import { AreaType } from "./areaType";
import { ControlCode } from "./controlCode";
import { PackageType } from "./packageType";
import { ProductCode } from "./productCode";
import { SatoPrinter } from "./satoPrinter";
import { UomSettings } from "./uom";
import { Warehouse } from "./warehouse";

export class Settings {
    constructor() {
        this.area = new Array<Area>();
        this.areaType = new Array<AreaType>();
        this.controlCode = new Array<ControlCode>();
        this.packageType = new Array<PackageType>();
        this.productCode = new Array<ProductCode>();
        this.satoPrinter = new Array<SatoPrinter>();
        this.uom = new Array<UomSettings>();
        this.warehouse = new Array<Warehouse>();
    }

    area: Array<Area>;
    areaType: Array<AreaType>;
    controlCode: Array<ControlCode>;
    packageType: Array<PackageType>;
    productCode: Array<ProductCode>;
    satoPrinter: Array<SatoPrinter>;
    uom: Array<UomSettings>;
    warehouse: Array<Warehouse>;
}