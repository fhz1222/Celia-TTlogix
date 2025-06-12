using Application.Common.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Extensions;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using Persistence.PetaPoco.Extensions;
using Application.Exceptions;
using Persistence.Entities;
using Application.UseCases.Decants;
using Application.UseCases.Decants.Queries.GetDecants;
using PetaPoco;
using Domain.Enums;

namespace Persistence.Repositories
{
    
    public class DecantRepository : IDecantRepository
    {
        private readonly Database dbContext;
        private readonly AppDbContext context;
        private readonly AutoMapper.IMapper mapper;

        public CodePrefix GetCodePrefix => CodePrefix.Decant;

        public DecantRepository(IPPDbContextFactory factory, AppDbContext context, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.context = context;
            this.mapper = mapper;
        }

        public int GetLastJobNumber(string prefix)
        {
            return dbContext.Single<int>($@"SELECT ISNULL(MAX(CAST(RIGHT(JobNo,5) AS INTEGER)),0) AS NumRecord FROM {TT_Decant.SqlTableName} WHERE JobNo LIKE '{prefix}%'");
        }

        public PaginatedList<DecantDto> GetDecants(DecantDtoFilter filter,
                                                   PaginationQuery pagination,
                                                   string? orderBy,
                                                   bool orderByDescending)
        {
            
            var query = new SqlKata.Query(TT_Decant.SqlTableName)
                .Join(TT_Customer.SqlTableName, j => j.On($"{TT_Decant.SqlTableName}.CustomerCode", "Code")
                    .On($"{TT_Decant.SqlTableName}.WHSCode", $"{TT_Customer.SqlTableName}.WHSCode"));

            filter.ApplyFilter(query);

            query.Select(TT_Decant.SqlTableName + ".{CustomerCode, WhsCode, JobNo, RefNo, CreatedDate, Status, Remark}",
                   TT_Customer.SqlTableName + ".{Name}");

            string orderByColumnName = PrepareOrderByClause(orderBy);

            if (!orderByDescending)
                query.OrderBy(orderByColumnName);
            else
                query.OrderByDesc(orderByColumnName);

            return dbContext.PagedFetch<TT_Decant, TT_Customer, (TT_Decant, TT_Customer)>(pagination.PageNumber, pagination.ItemsPerPage,
                (adj, cust) => new(adj, cust), query)
                .ToPaginatedList(am => mapper.Map<DecantDto>(am.Item1).SetCustomerName(am.Item2.Name));
        }

        private string PrepareOrderByClause(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
               return "JobNo";
            }
            string orderByResult = orderBy;
            bool addAdjustmentTablePrefix = true;
            switch(orderBy.ToLower())
            {
                case "referenceno": orderByResult = "RefNo"; break;
                case "customername": orderByResult = $"{TT_Customer.SqlTableName}.Name"; addAdjustmentTablePrefix = false; break;
                case "status":
                case "jobno":
                case "customercode":
                case "whscode":
                case "createddate":
                case "remark": break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addAdjustmentTablePrefix)
                return orderByResult;
            return $"{TT_Decant.SqlTableName}.{orderByResult}";
        }

        public async Task<Decant?> GetDecant(string jobNo)
        {
            var ttDecant = await context.FindAsync<TtDecant>(jobNo);
            if (ttDecant == null)
                return null;
            var decant = mapper.Map<Decant>(ttDecant);
            var customerData = await context.FindAsync<TtCustomer>(decant.CustomerCode, decant.WhsCode);

            decant.CustomerName = customerData?.Name ?? "";

            var decantData = context.TtDecantPkgs.Where(pkg => pkg.JobNo == decant.JobNo)
                .Join(context.TtPartMasters
                    , pkg => new {pkg.ProductCode, pkg.SupplierId, decant.CustomerCode }
                    , parts => new {ProductCode = parts.ProductCode1, parts.SupplierId, parts.CustomerCode}
                    , (pkg, parts) => pkg)
                .Join(context.TtDecantDetails
                    , pkg => new { pkg.JobNo, pkg.Pid }
                    , detail => new { detail.JobNo, Pid = detail.ParentId }
                    , (pkg, detail) => new { Pkg = pkg, Detail = detail })
                .ToList()
                .GroupBy(data => new { data.Pkg.JobNo, data.Pkg.Pid });
            foreach(var decantPkgWithDetail in decantData)
            {
                var decantItem = mapper.Map<DecantItem>(decantPkgWithDetail.First().Pkg);
                foreach(var decantDetail in decantPkgWithDetail.OrderBy(i => i.Detail.SeqNo))
                {
                    decantItem.NewPallets.Add(mapper.Map<DecantItemPallet>(decantDetail.Detail));
                }
                decant.Items.Add(decantItem);
            }
            return decant;
        }

        public async Task UpdateDecant(Decant updated)
        {
            var decant = await context.FindAsync<TtDecant>(updated.JobNo);
            if (decant == null)
                throw new EntityDoesNotExistException();

            decant.RefNo = updated.ReferenceNo ?? "";
            decant.Remark = updated.Remark ?? "";
            decant.Status = (byte) updated.Status;
            decant.CancelledBy = updated.CancelledBy ?? "";
            decant.CancelledDate = updated.CancelledDate;
        }

        public async Task AddNewDecant(Decant newObject)
        {
            var decant = mapper.Map<TtDecant>(newObject);
            
            context.TtDecants.Add(decant);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDecantItem(string jobNo, DecantItem decantItem)
        {
            DeleteDecantDetails(jobNo, decantItem.SourcePalletId);

            var decantPkg = await context.TtDecantPkgs.FindAsync(jobNo, decantItem.SourcePalletId);
            if (decantPkg == null)
                throw new EntityDoesNotExistException();
            context.TtDecantPkgs.Remove(decantPkg);
        }

        public async Task AddDecantItem(string jobNo, string userCode, DecantItem decantItem)
        {
            var decantPkg = new TtDecantPkg
            {
                JobNo = jobNo,
                Pid = decantItem.SourcePalletId,
                Qty = decantItem.SourceQty,
                CreatedBy = userCode,
                CreatedDate = DateTime.Now
            };
            mapper.Map(decantItem.NewPallets.First(), decantPkg);
            context.Add(decantPkg);
            AddNewDecantDetails(jobNo, decantItem.SourcePalletId, decantItem.NewPallets);
        }

        private void DeleteDecantDetails(string jobNo, string pid)
        {
            var decantDetails = context.TtDecantDetails.Where(dd => dd.JobNo == jobNo && dd.ParentId == pid).ToList();
            context.TtDecantDetails.RemoveRange(decantDetails);
        }

        private void AddNewDecantDetails(string jobNo, string pid, ICollection<DecantItemPallet> itemPallets )
        {
            foreach (var decantItemPallet in itemPallets)
            {
                var decantDetail = mapper.Map<TtDecantDetail>(decantItemPallet);
                decantDetail.JobNo = jobNo;
                decantDetail.ParentId = pid;
                // not filled yet - it will be filled on Complete action
                decantDetail.Pid = String.Empty;
                context.Add(decantDetail);
            }
        }

        public async Task UpdateDecantItemOnComplete(string jobNo, DecantItem decantItem)
        {
            foreach (var decantItemPallet in decantItem.NewPallets)
            {
                var decantDetail = await context.FindAsync<TtDecantDetail>(jobNo, decantItem.SourcePalletId, decantItemPallet.SequenceNo);
                if (decantDetail == null)
                    throw new EntityDoesNotExistException();
                decantDetail.Pid = decantItemPallet.PalletId;
            }
        }
    }
}
