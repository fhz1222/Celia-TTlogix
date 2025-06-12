using System.Collections.Generic;
using System.Threading.Tasks;

namespace TT.Services.Interfaces
{
    public interface IILogConnect
    {
        void InboundCompleted(string jobNo);
        Task<bool> IsProcessingOutbound(string jobNo);
        void QuarantineJobCreated(string jobNo);
        void StockTransferCompleted(string jobNo);
        void PidAddedToStockTransfer(IEnumerable<string> pids);
        void PidRemovedFromStockTransfer(IEnumerable<string> pids);
    }
}
