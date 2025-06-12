using Application.Common.Models;
using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.InvoiceRequest;
using Application.UseCases.InvoiceRequest.Commands.UploadBatch;
using Application.UseCases.InvoiceRequest.Commands.ValidatePrice;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using Persistence.PetaPoco;
using PetaPoco;
using IMapper = AutoMapper.IMapper;

namespace Persistence.Repositories;

public class InvoiceRequestRepository : IInvoiceRequestRepository
{
    private readonly AppDbContext dbContext;
    private readonly Database database;
    private readonly IMapper mapper;

    public InvoiceRequestRepository(IPPDbContextFactory factory, AppDbContext appDbContext, IMapper mapper)
    {
        database = factory.GetInstance();
        this.dbContext = appDbContext;
        this.mapper = mapper;
    }

    public InvRequestFlow GetFlow()
    {
        var sql = "SELECT TOP 1 StandardFlow, CustomsClearanceFlow FROM InvoiceRequestParameters";
        var result = database.Query<dynamic>(sql).Select(x => ((bool)x.StandardFlow, (bool)x.CustomsClearanceFlow)).SingleOrDefault();
        return result switch
        {
            (true, false) => InvRequestFlow.Standard,
            (false, true) => InvRequestFlow.CustomsClearance,
            _ => InvRequestFlow.None
        };
    }

    public int GetRelevancyThreshold()
    {
        var query = "SELECT TOP 1 RelevancyThreshold FROM InvoiceRequestParameters";
        return database.SingleOrDefault<int>(query);
    }

    public bool GetNoPriceValidationStatus()
    {
        var query = "SELECT TOP 1 NoPriceValidation FROM InvoiceRequestParameters";
        return database.SingleOrDefault<bool>(query);
    }

    public async Task<List<FactoryDto>> GetFactoriesForSupplier(string supplierId)
    {
        var query =
            from f in dbContext.Factories.AsNoTracking()
            join s in dbContext.SupplierMaster on f.FactoryId equals s.FactoryId
            where s.SupplierId == supplierId && s.Status == 1
            orderby f.FactoryId
            select new FactoryDto() { Code = f.FactoryId, Name = f.FactoryName };
        var factories = await query.ToListAsync();
        return factories;
    }

    public async Task<List<FactoryDto>> GetFactoriesForWarehouse(string whsCode)
    {
        var query =
            from f in dbContext.Factories.AsNoTracking()
            join c in dbContext.TtCustomers on f.FactoryId equals c.Code
            where c.Whscode == whsCode && c.Status == 1
            orderby c.Code
            select new FactoryDto() { Code = c.Code, Name = c.Name };
        var factories = await query.ToListAsync();
        return factories;
    }

    public List<JobForSupplier> GetOutboundsEligibleForStandardFlow(int relevancy)
    {
        // TODO consider CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON[dbo].[TT_Outbound]([Status], [TransType], [DispatchedDate]) INCLUDE([CustomerCode])
        var sql = @"
SELECT DISTINCT o.JobNo, t.SupplierID, o.CustomerCode, o.RefNo 'DeliveryDocket', o.ETD FROM TT_Outbound o
JOIN FactoryMaster f on f.FactoryID = o.CustomerCode
JOIN TT_InvTransactionPerSupplier t on t.JobNo = o.JobNo and Ownership = 0
JOIN SupplierMaster s on s.FactoryID = t.CustomerCode and s.SupplierID = t.SupplierID and s.IsBonded = 1
WHERE o.Status = 8
AND o.TransType NOT IN (3, 4) 
AND o.DispatchedDate >= DATEADD(DAY, @relevancy, GETDATE())
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequestBlocklist b WHERE b.JobNo = o.JobNo)
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequest ir WHERE ir.JobNo = o.JobNo)";

        var result = database.Query<JobForSupplier>(sql, new { relevancy }).ToList();
        return result;
    }

    public List<JobForSupplier> GetStockTransfersEligibleForStandardFlow(int relevancy)
    {
        var sql = @"
SELECT * FROM (
-- ongoing
SELECT st.JobNo, std.OriginalSupplierID 'SupplierID', st.CustomerCode FROM TT_StockTransfer st
JOIN FactoryMaster f on f.FactoryID = st.CustomerCode
JOIN TT_StockTransferDetail std on st.JobNo = std.JobNo
JOIN TT_StorageDetail sd on sd.PID = std.PID
WHERE st.Status = 1
AND st.CreatedDate >= DATEADD(DAY, @relevancy, GETDATE())
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequestBlocklist b WHERE b.JobNo = st.JobNo)
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequest ir WHERE ir.JobNo = st.JobNo)
AND (LTRIM(RTRIM(st.CommInvNo)) = '' OR st.CommInvNo IS NULL)
UNION
-- completed
SELECT st.JobNo, t.SupplierID, st.CustomerCode FROM TT_StockTransfer st
JOIN FactoryMaster f on f.FactoryID = st.CustomerCode
JOIN TT_InvTransactionPerSupplier t on t.JobNo = st.JobNo
WHERE st.Status = 8
AND st.CreatedDate >= DATEADD(DAY, @relevancy, GETDATE())
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequestBlocklist b WHERE b.JobNo = st.JobNo)
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequest ir WHERE ir.JobNo = st.JobNo)
AND (LTRIM(RTRIM(st.CommInvNo)) = '' OR st.CommInvNo IS NULL)) src";
        var result = database.Query<JobForSupplier>(sql, new { relevancy }).ToList();
        return result;
    }

    public List<JobForSupplier> GetOutboundsEligibleForCustomsClearanceFlow(int relevancy)
    {
        var sql = @"
SELECT DISTINCT o.JobNo, p.SupplierID, o.CustomerCode, o.RefNo 'DeliveryDocket', o.ETD FROM TT_Outbound o
JOIN FactoryMaster f on f.FactoryID = o.CustomerCode
JOIN TT_PickingList p on p.JobNo = o.JobNo
JOIN SupplierMaster s on s.FactoryID = o.CustomerCode and s.SupplierID = p.SupplierID and s.IsBonded = 1
JOIN TT_StorageDetail sd on sd.PID = (IIF(p.PID IS NOT NULL AND p.PID <> '', p.PID, p.AllocatedPID)) AND sd.Ownership = 0
WHERE o.Status IN (0, 3, 4)
AND o.TransType NOT IN (3, 4) 
AND o.CreatedDate >= DATEADD(DAY, @relevancy, GETDATE())
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequestBlocklist b WHERE b.JobNo = o.JobNo)
AND NOT EXISTS (SELECT JobNo FROM InvoiceRequest ir WHERE ir.JobNo = o.JobNo)
AND EXISTS (SELECT JobNo FROM TT_LoadingDetail ld WHERE ld.OutJobNo = o.JobNo)
AND NOT EXISTS (SELECT JobNo FROM TT_OutboundDetail t WHERE PickedQty < Qty AND t.JobNo = o.JobNo)";
        var result = database.Query<JobForSupplier>(sql, new { relevancy }).ToList();
        return result;
    }

    public JobForSupplier GetJob(string jobNo)
    {
        var query =
            from r in dbContext.InvoiceRequests.AsNoTracking()
            join o in dbContext.TtOutbounds on r.JobNo equals o.JobNo into og
            from o in og.DefaultIfEmpty()
            join s in dbContext.TtStockTransfers on r.JobNo equals s.JobNo into sg
            from s in sg.DefaultIfEmpty()
            where r.JobNo == jobNo
            select new JobForSupplier()
            {
                JobNo = r.JobNo,
                SupplierId = r.SupplierId,
                CustomerCode = r.FactoryId,
                DeliveryDocket = o != null ? o.RefNo : null,
                ETD = o != null ? o.ETD : null
            };
        var result = query.FirstOrDefault() ?? throw new EntityDoesNotExistException();
        return result;
    }

    public List<JobForSupplier> GetStockTransfersEligibleForCustomsClearanceFlow(int relevancy)
        => GetStockTransfersEligibleForStandardFlow(relevancy);

    public async Task<List<ProductLineDto>> GetProductsForInvoiceRequest(params string[] jobs)
    {
        var pallets = dbContext.TtStockTransferDetails
            .Select(s => new { s.JobNo, s.Pid, Qty = (int)(s.Qty ?? 0) })
            .Union(dbContext.TtPickingLists.Select(p => new { p.JobNo, Pid = !string.IsNullOrWhiteSpace(p.Pid) ? p.Pid : p.AllocatedPid!, Qty = (int)p.Qty }));
        var productLines =
            from p in pallets
            join sd in dbContext.TtStorageDetails on p.Pid equals sd.Pid
            join i in dbContext.TtInboundDetails on new { JobNo = sd.InJobNo, sd.LineItem } equals new { i.JobNo, i.LineItem }
            join a in dbContext.AsnDetails on new { i.Asnno, LineItem = i.AsnlineItem ?? 0 } equals new { a.Asnno, a.LineItem }
            let poNo = !string.IsNullOrWhiteSpace(a.Pono) ? a.Pono : null
            let poLine = !string.IsNullOrWhiteSpace(a.PolineNo) ? a.PolineNo : null
            group p by new { p.JobNo, i.ProductCode, InboundJob = i.JobNo, a.Asnno, Pono = poNo, PolineNo = poLine } into g
            orderby g.Key.ProductCode, g.Key.Asnno
            select new { g.Key.JobNo, g.Key.ProductCode, g.Key.InboundJob, g.Key.Asnno, g.Key.Pono, g.Key.PolineNo, Qty = g.Sum(o => o.Qty), PIDCount = g.Count() };
        var query =
            from i in productLines
            where jobs.Contains(i.JobNo)
            select new ProductLineDto()
            {
                JobNo = i.JobNo,
                ProductCode = i.ProductCode,
                Qty = i.Qty,
                PIDCount = i.PIDCount,
                InboundJob = i.InboundJob,
                AsnNo = i.Asnno,
                PoNumber = i.Pono,
                PoLineNo = i.PolineNo
            };

        var result = await query.ToListAsync();
        return result;
    }

    public async Task CreateInvoiceRequest(string factoryId, string supplierId, string jobNo, string refNo, string user, List<ProductLineDto> productLines)
    {
        var ir = new InvoiceRequest()
        {
            FactoryId = factoryId,
            SupplierId = supplierId,
            JobNo = jobNo,
            SupplierRefNo = refNo,
            CreatedBy = user
        };
        dbContext.InvoiceRequests.Add(ir);
        await dbContext.SaveChangesAsync();

        var irp = productLines.Select(p => new InvoiceRequestProduct()
        {
            InvoiceRequestId = ir.Id,
            ProductCode = p.ProductCode,
            InboundJob = p.InboundJob,
            Qty = p.Qty,
            PIDCount = p.PIDCount,
            AsnNo = p.AsnNo,
            PoNumber = p.PoNumber,
            PoLineNo = p.PoLineNo
        });
        dbContext.InvoiceRequestProducts.AddRange(irp);
    }

    public async Task<PaginatedList<InvoiceBatchDto>> GetBatches(GetBatchesQueryFilter filter, PaginationQuery pagination)
    {
        var query = dbContext.InvoiceBatches.AsNoTracking().Where(b => b.FactoryId == filter.FactoryId);

        // filters
        if (filter.SupplierId.IsNotEmpty())
        {
            query = query.Where(b => b.SupplierId == filter.SupplierId);
        }
        if (filter.CreatedDate?.From is { })
        {
            query = query.Where(b => b.CreatedDate >= filter.CreatedDate.From.Value.Date);
        }
        if (filter.CreatedDate?.To is { })
        {
            query = query.Where(b => b.CreatedDate <= filter.CreatedDate.To.Value.Date);
        }
        if (filter.InvoiceNumber.IsNotEmpty())
        {
            var withInvoice =
                from i in dbContext.Invoices
                where i.InvoiceNumber == filter.InvoiceNumber
                select i.InvoiceBatchId;
            query = query.Where(b => withInvoice.Contains(b.Id));
        }
        if (filter.AsnNo.IsNotEmpty())
        {
            var withAsn =
                from r in dbContext.InvoiceRequests
                join bl in dbContext.InvoiceBatchRequestLinks on r.Id equals bl.InvoiceRequestId
                join p in dbContext.InvoiceRequestProducts on r.Id equals p.InvoiceRequestId
                where p.AsnNo == filter.AsnNo
                select bl.InvoiceBatchId;
            query = query.Where(b => withAsn.Contains(b.Id));
        }
        if(filter.PoNo.IsNotEmpty())
        {
            var withPo =
                from r in dbContext.InvoiceRequests
                join bl in dbContext.InvoiceBatchRequestLinks on r.Id equals bl.InvoiceRequestId
                join p in dbContext.InvoiceRequestProducts on r.Id equals p.InvoiceRequestId
                where p.PoNumber == filter.PoNo
                select bl.InvoiceBatchId;
            query = query.Where(b => withPo.Contains(b.Id));
        }
        if(filter.DDSTNumber.IsNotEmpty())
        {
            var withDDST =
                from r in dbContext.InvoiceRequests
                join bl in dbContext.InvoiceBatchRequestLinks on r.Id equals bl.InvoiceRequestId
                where r.SupplierRefNo == filter.DDSTNumber
                select bl.InvoiceBatchId;
            query = query.Where(b => withDDST.Contains(b.Id));
        }
        if (filter.WhsCode.IsNotEmpty())
        {
            var jobs = dbContext.TtStockTransfers
                .Select(s => new { s.JobNo, s.WhsCode })
                .Union(dbContext.TtOutbounds.Select(o => new { o.JobNo, o.WhsCode }));
            var withWhs =
                from r in dbContext.InvoiceRequests
                join j in jobs on r.JobNo equals j.JobNo
                join bl in dbContext.InvoiceBatchRequestLinks on r.Id equals bl.InvoiceRequestId
                where j.WhsCode == filter.WhsCode
                select bl.InvoiceBatchId;
            query = query.Where(b => withWhs.Contains(b.Id));
        }

        var pagedQuery = query
            .OrderByDescending(b => b.RejectedDate == null && b.ApprovedDate == null)
            .ThenByDescending(b => b.Id)
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage);
        var batchesCount = await query.CountAsync();
        var batches = await pagedQuery.ToListAsync();

        if (batchesCount == 0)
        {
            return new PaginatedList<InvoiceBatchDto>(new List<InvoiceBatchDto>(), batchesCount, pagination.PageNumber, pagination.ItemsPerPage);
        }

        // get invoices
        var batchIds = batches.Select(b => b.Id).ToList();
        var invoices =
            from i in dbContext.Invoices.AsNoTracking()
            join f in dbContext.InvoiceFiles on i.Id equals f.InvoiceId into ig
            from f in ig.DefaultIfEmpty()
            where batchIds.Contains(i.InvoiceBatchId)
            orderby i.Id
            select new { i.InvoiceBatchId, i.InvoiceNumber, i.Value, i.Currency, FileId = f != null ? f.Id : (int?)null };
        var invoiceLookup = (await invoices.ToListAsync()).ToLookup(i => i.InvoiceBatchId);

        // get jobs
        var jobItems =
            from b in dbContext.InvoiceBatches.AsNoTracking()
            join bl in dbContext.InvoiceBatchRequestLinks on b.Id equals bl.InvoiceBatchId
            join r in dbContext.InvoiceRequests on bl.InvoiceRequestId equals r.Id
            join rp in dbContext.InvoiceRequestProducts on r.Id equals rp.InvoiceRequestId
            where batchIds.Contains(b.Id)
            select new { BatchId = b.Id, Request = r, Line = rp };
        var jobItemsLookup = (await jobItems.ToListAsync()).ToLookup(i => i.BatchId);

        // get loading ETDs
        var etds =
            from bl in dbContext.InvoiceBatchRequestLinks.AsNoTracking()
            join r in dbContext.InvoiceRequests on bl.InvoiceRequestId equals r.Id
            join ld in dbContext.TtLoadingDetails on r.JobNo equals ld.OutJobNo
            join l in dbContext.TtLoadings on ld.JobNo equals l.JobNo
            where batchIds.Contains(bl.InvoiceBatchId)
            group l by bl.InvoiceBatchId into g
            select new { BatchId = g.Key, Etd = g.Min(l => l.Etd) };
        var etdLookup = (await etds.ToListAsync()).ToDictionary(i => i.BatchId);

        // combine result
        var result =
            from b in batches
            join s in dbContext.SupplierMaster on new { b.FactoryId, b.SupplierId } equals new { s.FactoryId, s.SupplierId }
            join c in dbContext.InvoiceBatchCustoms on b.Id equals c.InvoiceBatchId into cg
            from c in cg.DefaultIfEmpty()
            let isApproved = b.ApprovedDate is { }
            let isRejected = b.RejectedDate is { }
            let status = b switch
            {
                { ApprovedDate: { } } => InvoiceBatchStatus.Approved,
                { RejectedDate: { } } => InvoiceBatchStatus.Rejected,
                _ => InvoiceBatchStatus.PendingApproval
            }
            let inv = invoiceLookup[b.Id].Select(i => new InvoiceDto()
            {
                InvoiceNumber = i.InvoiceNumber,
                Value = i.Value,
                Currency = i.Currency,
                FileId = i.FileId,
            })
            let jobs =
                from j in jobItemsLookup[b.Id]
                group j by new { j.Request.Id, j.Request.JobNo, j.Request.SupplierRefNo } into jg
                orderby jg.Key.JobNo
                let jobNo = jg.Key.JobNo
                select new JobDto()
                {
                    RequestId = jg.Key.Id,
                    Type = jobNo.StartsWith("OUT") ? JobType.Outbound : JobType.StockTransfer,
                    JobNo = jobNo,
                    DeliveryDocket = jg.Key.SupplierRefNo,
                    Details = jg.Select(d => new JobDetailsDto()
                    {
                        AsnNo = d.Line.AsnNo,
                        ProductCode = d.Line.ProductCode,
                        Qty = d.Line.Qty,
                        PONumber = d.Line.PoNumber,
                        POLineNo = d.Line.PoLineNo
                    }).OrderBy(d => d.AsnNo).ThenBy(d => d.ProductCode).ThenBy(d => d.PONumber).ToList()
                }
            select new InvoiceBatchDto()
            {
                BatchId = b.Id,
                BatchNumber = b.BatchNumber,
                SupplierId = b.SupplierId,
                SupplierName = s.CompanyName,
                UploadedOn = b.CreatedDate,
                UploadedBy = b.CreatedBy,
                Status = status.ToString(),
                Comment = c?.Comment,
                TruckDepartureHour = c?.TruckDepartureTime,
                LoadingEtd = etdLookup.ContainsKey(b.Id) ? etdLookup[b.Id].Etd : null,
                Invoices = inv.ToList(),
                Jobs = jobs.ToList()
            };
        var resultList = result.ToList();

        return new PaginatedList<InvoiceBatchDto>(resultList, batchesCount, pagination.PageNumber, pagination.ItemsPerPage);
    }

    public async Task Block(string jobNo, string user)
    {
        var exists = await dbContext.InvoiceRequestBlocklist.FindAsync(jobNo);
        if (exists is null)
        {
            var obj = new InvoiceRequestBlocklist() { JobNo = jobNo, CreatedBy = user };
            dbContext.InvoiceRequestBlocklist.Add(obj);
            await dbContext.SaveChangesAsync();
        }
    }

    public void Unblock(string jobNo)
    {
        var sql = "DELETE FROM InvoiceRequestBlocklist WHERE JobNo = {0}";
        dbContext.Database.ExecuteSqlRaw(sql, jobNo);
    }

    public async Task<Domain.Entities.InvoiceBatch?> GetBatch(int batchId)
    {
        var obj = await dbContext.InvoiceBatches.FindAsync(batchId);
        return obj is { } ? mapper.Map<Domain.Entities.InvoiceBatch>(obj) : null;
    }

    public async Task<List<Domain.Entities.InvoiceRequest>> GetRequests(int batchId)
    {
        var query =
            from bl in dbContext.InvoiceBatchRequestLinks
            join r in dbContext.InvoiceRequests on bl.InvoiceRequestId equals r.Id
            where bl.InvoiceBatchId == batchId
            select r;
        var result = await query.ProjectTo<Domain.Entities.InvoiceRequest>(mapper.ConfigurationProvider).ToListAsync();
        return result;
    }

    public async Task<List<Domain.Entities.InvoiceRequest>> GetRequests(List<int> requestIds)
    {
        var query =
            from r in dbContext.InvoiceRequests
            where requestIds.Contains(r.Id)
            select r;
        var result = await query.ProjectTo<Domain.Entities.InvoiceRequest>(mapper.ConfigurationProvider).ToListAsync();
        return result;
    }

    public async Task Update(Domain.Entities.InvoiceBatch batch)
    {
        var obj = await dbContext.InvoiceBatches.FindAsync(batch.Id) ?? throw new EntityDoesNotExistException();
        obj.ApprovedDate = batch.ApprovedDate;
        obj.ApprovedBy = batch.ApprovedBy;
        obj.RejectedDate = batch.RejectedDate;
        obj.RejectedBy = batch.RejectedBy;
    }

    public async Task Update(Domain.Entities.InvoiceRequest request)
    {
        var obj = await dbContext.InvoiceRequests.FindAsync(request.Id) ?? throw new EntityDoesNotExistException();
        obj.ApprovedBatchId = request.ApprovedBatchId;
    }

    public async Task UpsertCustomsAgencyData(int batchId, int hour, string? comment)
    {
        var obj = await dbContext.InvoiceBatchCustoms.FindAsync(batchId);
        if (obj is { })
        {
            obj.TruckDepartureTime = hour;
            obj.Comment = comment;
        }
        else
        {
            dbContext.InvoiceBatchCustoms.Add(new InvoiceBatchCustomsAgency()
            {
                InvoiceBatchId = batchId,
                TruckDepartureTime = hour,
                Comment = comment
            });
        }
    }

    public async Task<(byte[], string)> GetInvoiceFile(int id)
    {
        var obj = await dbContext.InvoiceFiles.FindAsync(id) ?? throw new EntityDoesNotExistException();
        return (obj.Content, obj.FileName);
    }

    public Domain.Entities.InvoiceRequest? GetRequest(string jobNo)
    {
        var r = dbContext.InvoiceRequests.FirstOrDefault(r => r.JobNo == jobNo);
        return r is { } ? mapper.Map<Domain.Entities.InvoiceRequest>(r) : null;
    }

    public bool IsOnBlocklist(string jobNo)
        => dbContext.InvoiceRequestBlocklist.Any(b => b.JobNo == jobNo);

    public async Task<List<JobDto>> GetOpenRequests(string factoryId, string supplierId)
    {
        // open request is a request with all batches (if any) rejected

        var withNotRejectedBatches =
            from b in dbContext.InvoiceBatches
            join bl in dbContext.InvoiceBatchRequestLinks on b.Id equals bl.InvoiceBatchId
            where b.RejectedDate == null
            select bl.InvoiceRequestId;
        var openRequests =
            from r in dbContext.InvoiceRequests.AsNoTracking()
            where r.SupplierId == supplierId && r.FactoryId == factoryId
            where !withNotRejectedBatches.Contains(r.Id)
            join rp in dbContext.InvoiceRequestProducts on r.Id equals rp.InvoiceRequestId
            select new { r.Id, r.JobNo, r.SupplierRefNo, Line = rp };
        var data = await openRequests.ToListAsync();

        var result = data
            .GroupBy(r => (r.Id, r.JobNo, r.SupplierRefNo))
            .OrderBy(g => g.Key.JobNo)
            .Select(g => new JobDto()
            {
                RequestId = g.Key.Id,
                Type = g.Key.JobNo.StartsWith("OUT") ? JobType.Outbound : JobType.StockTransfer,
                JobNo = g.Key.JobNo,
                DeliveryDocket = g.Key.SupplierRefNo,
                Details = g.Select(d => new JobDetailsDto()
                {
                    AsnNo = d.Line.AsnNo,
                    ProductCode = d.Line.ProductCode,
                    Qty = d.Line.Qty,
                    PONumber = d.Line.PoNumber,
                    POLineNo = d.Line.PoLineNo
                }).OrderBy(d => d.AsnNo).ThenBy(d => d.ProductCode).ThenBy(d => d.PONumber).ToList()
            })
            .ToList();

        return result;
    }

    public async Task<List<PriceInfo>> GetPrices(List<int> requestIds)
    {
        var query =
            from r in dbContext.InvoiceRequests.AsNoTracking()
            join p in dbContext.InvoiceRequestProducts on r.Id equals p.InvoiceRequestId
            join pr in dbContext.Prices on new { r.FactoryId, r.SupplierId, p.ProductCode } equals
                new { FactoryId = pr.CustomerCode, pr.SupplierId, ProductCode = pr.ProductCode1 } into prg
            from pr in prg.DefaultIfEmpty()
            where requestIds.Contains(r.Id)
            select new PriceInfo()
            {
                ProductCode = p.ProductCode,
                Qty = p.Qty,
                Price = pr != null ? pr.SellingPrice : 0,
                Currency = pr != null ? pr.Currency : null
            };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<int> SavePriceValidation(string currency, decimal supplierPrice, decimal ttlogixPrice, bool success, List<int> requests, string user)
    {
        var v = new InvoicePriceValidation()
        {
            Currency = currency,
            InvoiceTotalValue = supplierPrice,
            TtlogixTotalValue = ttlogixPrice,
            Success = success,
            CreatedBy = user
        };
        dbContext.InvoicePriceValidations.Add(v);
        await dbContext.SaveChangesAsync();

        foreach (var r in requests)
        {
            await dbContext.Database.ExecuteSqlRawAsync("INSERT INTO InvoicePriceValidationRequest VALUES ({0}, {1})", v.Id, r);
        }

        return v.Id;
    }

    public async Task<Domain.Entities.PriceValidation?> GetPriceValidation(int priceValidationId)
    {
        var v = await dbContext.InvoicePriceValidations.FindAsync(priceValidationId);
        return v is { } ? mapper.Map<Domain.Entities.PriceValidation>(v) : null;
    }

    public List<int> GetValidationRequestIds(int validationId)
    {
        return dbContext.InvoicePriceValidationRequests
            .Where(v => v.InvoicePriceValidationId == validationId)
            .Select(v => v.InvoiceRequestId)
            .ToList();
    }

    public async Task<string> GetNextBatchNumber(string factoryId, string supplierId)
    {
        var supDetails = dbContext.SupplierDetails.Find(supplierId, factoryId) ?? throw new EntityDoesNotExistException();
        var sequenceNo = supDetails.InvoiceBatchSequenceNo;

        supDetails.InvoiceBatchSequenceNo += 1;
        await dbContext.SaveChangesAsync();

        return $"{supplierId} - #{sequenceNo}";
    }

    public async Task<int> CreateBatch(string batchNumber, string supplierId, string factoryId, string userCode, List<int> requestIds, List<UploadInvoiceDto> invoices, string currency)
    {
        var batch = new InvoiceBatch()
        {
            BatchNumber = batchNumber,
            SupplierId = supplierId,
            FactoryId = factoryId,
            CreatedBy = userCode
        };
        dbContext.InvoiceBatches.Add(batch);
        await dbContext.SaveChangesAsync();

        foreach (var r in requestIds)
        {
            await dbContext.Database.ExecuteSqlRawAsync("INSERT INTO InvoiceBatchRequestLink VALUES ({0}, {1})", r, batch.Id);
        }

        foreach (var i in invoices)
        {
            var inv = new Invoice()
            {
                InvoiceBatchId = batch.Id,
                InvoiceNumber = i.InvoiceNumber,
                Value = i.Value,
                Currency = currency
            };
            dbContext.Invoices.Add(inv);
            await dbContext.SaveChangesAsync();

            var file = new InvoiceFile()
            {
                InvoiceId = inv.Id,
                FileName = i.FileName,
                Content = i.Content
            };
            dbContext.InvoiceFiles.Add(file);
            await dbContext.SaveChangesAsync();
        };

        return batch.Id;
    }

    public void DeletePriceValidation(int validationId)
    {
        dbContext.Database.ExecuteSqlRaw("DELETE FROM InvoicePriceValidationRequest WHERE InvoicePriceValidationID = {0}", validationId);
        dbContext.Database.ExecuteSqlRaw("DELETE FROM InvoicePriceValidation WHERE ID = {0}", validationId);
    }

    public CustomerSupplierDto GetCustomerSupplierData(string factoryId, string supplierId)
    {
        var query =
            from s in dbContext.SupplierMaster.AsNoTracking()
            join f in dbContext.Factories on s.FactoryId equals f.FactoryId
            where s.FactoryId == factoryId && s.SupplierId == supplierId
            select new CustomerSupplierDto()
            {
                FactoryId = factoryId,
                FactoryName = f.FactoryName,
                SupplierId = supplierId,
                CompanyName = s.CompanyName
            };
        return query.FirstOrDefault() ?? throw new EntityDoesNotExistException();
    }

    public async Task<List<NamedStream>> GetInvoiceFiles(int batchId)
    {
        var query =
            from i in dbContext.Invoices.AsNoTracking()
            join f in dbContext.InvoiceFiles on i.Id equals f.InvoiceId
            where i.InvoiceBatchId == batchId
            select f;
        var result = await query.ToListAsync();
        var files = result
            .Select(r => new NamedStream() { FileName = r.FileName, Content = new MemoryStream(r.Content) })
            .ToList();
        return files;
    }

    public (string departureHour, string? comment)? GetCustomsInfo(int batchId)
    {
        var info = dbContext.InvoiceBatchCustoms.FirstOrDefault(c => c.InvoiceBatchId == batchId);
        return info != null ? ($"{info.TruckDepartureTime}:00", info.Comment) : null;
    }

    public async Task DeleteInvoiceFiles(int batchId)
    {
        var query =
            from i in dbContext.Invoices
            join f in dbContext.InvoiceFiles on i.Id equals f.InvoiceId into fg
            from f in fg.DefaultIfEmpty()
            where i.InvoiceBatchId == batchId
            select f.Id;
        var fileIds = await query.ToListAsync();
        dbContext.Database.ExecuteSqlRaw($"DELETE FROM InvoiceFile WHERE ID IN ({string.Join(", ", fileIds)})");
    }

    public async Task UpdateJobCommercialInvNumber(int batchId, List<string> jobs)
    {
        var invoiceNumbers = await dbContext.Invoices.Where(i => i.InvoiceBatchId == batchId).Select(i => i.InvoiceNumber).ToListAsync();
        var invNo = string.Join("|||", invoiceNumbers);

        var outs = jobs.Where(j => j.StartsWith("OUT")).ToList();
        var stfs = jobs.Except(outs).ToList();

        if (outs.Any())
        {
            var outParam = outs.Select(j => $"'{j}'");
            dbContext.Database.ExecuteSqlRaw($"UPDATE TT_Outbound SET CommInvNo = {{0}} WHERE JobNo IN ({string.Join(", ", outParam)})", invNo);
        }
        if (stfs.Any())
        {
            var stfParam = stfs.Select(j => $"'{j}'");
            dbContext.Database.ExecuteSqlRaw($"UPDATE TT_StockTransfer SET CommInvNo = {{0}} WHERE JobNo IN ({string.Join(", ", stfParam)})", invNo);
        }
    }

    public void AddForCustomsAgencyIntegration(string jobNo)
    {
        dbContext.Database.ExecuteSqlRaw("INSERT INTO dbi.YusenCustomsIntegration (JobNo) VALUES ({0})", jobNo);
    }
}
