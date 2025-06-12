using Application.Exceptions;
using Domain.ValueObjects;

namespace Application.UseCases.StorageDetails;

public class StorageDetailItemDto
{
    public Domain.Entities.Product Product { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public Ownership Ownership { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PID { get; set; } = null!;
    public string? ExternalPID { get; set; }
    public string InboundJobNo { get; set; } = null!;
    public string OutboundJobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int SequenceNo { get; set; }
    public DateTime? InboundDate { get; set; }
    public string? ControlCode1 { get; set; }
    public string? ControlCode2 { get; set; }
    public string? ParentId { get; set; }
    public StorageStatus Status { get; set; } = null!;
    public bool BondedStatus { get; set; }
    public string? RefNo { get; set; }
    public string? CommInvNo { get; set; }
    public int Qty { get; set; }
    public int AllocatedQty { get; set; }
    public string ILogLocationCategory { get; set; }

    // TODO perhaps this behavior could be exported out to some service, with help of reflection (on generic)?
    public static Func<StorageDetailItemDto, object?>? GetOrderByFunction(string? by)
    {
        switch (by?.ToLower())
        {
            case null: return null;
            case "supplierid": return (i) => i.Product.CustomerSupplier.SupplierId;
            case "customercode": return (i) => i.Product.CustomerSupplier.CustomerCode;
            case "productcode": return (i) => i.Product.Code;
            case "ownership": return (i) => (byte)i.Ownership;
            case "location": return (i) => i.Location;
            case "pid": return (i) => i.PID;
            case "externalpid": return (i) => i.ExternalPID;
            case "inboundjobno": return (i) => i.InboundJobNo;
            case "outboundjobno": return (i) => i.OutboundJobNo;
            case "lineitem": return (i) => i.LineItem;
            case "sequenceno": return (i) => i.SequenceNo;
            case "controlcode1": return (i) => i.ControlCode1;
            case "inbounddate": return (i) => i.InboundDate;
            case "controlcode2": return (i) => i.ControlCode2;
            case "parentid": return (i) => i.ParentId;
            case "status": return (i) => (byte)i.Status;
            case "bondedstatus": return (i) => i.BondedStatus;
            case "refno": return (i) => i.RefNo;
            case "comminvno": return (i) => i.CommInvNo ?? "";
            case "qty": return (i) => i.Qty;
            case "allocatedqty": return (i) => i.AllocatedQty;
            default: throw new UnknownOrderByExpressionException($"OrderBy expression '{by}' was not recognized");
        }
    }
}