using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;
/// <summary>
/// This class is used by QuarantineController to specify required parameters, filters, sorting and pagination requirements
/// for the quarantine list. Paging has to be always given 
/// </summary>
public class QuarantineParameters
{
    /// <summary>
    /// Specifies customer code - optional
    /// </summary>
    public string? CustomerCode { get; set; } = null!;
    /// <summary>
    /// Specifies supplier identifier filter - optional <br/>
    /// It can be whole or a part of supplier identifier - the method checks if supplierId in quarantined pallet contains the specified value
    /// </summary>
    public string? SupplierId { get; set; }
    /// <summary>
    /// Specifies product code filter - optional<br/>
    /// It can be whole or a part of customer code - the method checks if product code in quarantined pallet contains the specified value
    /// </summary>
    public string? ProductCode { get; set; }
    /// <summary>
    /// Location filter - optional<br/>
    /// It can be whole or a part of location code - the method checks if location in quarantined pallet contains the specified value
    /// </summary>
    public string? Location { get; set; }
    /// <summary>
    /// Reason filter - optional<br/>
    /// It matches a fragment or a whole text with quarantine reason
    /// </summary>
    public string? Reason { get; set; }
    /// <summary>
    /// Pallet identifier filter - optional<br/>
    /// It can be a whole or a part of pallet identifier 
    /// </summary>
    public string? PID { get; set; }
    /// <summary>
    /// Created by filter - optional<br/>
    /// It filters data based on code of user who created the quarantine. It can be a whole or a part of user code
    /// </summary>
    public string? CreatedBy { get; set; }
    /// <summary>
    /// Quarantine date filter - optional<br/>
    /// It is possible to specify <b>date from</b>, <b>date to</b> or <b>range of dates</b> to filter quaratine data by creation date
    /// </summary>
    public DtoFilterDateTimeRange? QuarantineDate { get; set; }
    /// <summary>
    /// Quantity filter - optional<br/>
    /// It is possible to specify <b>value from</b>, <b>value to</b> or <b>range of values</b>
    /// </summary>
    public DtoFilterIntRange? Qty { get; set; }
    /// <summary>
    /// DecimalNum filter - optional<br/>
    /// It is possible to specify <b>value from</b>, <b>value to</b> or <b>range of values</b>
    /// </summary>
    public DtoFilterIntRange? DecimalNum { get; set; }

    /// <summary>
    /// Specifies requested page number and size of the page
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



