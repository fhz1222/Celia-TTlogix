using Application.UseCases.StockTransfer.Queries.GetStockTransferPalletsInILog;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// StockTransferController provides methods to manage stock transfers
/// </summary>
public class StockTransferController : ApiControllerBase
{
    /// <summary>
    /// Gets pallets on stock transfer job which are on iLog Storage locations
    /// </summary>
    /// <param name="jobNo"></param>
    /// <returns></returns>
    [HttpGet("getPalletsInILog")]
    public async Task<IEnumerable<string>> GetPalletsInILog(string jobNo)
    {
        return await Mediator.Send(new GetStockTransferPalletsInILogQuery()
        {
            JobNo = jobNo
        });
    }
}
