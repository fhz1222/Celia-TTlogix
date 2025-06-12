using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using Persistence.PetaPoco;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.Repositories
{
    public class PalletTransferRequestsRepository : IPalletTransferRequestsRepository
    {
        private readonly AppDbContext context;

        public CodePrefix GetCodePrefix => CodePrefix.PalletTransferRequest;

        public PalletTransferRequestsRepository(IPPDbContextFactory factory, AppDbContext context)
        {
            this.context = context;
        }

        public void Add(PalletTransferRequest palletTransferRequest)
        {
            context.Add(new TtPalletTransferRequest
            {
                JobNo = palletTransferRequest.JobNo,
                CompletedOn = palletTransferRequest.CompletedOn,
                CreatedBy = palletTransferRequest.CreatedBy,
                CreatedOn = palletTransferRequest.CreatedOn,
                PID = palletTransferRequest.PID
            });
        }

        public async Task<PalletTransferRequest?> Get(string jobNo)
        {
            var entity = await context.TtPalletTransferRequests.FirstOrDefaultAsync(x => x.JobNo == jobNo);
            return entity == null ? null : Map(entity);
        }

        public int GetLastJobNumber(string prefix)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = $@"SELECT ISNULL(MAX(CAST(RIGHT(JobNo,5) AS INTEGER)),0) AS NumRecord FROM TT_PalletTransferRequest WHERE JobNo LIKE '{prefix}%'";
            cmd.CommandType = CommandType.Text;
            return (int)cmd.ExecuteScalar();
        }

        public async Task Update(PalletTransferRequest ptr)
        {
            var entity = await context.TtPalletTransferRequests.FirstAsync(x => x.JobNo == ptr.JobNo);
            entity.CompletedOn = ptr.CompletedOn;
        }

        public async Task<IEnumerable<PalletTransferRequest>> GetOngoing()
        {
            return (await context.TtPalletTransferRequests.AsNoTracking().Where(p => p.CompletedOn == null).ToListAsync())
                .Select(Map);
        }

        private PalletTransferRequest Map(TtPalletTransferRequest ptr)
            => new(ptr.JobNo, ptr.PID, ptr.CreatedOn, ptr.CreatedBy, ptr.CompletedOn);
    }
}
