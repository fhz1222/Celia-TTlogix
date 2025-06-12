using Application.Interfaces.Repositories;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using PetaPoco;
using PetaPoco.SqlKata;
using SqlKata;

namespace Persistence.Repositories;

public class StockTransferRepository : IStockTransferRepository
{
    private readonly Database dbContext;

    public StockTransferRepository(IPPDbContextFactory factory)
    {
        dbContext = factory.GetInstance();
    }

    public IEnumerable<string> GetStockTransferPalletsOnLocationCategory(string jobNo, int locationCategoryId)
    {
        var query = new Query(TT_StorageDetail.SqlTableName)
            .Join("TT_StockTransferDetail", j => j.On($"{TT_StorageDetail.SqlTableName}.PID", $"TT_StockTransferDetail.PID"))
            .Join("TT_Location", j => j.On($"{TT_StorageDetail.SqlTableName}.LocationCode", "TT_Location.Code").On($"{TT_StorageDetail.SqlTableName}.WHSCode", "TT_Location.WHSCode"))
            .Where($"TT_StockTransferDetail.JobNo", jobNo)
            .Where("TT_Location.ILogLocationCategoryId", locationCategoryId)
            .Select($"{TT_StorageDetail.SqlTableName}.PID")
            .ToSql();

        return dbContext.Query<string>(query);
    }
}
