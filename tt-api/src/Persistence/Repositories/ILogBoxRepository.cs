using Application.Interfaces.Repositories;
using Application.UseCases.ILogIntegration;
using Microsoft.EntityFrameworkCore;
using Persistence.PetaPoco;
using PetaPoco;

namespace Persistence.Repositories;

public class ILogBoxRepository : IILogBoxRepository
{
    private readonly Database dbContext;
    private readonly AppDbContext efContext;

    public ILogBoxRepository(IPPDbContextFactory factory, AppDbContext efContext)
    {
        dbContext = factory.GetInstance();
        this.efContext = efContext;
    }

    public void DeletePalletBoxes(string palletId)
    {
        var sql = "DELETE FROM dbi.iLogIntegrationBox WHERE MasterPalletId = {0}";
        efContext.Database.ExecuteSqlRaw(sql, palletId);
    }

    public void DeleteBoxes(string[] boxIds)
    {
        if (boxIds.Length == 0) { return; }

        foreach (var boxIdChunk in boxIds.ChunkCollectionForSqlServer())
        {
            var sql = $"DELETE FROM dbi.iLogIntegrationBox WHERE BoxId IN ({string.Join(',', boxIdChunk.Select(box => $"'{box}'"))})";
            efContext.Database.ExecuteSqlRaw(sql);
        }
    }

    public void DeleteAllBoxes()
    {
        // TRUNCATE TABLE cannot be used here - temporal table
        efContext.Database.ExecuteSqlRaw("DELETE FROM dbi.iLogIntegrationBox");
    }

    public void CreateBox(string palletId, int qty)
    {
        // EF Core has strange issues with computed PK columns - insert done manually
        var sql = "INSERT INTO dbi.iLogIntegrationBox(MasterPalletId, Qty) VALUES ({0}, {1})";
        efContext.Database.ExecuteSqlRaw(sql, palletId, qty);
    }

    public List<BoxDto> GetBoxes(string palletId)
    {
        return dbContext.Query<BoxDto>("SELECT * FROM dbi.iLogIntegrationBox WHERE MasterPalletId = @0", palletId).ToList();
    }

    public List<BoxDto> GetBoxes(string[] palletIds)
    {
        if (palletIds.Length == 0) { return new List<BoxDto>(); }

        var sql = "SELECT * FROM dbi.iLogIntegrationBox WHERE MasterPalletId IN (@palletIdChunk)";
        var result = new List<BoxDto>();
        foreach (var palletIdChunk in palletIds.ChunkCollectionForSqlServer())
        {
            result.AddRange(dbContext.Query<BoxDto>(sql, new { palletIdChunk }));
        }

        return result;
    }

    public BoxDto? GetBox(string boxId)
    {
        return dbContext.FirstOrDefault<BoxDto?>("SELECT * FROM dbi.iLogIntegrationBox WHERE BoxId = @0", boxId);
    }

    public void UpdateBoxes(BoxDto[] boxes)
    {
        foreach (var box in boxes)
        {
            var sql = "UPDATE dbi.iLogIntegrationBox SET Qty = {0} WHERE BoxId = {1}";
            efContext.Database.ExecuteSqlRaw(sql, box.Qty, box.BoxId);
        }
    }

    public bool AreBoxesGenerated()
    {
        return dbContext.FirstOrDefault<bool?>("SELECT TOP 1 1 FROM dbi.iLogIntegrationBox") ?? false;
    }
}
