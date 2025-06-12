using Application.Interfaces.Repositories;
using Domain.Entities;
using Persistence.Extensions;
using Persistence.PetaPoco;
using Persistence.PetaPoco.Models;
using PetaPoco;
using SqlKata;
using Application.Exceptions;
using PetaPoco.SqlKata;
using Persistence.Entities;
using Application.UseCases.AdjustmentItems;
using Domain.ValueObjects;
using Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;
using Application.UseCases.Adjustments.Queries.GetAdjustmentPalletsInILog;

namespace Persistence.Repositories
{
    public class AdjustmentItemRepository: IAdjustmentItemRepository
    {
        private readonly Database dbContext;
        private readonly AppDbContext context;
        private readonly AutoMapper.IMapper mapper;
        public AdjustmentItemRepository(IPPDbContextFactory factory, AppDbContext context, AutoMapper.IMapper mapper)
        {
            dbContext = factory.GetInstance();
            this.context = context;
            this.mapper = mapper;
        }
        public List<AdjustmentItem> GetAdjustmentItems(string jobNo, string? orderBy, bool orderByDescending)
        {
            var query = new Query(TT_InvAdjustmentDetail.SqlTableName)
                .Join(TT_StorageDetail.SqlTableName, j => j.On($"{TT_InvAdjustmentDetail.SqlTableName}.{nameof(TT_InvAdjustmentDetail.PID)}", $"{TT_StorageDetail.SqlTableName}.{nameof(TT_StorageDetail.PID)}"))
                .Join(TT_PartMaster.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.{nameof(TT_StorageDetail.ProductCode)}", $"{TT_PartMaster.SqlTableName}.{nameof(TT_PartMaster.ProductCode1)}")
                    .On($"{TT_StorageDetail.SqlTableName}.{nameof(TT_StorageDetail.CustomerCode)}", $"{TT_PartMaster.SqlTableName}.{nameof(TT_PartMaster.CustomerCode)}")
                    .On($"{TT_StorageDetail.SqlTableName}.{nameof(TT_StorageDetail.SupplierId)}", $"{TT_PartMaster.SqlTableName}.{nameof(TT_PartMaster.SupplierId)}"))
                .Where(nameof(TT_InvAdjustmentDetail.JobNo), jobNo);


            query.Select(TT_InvAdjustmentDetail.SqlTableName + ".{*}",
                   TT_StorageDetail.SqlTableName + ".{SupplierId}");

            string orderByColumnName = PrepareOrderByClause(orderBy);

            if (!orderByDescending)
                query.OrderBy(orderByColumnName);
            else
                query.OrderByDesc(orderByColumnName);
            return dbContext.Fetch<TT_InvAdjustmentDetail, TT_StorageDetail, AdjustmentItem>((adjD, stD) => mapper.Map<AdjustmentItem>(adjD).SetSupplierId(stD.SupplierId), query.ToSql());
        }

        
        private string PrepareOrderByClause(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
               return "LineItem";
            }
            string orderByResult = orderBy;
            bool addAdjustmentTablePrefix = true;
            switch(orderBy.ToLower())
            {
                case "palletid": orderByResult = "PID"; break;
                case "initialqty": orderByResult = "OldQty"; break;
                case "remarks": orderByResult = "Remark"; break;
                case "supplierid": orderByResult = $"{TT_StorageDetail.SqlTableName}.SupplierId"; addAdjustmentTablePrefix = false; break;
                case "productcode":
                case "newqty": break;
                default: throw new UnknownOrderByExpressionException();
            }

            if (!addAdjustmentTablePrefix)
                return orderByResult;
            return $"{TT_InvAdjustmentDetail.SqlTableName}.{orderByResult}";
        }
        public AdjustmentItemWithPalletDto? GetAdjustmentItemDetails(string jobNo, int lineItem)
        {
            var query = new Query(TT_InvAdjustmentDetail.SqlTableName)
                .Join(TT_StorageDetail.SqlTableName, j => j.On($"{TT_InvAdjustmentDetail.SqlTableName}.PID", $"{TT_StorageDetail.SqlTableName}.PID"))
                .Join(TT_PartMaster.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.ProductCode", $"{TT_PartMaster.SqlTableName}.ProductCode1")
                    .On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_PartMaster.SqlTableName}.CustomerCode")
                    .On($"{TT_StorageDetail.SqlTableName}.SupplierID", $"{TT_PartMaster.SqlTableName}.SupplierID"))
                .Where($"{TT_InvAdjustmentDetail.SqlTableName}.JobNo", jobNo)
                .Where($"{TT_InvAdjustmentDetail.SqlTableName}.LineItem", lineItem);

            query.Select(TT_InvAdjustmentDetail.SqlTableName + ".*",
                   TT_StorageDetail.SqlTableName + ".*",
                   TT_PartMaster.SqlTableName + ".*");

            var foundObj = dbContext.Query<TT_InvAdjustmentDetail, TT_StorageDetail, TT_PartMaster, (TT_InvAdjustmentDetail, TT_StorageDetail, TT_PartMaster)>((a, b, c) => new(a, b, c), query.ToSql()).FirstOrDefault();

            if (foundObj.Item1?.JobNo == null || foundObj.Item1?.LineItem == null)
                return null;

            // map main fields from inv adjustment detail 
            var result = mapper.Map<AdjustmentItemWithPalletDto>(foundObj.Item1);
            // map other fields from storage detail and part master
            result.Pallet = mapper.Map<Pallet>(foundObj.Item2);
            result.Pallet.Product = mapper.Map<Product>(foundObj.Item3);
            return result;
        }

        public int GetLastLineItemNumber(string jobNo)
        {
            return dbContext.Single<int>($@"SELECT ISNULL(MAX(LineItem), 0) FROM TT_InvAdjustmentDetail WHERE JobNo = '{jobNo}'");
        }

        public async Task Update(AdjustmentItem updatedObject, string userCode, DateTime dateTime)
        {
            var existingObject = await context.TtInvAdjustmentDetails.FindAsync(updatedObject.JobNo, updatedObject.LineItem);
            if (existingObject == null)
                throw new EntityDoesNotExistException();

            mapper.Map(updatedObject, existingObject);
            existingObject.RevisedBy = userCode;
            existingObject.RevisedDate = dateTime;
        }

        public async Task AddNew(AdjustmentItem updatedObject, string userCode, DateTime dateTime)
        {
            var existingObject = await context.TtInvAdjustmentDetails.FindAsync(updatedObject.JobNo, updatedObject.LineItem);
            if (existingObject != null)
                throw new EntityAlreadyExistsException();

            var newObject = mapper.Map<TtInvAdjustmentDetail>(updatedObject);
            newObject.CreatedBy = userCode;
            newObject.CreatedDate = dateTime;
            context.TtInvAdjustmentDetails.Add(newObject);
        }

        public async Task Delete(string jobNo, int lineItem)
        {
            var deletedObject = await context.TtInvAdjustmentDetails.FindAsync(jobNo, lineItem);
            if (deletedObject == null)
                throw new EntityDoesNotExistException();
            context.TtInvAdjustmentDetails.Remove(deletedObject);
        }

        public bool PalletAppearsInAdjustment(string jobNo, string palletId)
        {
            return context.TtInvAdjustmentDetails.Where(i => i.JobNo == jobNo && i.Pid == palletId).Any();
        }

        public bool PalletAppearsInOutgoingAdjustment(string palletId)
        {
            return context.TtInvAdjustmentDetails.Where(i => i.Pid == palletId)
                .Join(context.TtInvAdjustmentMasters
                    , item => item.JobNo
                    , adjustment => adjustment.JobNo
                    , (item, adjustment) => adjustment)
                .Where(a => a.Status == (byte) InventoryAdjustmentStatus.Processing || a.Status == (byte)InventoryAdjustmentStatus.New)
                .Any();
        }

        public IEnumerable<AdjustmentItemSummaryDto> GetAdjustmentItemGroupedData(string jobNo)
        {
            return context.TtInvAdjustmentDetails.Where(i => i.JobNo == jobNo)
                .Join(context.TtStorageDetails
                , adjItem => adjItem.Pid
                , storageDetails => storageDetails.Pid
                , (adjItem, storageDetails) => new { AdjustmentItem = adjItem, StorageDetails = storageDetails })
                .ToList()
                .GroupBy(d => new { d.AdjustmentItem.JobNo, d.AdjustmentItem.ProductCode, d.StorageDetails.CustomerCode, d.StorageDetails.SupplierId, d.StorageDetails.Ownership })
                .Select(g => new AdjustmentItemSummaryDto
                {
                    JobNo = g.Key.JobNo,
                    ProductCode = g.Key.ProductCode,
                    CustomerCode = g.Key.CustomerCode ?? "",
                    SupplierId = g.Key.SupplierId,
                    Ownership = (Ownership)g.Key.Ownership,
                    OldQty = (int)g.Sum(d => d.AdjustmentItem.OldQty),
                    OldQtyPerPkg = (int)g.Sum(d => d.AdjustmentItem.OldQtyPerPkg),
                    NewQty = (int)g.Sum(d => d.AdjustmentItem.NewQty),
                    NewQtyPerPkg = (int)g.Sum(d => d.AdjustmentItem.NewQtyPerPkg),
                    TotalDifferentPkg = g.Sum(d => d.AdjustmentItem.GetDiffPkg())
                })
                .ToList();
        }

        public IEnumerable<AdjustmentItemSummaryByProductDto> GetAdjustmentItemByProductGroupedData(string jobNo)
        {
            return context.TtInvAdjustmentDetails.Where(i => i.JobNo == jobNo)
                .Join(context.TtStorageDetails
                , adjItem => adjItem.Pid
                , storageDetails => storageDetails.Pid
                , (adjItem, storageDetails) => new { AdjustmentItem = adjItem, StorageDetails = storageDetails })
                .ToList()
                .GroupBy(d => new { d.AdjustmentItem.JobNo, d.AdjustmentItem.ProductCode, d.StorageDetails.CustomerCode})
                .Select(g => new AdjustmentItemSummaryByProductDto
                {
                    JobNo = g.Key.JobNo,
                    ProductCode = g.Key.ProductCode,
                    CustomerCode = g.Key.CustomerCode ?? "",
                    OldQty = (int)g.Sum(d => d.AdjustmentItem.OldQty),
                    OldQtyPerPkg = (int)g.Sum(d => d.AdjustmentItem.OldQtyPerPkg),
                    NewQty = (int)g.Sum(d => d.AdjustmentItem.NewQty),
                    NewQtyPerPkg = (int)g.Sum(d => d.AdjustmentItem.NewQtyPerPkg),
                    TotalDifferentPkg = g.Sum(d => d.AdjustmentItem.GetDiffPkg())
                })
                .ToList();
        }

        public IEnumerable<AdjustmentPalletDto> GetAdjustmentPalletsOnLocationCategory(string jobNo, int locationCategoryId)
        {
            var query = new Query(TT_StorageDetail.SqlTableName)
                .Join(TT_InvAdjustmentDetail.SqlTableName, j => j.On($"{TT_StorageDetail.SqlTableName}.PID", $"{TT_InvAdjustmentDetail.SqlTableName}.PID"))
                .Join(TT_PartMaster.SqlTableName, j => j
                    .On($"{TT_StorageDetail.SqlTableName}.CustomerCode", $"{TT_PartMaster.SqlTableName}.CustomerCode")
                    .On($"{TT_StorageDetail.SqlTableName}.SupplierId", $"{TT_PartMaster.SqlTableName}.SupplierId")
                    .On($"{TT_StorageDetail.SqlTableName}.ProductCode", $"{TT_PartMaster.SqlTableName}.ProductCode1"))
                .Join("TT_Location", j => j.On($"{TT_StorageDetail.SqlTableName}.LocationCode", "TT_Location.Code").On($"{TT_StorageDetail.SqlTableName}.WHSCode", "TT_Location.WHSCode"))
                .Where($"{TT_InvAdjustmentDetail.SqlTableName}.JobNo", jobNo)
                .Where("TT_Location.ILogLocationCategoryId", locationCategoryId)
                .Select($"{TT_StorageDetail.SqlTableName}.PID")
                .Select($"{TT_InvAdjustmentDetail.SqlTableName}.OldQty", $"{TT_InvAdjustmentDetail.SqlTableName}.NewQty")
                .Select($"{TT_PartMaster.SqlTableName}.IsCPart", $"{TT_PartMaster.SqlTableName}.CPartSPQ as CPartBoxQty")
                .ToSql();

            return dbContext.Query<AdjustmentPalletDto>(query);
        }
    }
}
