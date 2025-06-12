using Domain.Entities;

namespace Application.Interfaces.Utils;

public interface IInventoryTransactionService
{
    Task GenerateInventoryTransactionsOnAdjustmentComplete(string jobNo);
    Task GenerateInventoryTransactionsOnDecantComplete(Decant decant);
}
