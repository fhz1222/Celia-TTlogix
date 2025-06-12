using Application.Common.Enums;
using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;
/// <summary>
/// </summary>
public class DecantItemDto
{
    ///<summary>
    /// Decant job number
    /// </summary>
    public string JobNo { get; set; } = null!;
    /// <summary>
    /// Pallet identifier for decant 
    /// </summary>
    public string PID { get; set; } = null!;
    ///<summary>
    /// table of new quantities - it determines how to split old pallet 
    ///</summary>
    public int[] NewQuantities { get; set; } = new int[0];
}



