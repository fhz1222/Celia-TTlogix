using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Label;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class StorageGroupService : ServiceBase<StorageGroupService>, IStorageGroupService
    {
        public StorageGroupService(ITTLogixRepository repository,
            ILocker locker,
            ILogger<StorageGroupService> logger,
            IUtilityService utilityService,
            ILabelProvider labelProvider, IStorageService storageService)
            : base(locker, logger)
        {
            this.repository = repository;
            this.utilityService = utilityService;
            this.labelProvider = labelProvider;
            this.storageService = storageService;
        }

        public async Task<Result<string[]>> CreateGroup(string whsCode, int num, string prefix)
        {
            return await WithTransactionScope<string[]>(async () =>
            {
                var list = new List<string>();
                for (var i = 1; i <= num; i++)
                {
                    var entity = new StorageDetailGroup();
                    entity.CreatedDate = DateTime.Now;
                    entity.Quantity = 0;
                    entity.Name = prefix + " " + i;
                    entity.WHSCode = whsCode;
                    entity.GroupID = await utilityService.GetNextGroupPIDNumber();
                    list.Add(entity.GroupID);

                    await repository.AddStorageDetailGroupAsync(entity);
                }

                return new SuccessResult<string[]>(list.ToArray());
            });
        }

        public async Task<Result<bool>> DeleteGroup(string GroupID)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var list = new List<string>();
                var group = await repository.GetStorageDetailGroupAsync(GroupID);
                if (group != null && group.Status == 0)
                {
                    await repository.DeleteStorageDetailGroupAsync(group);
                }


                return new SuccessResult<bool>(true);
            });
        }

        public async Task<Result<bool>> TransformGroup(string GroupID)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var list = new List<string>();
                var group = await repository.GetStorageDetailGroupAsync(GroupID);
                var pids = await repository.StorageDetails().Where(x => x.GroupID == GroupID).OrderBy(x => x.PID).Where(x => x.Status == StorageStatus.Putaway).ToListAsync();
                if (pids.Count == 0)
                {
                    return new InvalidResult<bool>(new JsonResultError("ListIsEmpty").ToJson());
                }
                var pid = pids.First();
                var product = await repository.GetPartMasterAsync(pid.CustomerCode, pid.SupplierID, pid.ProductCode);
                if (product.ProductCode4 == null || product.ProductCode4 == "")
                {
                    return new InvalidResult<bool>(new JsonResultError("EmptyTargetSPQ").ToJson());
                }
                var target = Decimal.Parse(product.ProductCode4);
                if (target == product.SPQ)
                {
                    return new InvalidResult<bool>(new JsonResultError("IdenticalSPQ").ToJson());
                }
                if (target < 1)
                {
                    return new InvalidResult<bool>(new JsonResultError("TooSmallSPQ").ToJson());
                }
                pids = await repository.StorageDetails().Where(x => x.Qty != target
                                                                 && x.SupplierID == pid.SupplierID
                                                                 && x.ProductCode == pid.ProductCode
                                                                 && x.LocationCode == pid.LocationCode
                                                                 && x.WHSCode == pid.WHSCode
                                                                 && pid.CustomerCode == x.CustomerCode
                                                                 && x.Qty > 0
                                                                 && x.Status == StorageStatus.Putaway).OrderBy(x => x.Qty).OrderBy(x => x.PID).ToListAsync();
                var gids = new List<string>();
                decimal qty = 0;
                foreach (var p in pids)
                {
                    if (p.GroupID != null && !gids.Contains(p.GroupID))
                    {
                        gids.Add(p.GroupID);
                    }
                    p.GroupID = null;
                    qty += p.Qty;
                }
                decimal sum = qty;
                group.RepackedDate = DateTime.Now;
                while (sum >= target)
                {
                    sum -= target;
                    var tmp = target;
                    StorageDetail p = null;
                    while (tmp > 0)
                    {
                        p = pids[0];
                        if (p.Qty > tmp)
                        {
                            p.Qty -= tmp;
                            tmp = 0;
                            p.GroupID = null;
                            await repository.SaveChangesAsync();
                        }
                        else
                        {
                            tmp -= p.Qty;
                            p.Qty = 0;
                            p.Status = StorageStatus.Splitted;
                            await repository.SaveChangesAsync();
                            pids = pids.Skip(1).ToList();
                        }



                    }

                    var pidNumber = await utilityService.GetNextPIDNumber();
                    var n = new StorageDetail()
                    {
                        PID = pidNumber,
                        InJobNo = p.InJobNo,
                        LineItem = p.LineItem,
                        SeqNo = p.SeqNo,
                        ParentID = p.PID,
                        CustomerCode = p.CustomerCode,
                        ProductCode = p.ProductCode,
                        InboundDate = p.InboundDate,
                        ControlDate = p.ControlDate,
                        OriginalQty = target,
                        Qty = target,
                        QtyPerPkg = target,
                        NoOfLabel = p.NoOfLabel,
                        Length = p.Length,
                        Width = p.Width,
                        Height = p.Height,
                        NetWeight = p.NetWeight,
                        GrossWeight = p.GrossWeight,
                        ControlCode1 = p.ControlCode1,
                        ControlCode2 = p.ControlCode2,
                        ControlCode3 = p.ControlCode3,
                        ControlCode4 = p.ControlCode4,
                        ControlCode5 = p.ControlCode5,
                        ControlCode6 = p.ControlCode6,
                        LocationCode = p.LocationCode,
                        GroupID = GroupID,
                        WHSCode = p.WHSCode,
                        Status = StorageStatus.Putaway,
                        SupplierID = p.SupplierID,
                        IsVMI = p.IsVMI,
                        BondedStatus = p.BondedStatus,
                        Ownership = p.Ownership,
                        PutawayBy = p.PutawayBy,
                        PutawayDate = p.PutawayDate,
                        BuyingPrice = p.BuyingPrice,
                        SellingPrice = p.SellingPrice
                    };

                    group.RepackedDate = DateTime.Now;
                    await repository.AddStorageDetailAsync(n);
                    await repository.SaveChangesAsync();
                }

                foreach (var gid in gids)
                {
                    var gr = await repository.GetStorageDetailGroupAsync(gid);
                    pids = await repository.StorageDetails().Where(x => x.GroupID == gid).ToListAsync();
                    gr.Quantity = pids.Count();
                    gr.RepackedDate = DateTime.Now;
                    await repository.SaveChangesAsync();
                }

                return new SuccessResult<bool>(true);
            });

        }

        public async Task<StorageGroupListDto> GetGroupList(string whsCode, StorageGroupListQueryFilter filter)
        {

            var query = repository.GetStorageDetailGroupList<StorageGroupDto>(whsCode, filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new StorageGroupListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };

        }
        public async Task<Result<bool>> PrintLabels(string[] GroupID, ILabelFactory.LabelType labelType, string IP, int copies)
        {
            var labelPrinter = await repository.LabelPrinters().Where(x => x.IP == IP).FirstAsync();
            var factory = labelProvider.CreateFactory(labelPrinter);
            var groups = await repository.StorageDetailGroups().Where(x => GroupID.Contains(x.GroupID)).OrderBy(x => x.GroupID).ToListAsync();
            if (groups.Count == 0)
            {
                return new InvalidResult<bool>(new JsonResultError("ListIsEmpty").ToJson());
            }

            foreach (var item in groups)
            {
                await factory.AddLabel(item, labelType);
            }
            try
            {
                await factory.Print(copies);
            }
            catch (PrinterUnavailableException)
            {
                return new InvalidResult<bool>(new JsonResultError("PrinterIsUnavailable").ToJson());
            }

            return new SuccessResult<bool>(true);
        }

        public async Task<Result<bool>> PrintPIDLabels(string[] GroupIDs, ILabelFactory.LabelType labelType, string IP)
        {
            var labelPrinter = await repository.LabelPrinters().Where(x => x.IP == IP).FirstAsync();
            var factory = labelProvider.CreateFactory(labelPrinter);
            var pids = await repository.StorageDetails().Where(x => GroupIDs.Contains(x.GroupID)).OrderBy(x => x.PID).Where(x => x.Status != StorageStatus.Splitted && x.Status != StorageStatus.Dispatched).ToListAsync();
            if (pids.Count == 0)
            {
                return new InvalidResult<bool>(new JsonResultError("ListIsEmpty").ToJson());
            }

            foreach (var item in pids)
            {
                await factory.AddLabel(item, labelType);
            }
            try
            {
                await factory.Print(1);
            }
            catch (PrinterUnavailableException)
            {
                return new InvalidResult<bool>(new JsonResultError("PrinterIsUnavailable").ToJson());
            }

            return new SuccessResult<bool>(true);
        }

        public async Task<IEnumerable<AllocatedStorageDetailSummaryQueryResult>> GetStorageDetails([RequiredAsJsonError] string groupId)
        {
            return await repository.GetStorageDetailSummaryListForGroup(groupId);
        }

        public async Task<Result<IEnumerable<StorageLabelDto>>> GetStorageLabelsForGIDs(string[] gids)
        {
            var pids = await repository.StorageDetails().Where(sd => gids.Contains(sd.GroupID)).Select(sd => sd.PID).ToArrayAsync();
            return await storageService.GetStorageLabels(pids);
        }

        public async Task<Result<IEnumerable<GroupLabelDto>>> GetGroupLabels(string[] gids)
        {
            var groups = await repository.StorageDetailGroups().Where(x => gids.Contains(x.GroupID)).OrderBy(x => x.GroupID).ToListAsync();

            List<string> data = new List<string>();

            var factory = new QRCodeLabelFactory(repository, (string[] res) => data = new List<string>(res));
            foreach (var g in groups)
            {
                await factory.AddLabel(g, ILabelFactory.LabelType.SMALL);
            }
            await factory.Print(1);

            IEnumerable<QRCodeDto> codes = data.Select((d, i) => new QRCodeDto { Code = d, Name = groups[i].GroupID });
            IEnumerable<GroupLabelDto> labels = Enumerable.Empty<GroupLabelDto>();

            foreach (var g in groups)
            {
                var label = new GroupLabelDto()
                {
                    Code = codes.SingleOrDefault(c => c.Name == g.GroupID),
                    Qty = g.Quantity,
                    CreatedDate = g.CreatedDate,
                    Gid = g.GroupID,
                    Name = g.Name,
                    WHSCode = g.WHSCode
                };
                labels = labels.Append(label);
            }
            return new SuccessResult<IEnumerable<GroupLabelDto>>(labels);
        }

        private readonly ITTLogixRepository repository;
        private readonly IUtilityService utilityService;
        private readonly ILabelProvider labelProvider;
        private readonly IStorageService storageService;

    }
}
