using Application.Common.Enums;
using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;
/// <summary>
/// This class is used by RelocationController to specify filters, sorting and pagination requirements
/// for the relocation log list. Paging have to be always given and one of these filters: relocation dates (up to 3 months period), PID or external PID 
/// </summary>
public class RelocationLogParameters
{
    /// <summary>
    /// Pallet identifier filter 
    /// </summary>
    public string? PID { get; set; }
    /// <summary>
    /// External pallet identifier filter 
    /// </summary>
    public string? ExternalPID { get; set; }
    /// <summary>
    /// Specifies supplier identifier filter - optional <br/>
    /// It can be whole or a part of supplier identifier - the method checks if the supplierId of relocated pallet contains the specified value
    /// </summary>
    public string? SupplierId { get; set; }
    /// <summary>
    /// Specifies product code filter - optional<br/>
    /// It can be whole or a part of customer code - the method checks if the product code of relocated pallet contains the specified value
    /// </summary>
    public string? ProductCode { get; set; }
    /// <summary>
    /// Old warehouse filter - optional<br/>
    /// It can be whole or a part of the warehouse code - the method checks if the old warehouse code of relocation log contains the specified value
    /// </summary>
    public string? OldWhsCode { get; set; }
    /// Old location filter - optional<br/>
    /// It can be whole or a part of the location code - the method checks if the old location code of relocation log contains the specified value
    /// </summary>    
    public string? OldLocation { get; set; }
    /// <summary>
    /// New warehouse filter - optional<br/>
    /// It can be whole or a part of the warehouse code - the method checks if the new warehouse code of relocation log contains the specified value
    /// </summary>
    public string? NewWhsCode { get; set; }
    /// New location filter - optional<br/>
    /// It can be whole or a part of the location code - the method checks if the location location code of relocation log contains the specified value
    /// </summary>
    public string? NewLocation { get; set; }
    /// <summary>
    /// Scanner type filter - optional<br/>
    /// Possible values:<br/>
    /// <b> 0 </b> - Batch Scanner<br/>
    /// <b> 1 </b> - RF Scanner
    /// </summary>
    public int? ScannerType { get; set; }
    /// <summary>
    /// Relocated by filter - optional<br/>
    /// It filters data based on code of user who relocated the pallet. It can be a whole or a part of user code
    /// </summary>
    public string? RelocatedBy { get; set; }
    /// <summary>
    /// Relocation date filter - optional<br/>
    /// It is possible to specify <b>date from</b>, <b>date to</b> or <b>range of dates</b> to filter relocation logs by relocation date
    /// </summary>
    public DtoFilterDateTimeRange? RelocationDate { get; set; }
    /// <summary>
    /// Customer Code filter - optional; the complete code must be provided
    /// </summary>
    public string? CustomerCode { get; set; }
    /// <summary>
    /// Quantity filter - optional<br/>
    /// It is possible to specify <b>value from</b>, <b>value to</b> or <b>range of values</b>
    /// </summary>
    public DtoFilterIntRange? Qty { get; set; }
    /// <summary>
    /// Specifies requested page number and size of the page
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



