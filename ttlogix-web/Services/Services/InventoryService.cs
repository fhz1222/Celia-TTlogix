using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Entities.MRP;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class InventoryService : ServiceBase<InventoryService>, IInventoryService
    {
        public InventoryService(ITTLogixRepository repository,
            IMRPRepository mrpRepository,
            IMapper mapper,
            ILocker locker,
            IHttpContextAccessor contextAccessor,
            ILogger<InventoryService> logger)
            : base(locker, logger)
        {
            this.repository = repository;
            this.mrpRepository = mrpRepository;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<IEnumerable<PartMasterBySupplierDto>> GetPartMasterListBySupplier(string customerCode, string supplierID)
        {
            return await repository.GetPartMasterListBySupplier<PartMasterBySupplierDto>(customerCode, supplierID);
        }

        public async Task<PartMasterListDto> GetPartMasterList(PartMasterListQueryFilter filter)
        {
            var query = repository.GetPartMasterList<PartMasterListItemDto>(filter);

            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new PartMasterListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<PartMasterDto> GetPartMaster(string customerCode, string productCode1, string supplierID)
        {
            var entity = await repository.GetPartMasterAsync(customerCode.Trim(), supplierID.Trim(), productCode1.Trim());
            if (entity != null)
            {
                return await MapPartMasterDto(entity);
            }
            return null;
        }

        public async Task<CustomerInventoryProductCodeMapDto> GetCustomerInventoryProductCodeMap(string customerCode)
        {
            var inventoryCtr = await repository.GetInventoryControlAsync(customerCode);
            if (inventoryCtr == null)
                return null;

            return new CustomerInventoryProductCodeMapDto()
            {
                CustomerCode = customerCode,
                PC1Name = !string.IsNullOrEmpty(inventoryCtr.PC1Type) ? (await repository.GetProductCodeAsync(inventoryCtr.PC1Type))?.Name : null,
                PC2Name = !string.IsNullOrEmpty(inventoryCtr.PC2Type) ? (await repository.GetProductCodeAsync(inventoryCtr.PC2Type))?.Name : null,
                PC3Name = !string.IsNullOrEmpty(inventoryCtr.PC3Type) ? (await repository.GetProductCodeAsync(inventoryCtr.PC3Type))?.Name : null,
                PC4Name = !string.IsNullOrEmpty(inventoryCtr.PC4Type) ? (await repository.GetProductCodeAsync(inventoryCtr.PC4Type))?.Name : null,
            };
        }

        public async Task<CustomerInventoryControlCodeMapDto> GetCustomerInventoryControlCodeMap(string customerCode)
        {
            var inventoryCtr = await repository.GetInventoryControlAsync(customerCode);
            if (inventoryCtr == null)
                return null;

            return new CustomerInventoryControlCodeMapDto()
            {
                CustomerCode = customerCode,
                CC1Name = !string.IsNullOrEmpty(inventoryCtr.CC1Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC1Type))?.Name : null,
                CC2Name = !string.IsNullOrEmpty(inventoryCtr.CC2Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC2Type))?.Name : null,
                CC3Name = !string.IsNullOrEmpty(inventoryCtr.CC3Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC3Type))?.Name : null,
                CC4Name = !string.IsNullOrEmpty(inventoryCtr.CC4Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC4Type))?.Name : null,
                CC5Name = !string.IsNullOrEmpty(inventoryCtr.CC5Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC5Type))?.Name : null,
                CC6Name = !string.IsNullOrEmpty(inventoryCtr.CC6Type) ? (await repository.GetControlCodeAsync(inventoryCtr.CC6Type))?.Name : null,
            };
        }

        public async Task<IEnumerable<UOMWithDecimalDto>> GetUOMListWithDecimal(string customerCode)
        {
            return await repository.GetUOMListWithDecimal<UOMWithDecimalDto>(customerCode);
        }

        public async Task<Result<PartMasterDto>> UpdatePartMaster(PartMasterDto partMasterDto)
        {
            #region Get UOM
            var uom = repository.UOMs().Where(u => u.Code == partMasterDto.UOM && u.Status == 1).FirstOrDefault();
            if (uom == null)
                return new InvalidResult<PartMasterDto>(new JsonResultError("FailedToRetrieveUOM").ToJson());
            #endregion

            var result = await WithTransactionScope<PartMasterDto>(async () =>
            {
                if (!partMasterDto.SPQCorrectForCPart)
                {
                    return new InvalidResult<PartMasterDto>(new JsonResultError("CPartSPQIncorrectQty").ToJson());
                }

                var entity = await repository.GetPartMasterAsync(partMasterDto.CustomerCode, partMasterDto.SupplierID, partMasterDto.ProductCode1);
                if (entity == null)
                {
                    return new NotFoundResult<PartMasterDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (!await CurrentUserHasRole(SystemModuleNames.PARTMASTERUNLOADING))
                {
                    partMasterDto.UnloadingPointId = entity.UnloadingPointId;
                }
                mapper.Map(partMasterDto, entity);
                await repository.SaveChangesAsync();

                var postAddResult = await PostUpdatePartMaster(entity, uom.Name);
                if (postAddResult.ResultType != ResultType.Ok)
                    return new InvalidResult<PartMasterDto>(postAddResult.Errors[0]);

                var dto = await MapPartMasterDto(entity);
                return new SuccessResult<PartMasterDto>(dto);
            });
            if (result.ResultType == ResultType.Ok)
            {
                var postAddResult = await PostUpdatePartMasterInMRP(result.Data, uom.Name);
                if (postAddResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<PartMasterDto>(postAddResult.Errors[0]);
                }
            }
            return result;
        }

        public async Task<Result<PartMasterDto>> CreatePartMaster(PartMasterDto partMasterDto, string userCode)
        {
            var uom = repository.UOMs().Where(u => u.Code == partMasterDto.UOM && u.Status == 1).FirstOrDefault();
            if (uom == null)
                return new InvalidResult<PartMasterDto>(new JsonResultError("FailedToRetrieveUOM").ToJson());

            var result = await WithTransactionScope<PartMasterDto>(async () =>
            {
                if (!partMasterDto.SPQCorrectForCPart)
                {
                    return new InvalidResult<PartMasterDto>(new JsonResultError("CPartSPQIncorrectQty").ToJson());
                }

                #region Step 1 : Insert into Part Master
                var partMasterObjResult = await CreatePartMasterObject(partMasterDto, userCode);
                if (partMasterObjResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<PartMasterDto>(partMasterObjResult.Errors[0]);
                }
                #endregion

                #region Step 2 : Get Warehouse List
                var warehouseList = await repository.Warehouses().ToListAsync();
                #endregion

                #region Step 3 : Insert into Inventory 
                var inventoryList = warehouseList.Select(w => new Inventory()
                {
                    CustomerCode = partMasterDto.CustomerCode,
                    ProductCode1 = partMasterDto.ProductCode1,
                    SupplierID = partMasterDto.SupplierID,
                    Ownership = Core.Enums.Ownership.Supplier,
                    WHSCode = w.Code,
                    AllocatedPkg = 0,
                    AllocatedQty = 0,
                    OnHandPkg = 0,
                    OnHandQty = 0,
                    QuarantinePkg = 0,
                    QuarantineQty = 0
                });
                foreach (var i in inventoryList)
                {
                    await repository.AddInventoryAsync(i);
                }
                #endregion
                var postAddResult = await PostAddPartMaster(partMasterObjResult.Data, warehouseList, uom.Name);
                if (postAddResult.ResultType != ResultType.Ok)
                    return new InvalidResult<PartMasterDto>(postAddResult.Errors[0]);

                var dto = await MapPartMasterDto(partMasterObjResult.Data);
                return new SuccessResult<PartMasterDto>(dto);
            });
            if (result.ResultType == ResultType.Ok)
            {
                var postAddResult = await PostAddPartMasterToMRP(result.Data, uom.Name);
                if (postAddResult.ResultType != ResultType.Ok)
                {
                    return new InvalidResult<PartMasterDto>(postAddResult.Errors[0]);
                }
            }
            return result;
        }

        public async Task<Result<UnloadingPointChoiceDto>> GetUnloadingPointChoice(string customerCode, string supplierID = null)
        {
            var unloadingPoints = await repository.GetUnloadingPoints(customerCode);
            var defaultPoint = supplierID != null ? await repository.GetDefaultUnloadingPointId(customerCode, supplierID) : null;
            return new SuccessResult<UnloadingPointChoiceDto>(new UnloadingPointChoiceDto
            {
                DefaultId = defaultPoint,
                Options = unloadingPoints
            });
        }

        public async Task<Result<IEnumerable<PalletTypeDto>>> GetPalletTypes()
        {
            var types = await repository.GetPalletTypes<PalletTypeDto>();
            return new SuccessResult<IEnumerable<PalletTypeDto>>(types);
        }

        public async Task<Result<IEnumerable<ELLISPalletTypeDto>>> GetELLISPalletTypes()
        {
            var types = await repository.GetELLISPalletTypes<ELLISPalletTypeDto>();
            return new SuccessResult<IEnumerable<ELLISPalletTypeDto>>(types);
        }

        private async Task<PartMasterDto> MapPartMasterDto(PartMaster entity)
        {
            string supplierName = (await repository.GetSupplierMasterAsync(entity.CustomerCode, entity.SupplierID)).CompanyName;
            return mapper.Map<PartMasterDto>(entity,
                opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        dest.SupplierName = supplierName;
                    });
                });
        }

        private async Task<Result<bool>> PostAddPartMaster(PartMaster partMaster, IEnumerable<Warehouse> warehouseList, string uomName)
        {
            #region Get UOM // AU: UOM is already checked an dwe get it from the partMaster
            #endregion

            #region VMI.ItemMaster

            var inventoryList = warehouseList.Select(w => new Inventory()
            {
                CustomerCode = partMaster.CustomerCode,
                SupplierID = partMaster.SupplierID,
                ProductCode1 = partMaster.ProductCode1,
                WHSCode = w.Code,
                AllocatedPkg = 0,
                AllocatedQty = 0,
                OnHandPkg = 0,
                OnHandQty = 0,
                QuarantinePkg = 0,
                QuarantineQty = 0,
                TransitPkg = 0,
                TransitQty = 0,
                DiscrepancyQty = 0,
                Ownership = Core.Enums.Ownership.EHP
            });
            foreach (var i in inventoryList)
            {
                await repository.AddInventoryAsync(i);
            }
            #endregion

            #region VMI.ItemMaster
            var itemMaster = new ItemMaster()
            {
                FactoryID = partMaster.CustomerCode,
                SupplierID = partMaster.SupplierID,
                ProductCode = partMaster.ProductCode1,
                Description = partMaster.Description,
                UOM = uomName.ToUpper(),
                PeakUsage = 0,
                Width = partMaster.Width,
                Height = partMaster.Height,
                Depth = partMaster.Length,
                KanbanQty = (int)partMaster.SPQ,
                Version = 2,
                CommitFinishedGoods = 0,
                CommitRawMaterials = 0,
                CommitWIP = 0
            };
            await repository.AddItemMasterAsync(itemMaster);
            #endregion

            #region Setting Sunset Expired Alert
            if (await repository.GetSunsetExpiredAlertAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1) == null)
            {
                var supplierDetail = repository.SupplierDetails().Where(s => s.SupplierID == partMaster.SupplierID && s.FactoryID == partMaster.CustomerCode).FirstOrDefault();
                await repository.AddSunsetExpiredAlertAsync(new SunsetExpiredAlert
                {
                    FactoryID = partMaster.CustomerCode,
                    SupplierID = partMaster.SupplierID,
                    ProductCode = partMaster.ProductCode1,
                    SunsetPeriod = supplierDetail?.DefaultSunsetPeriod ?? 30
                });
            }
            #endregion

            #region VMI.SupplierItemMaster
            await repository.AddSupplierItemMasterAsync(new SupplierItemMaster()
            {
                FactoryID = partMaster.CustomerCode,
                SupplierID = partMaster.SupplierID,
                ProductCode = partMaster.ProductCode1,
                SupplierPartNo = partMaster.ProductCode2,
                PastCost = 0,
                CurrentCost = 0,
                FutureCost = 0,
                TargetMinStockDays = 0,
                TargetMinStockQty = 0,
                TargetMinStockQtyStatus = "A",
                TargetMaxStockDays = 0,
                TargetMaxStockQty = 0,
                TargetMaxStockQtyStatus = "A",
                MinShipQty = 0,
                OuterQty = (int)partMaster.SPQ,
                InnerQty = (int)partMaster.SPQ,
                LeadTimeMRPRunToExSupplier = 0,
                LeadTimeExSupplierToShipDepart = 0,
                ShipTransitTime = 0,
                LeadTimePortArrivalToWH = 0,
                TotalOverseasLeadTime = 0,
                LeadTimeOrderToDispatch = 0,
                LocalTransitTime = 0,
                TotalLocalLeadTime = 0,
                SupplierStatus = (byte)partMaster.Status,
                DTL = ""
            });
            #endregion
            return new SuccessResult<bool>(true);
        }

        private async Task<Result<bool>> PostAddPartMasterToMRP(PartMasterDto partMaster, string uomName)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                #region VMI_MRP.ItemMaster

                if (await mrpRepository.GetItemMasterAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1) == null)
                {
                    await mrpRepository.AddItemMasterAsync(new MRPItemMaster
                    {
                        FactoryID = partMaster.CustomerCode,
                        SupplierID = partMaster.SupplierID,
                        ProductCode = partMaster.ProductCode1,
                        Description = partMaster.Description,
                        UOM = uomName,
                        PeakUsage = 0,
                        Width = partMaster.Width,
                        Height = partMaster.Height,
                        Depth = partMaster.Length,
                        KanbanQty = (int)partMaster.SPQ,
                        Version = 2,
                        CommitFinishedGoods = 0,
                        CommitRawMaterials = 0,
                        CommitWIP = 0
                    });
                }
                #endregion

                #region VMI_MRP.Inventory
                // insert data to a different db

                if (await mrpRepository.GetInventoryAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1) == null)
                {
                    await mrpRepository.AddInventoryAsync(new MRPInventory()
                    {
                        FactoryID = partMaster.CustomerCode,
                        SupplierID = partMaster.SupplierID,
                        ProductCode = partMaster.ProductCode1,
                        OnHandQty = 0,
                        AllocatedQty = 0,
                        OnHoldQty = 0,
                        TransitQty = 0
                    });
                }
                #endregion
                return new SuccessResult<bool>(true);
            });
        }

        private async Task<Result<bool>> PostUpdatePartMaster(PartMaster partMaster, string uomName)
        {
            #region VMI.ItemMaster
            var itemMaster = await repository.GetItemMasterAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1);
            if (itemMaster != null)
            {
                itemMaster.Description = partMaster.Description;
                itemMaster.Width = partMaster.Width;
                itemMaster.Height = partMaster.Height;
                itemMaster.Depth = partMaster.Length;
                itemMaster.KanbanQty = (int)partMaster.SPQ;
                itemMaster.UOM = uomName.ToUpper();
                itemMaster.Obsolete = partMaster.Status == (byte)ValueStatus.Active ? "" : "O";
            }
            #endregion

            #region VMI.SupplierItemMaster
            var supplierMaster = await repository.GetSupplierItemMasterAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1);
            if (supplierMaster != null)
            {
                supplierMaster.SupplierPartNo = partMaster.ProductCode2;
                supplierMaster.OuterQty = (int)partMaster.SPQ;
                supplierMaster.InnerQty = (int)partMaster.SPQ;
                supplierMaster.SupplierStatus = (byte)partMaster.Status;
            }
            #endregion

            await repository.SaveChangesAsync();
            return new SuccessResult<bool>(true);
        }

        private async Task<Result<bool>> PostUpdatePartMasterInMRP(PartMasterDto partMaster, string uomName)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                #region VMI_MRP.ItemMaster
                var mrpItemMaster = await mrpRepository.GetItemMasterAsync(partMaster.CustomerCode, partMaster.SupplierID, partMaster.ProductCode1);
                if (mrpItemMaster != null)
                {
                    mrpItemMaster.Description = partMaster.Description;
                    mrpItemMaster.Width = partMaster.Width;
                    mrpItemMaster.Height = partMaster.Height;
                    mrpItemMaster.Depth = partMaster.Length;
                    mrpItemMaster.KanbanQty = (int)partMaster.SPQ;
                    mrpItemMaster.UOM = uomName.ToUpper();
                }
                #endregion

                await mrpRepository.SaveChangesAsync();
                return new SuccessResult<bool>(true);
            });
        }

        private async Task<Result<PartMaster>> CreatePartMasterObject(PartMasterDto partMasterDto, string userCode)
        {
            var entity = new PartMaster();
            mapper.Map(partMasterDto, entity);

            //check key unique
            var t = await repository.GetPartMasterAsync(partMasterDto.CustomerCode, partMasterDto.SupplierID, partMasterDto.ProductCode1);
            if (t != null)
            {
                return new InvalidResult<PartMaster>(new JsonResultError("PartMasterAlreadyExists").ToJson());
            }
            entity.CreatedBy = userCode;
            await repository.AddPartMasterAsync(entity);

            return new SuccessResult<PartMaster>(entity);
        }

        private async Task<bool> CurrentUserHasRole(string moduleName)
        {
            var userName = contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return false;
            }
            var user = await repository.GetUserAsync(userName);
            var modules = await repository.GetSystemModuleNamesForGroup(user.GroupCode);
            return modules.Contains(moduleName);
        }

        private readonly ITTLogixRepository repository;
        private readonly IMRPRepository mrpRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;
    }
}
