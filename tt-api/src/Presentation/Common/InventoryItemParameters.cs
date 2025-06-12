using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

/// <summary>
/// This class is used by InventoryController to specify required parameters, filters, sorting and pagination requirements
/// for the inventory item list. CustomerCode, Paging have to be always given 
/// </summary>
public class InventoryItemParameters
{
    /// <summary>
    /// ownership filter; 1 for EHP, 0 for Supplier
    /// </summary>
    public int? Ownership { get; set; }
    /// <summary>
    /// customer code - mandatory filter
    /// </summary>
    public string CustomerCode { get; set; } = null!;
    /// <summary>
    /// supplier id filter - optional
    /// </summary>
    public string? SupplierId { get; set; }
    /// <summary>
    /// product code - optional
    /// it matches full or fragment of product code
    /// </summary>
    public string? ProductCode { get; set; }
    /// <summary>
    /// this filter is applied to the OnHandQty field;
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange OnHandQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the QuarantineQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange QuarantineQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the AllocatedQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange AllocatedQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the IncomingQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange IncomingQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the PickableQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange PickableQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the BondedQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange BondedQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// this filter is applied to the NonBondedQty field
    /// it is possible to specify <b>from</b> value, or <b>to</b> value or <b>from - to</b> range
    /// </summary>
    public DtoFilterIntRange NonBondedQty { get; set; } = new DtoFilterIntRange();
    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



