using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class UtilityService : IUtilityService
    {
        public static string[] CURRENCIES = new string[] { "EUR", "USD", "CNY" };
        public const string TESA_CODE = "TESA";
        public const string MANUAL_ORDER_PREFIX = "9";
        public const char DELIMITER = '\u0005';
        public const string STR_EHP_SUPPLIER_ID = "999999";
        public const string STR_ESTOCKTRANSFER_ORDERPREFIX = "8";
        public const string ORDERNO_DATEPART_FROMAT = "yyMMdd";
        public static string[] ILOG_READINESS_STATUSES = new string[] { "Registered", "Not registered", "Prototype" };

        public UtilityService(ITTLogixRepository repository, IOptions<AppSettings> appSettings)
        {
            this.repository = repository;
            this.appSettings = appSettings.Value;
        }

        public async Task<Result<string>> GenerateJobNo(JobType jobType)
        {
            var sysDate = DateTime.Now.Date.ToString("yyyyMM");

            var code = JobTypeToCodePrefix(jobType);
            var jobCode = await repository.GetJobCodeAsync((int)code);

            if (jobCode == null)
            {
                return new InvalidResult<string>(new JsonResultError("JobCodeCannotBeFound").ToJson());
            }

            var codePrefix = jobCode.Prefix;
            var jobNo = codePrefix + sysDate;
            var suffix = GetMaxJobNo(jobType, jobNo);

            jobNo += (suffix + 1).ToString().PadLeft(5, '0');

            return new SuccessResult<string>(jobNo);
        }

        public int GetAutoNum(AutoNumTable tableName, string jobNo, int? lineItem = null)
        {
            int p_intNum = 0;
            switch (tableName)
            {
                case AutoNumTable.OutboundDetail:
                    p_intNum = repository.OutboundDetails().Where(d => d.JobNo == jobNo)
                                .Select(d => d.LineItem).ToList().DefaultIfEmpty().Max();
                    break;
                case AutoNumTable.PickingList:
                    p_intNum = repository.PickingLists().Where(pl => pl.JobNo == jobNo && pl.LineItem == lineItem.Value)
                        .Select(pl => pl.SeqNo).ToList().DefaultIfEmpty().Max();
                    break;
                case AutoNumTable.QuarantineLog:
                    p_intNum = repository.QuarantineLogs().Where(d => d.JobNo == jobNo)
                       .Select(d => d.LineItem).ToList().DefaultIfEmpty().Max();
                    break;
                case AutoNumTable.StorageDetail:
                    p_intNum = repository.StorageDetails().Where(s => s.InJobNo == jobNo && s.LineItem == lineItem.Value)
                        .Select(s => s.SeqNo).ToList().DefaultIfEmpty().Max();
                    break;
                case AutoNumTable.StockTransferDetail:
                    p_intNum = repository.StockTransferDetails().Where(s => s.JobNo == jobNo)
                        .Select(s => s.LineItem).ToList().DefaultIfEmpty().Max();
                    break;

                default:
                    throw new NotImplementedException();
                    #region to be implemented
                    //case AutoNumTable.ChargeDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.ChargeDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.CycleCountDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.CycleCountDetail.ToString(), EUtilityGetListMethod.GetAutoSeqNo, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.CycleCountItem:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.CycleCountItem.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.InboundDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.InboundDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.InvAdjustmentDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.InvAdjustmentDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.InvoiceDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.InvoiceDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.PackingList:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.PackingList.ToString(), EUtilityGetListMethod.GetAutoSeqNo, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.RelocationDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.RelocationDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.InboundReversalDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.InboundReversalDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.OutboundReversalDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.OutboundReversalDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.AdjustmentReversalDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.AdjustmentReversalDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    //case AutoNumTable.STFReversalDetail:
                    //    l_oUtilityMgr.GetDataSet(ref p_oFilter, AutoNumTable.STFReversalDetail.ToString(), EUtilityGetListMethod.GetAutoLineItem, ref l_dstRecordset, ref l_oInnerCon);
                    //    break;
                    #endregion

            }
            p_intNum++;
            return p_intNum;
        }

        private CodePrefix JobTypeToCodePrefix(JobType jobType)
        {
            return jobType switch
            {
                JobType.Adjustment => CodePrefix.InventoryAdjustment,
                JobType.Invoice => CodePrefix.Invoice,
                JobType.CycleCount => CodePrefix.CycleCount,
                JobType.Inbound => CodePrefix.Inbound,
                JobType.Other => CodePrefix.Other,
                JobType.Outbound => CodePrefix.Outbound,
                JobType.Loading => CodePrefix.Loading,
                JobType.Quarantine => CodePrefix.Quarantine,
                JobType.Relocation => CodePrefix.Relocation,
                JobType.Traffic => CodePrefix.Traffic,
                JobType.Split => CodePrefix.Split,
                JobType.InboundUpload => CodePrefix.InboundUpload,
                JobType.OutboundUpload => CodePrefix.OutboundUpload,
                JobType.MiscellenousCharge => CodePrefix.MiscelleneousCharges,
                JobType.StorageCharge => CodePrefix.StorageCharges,
                JobType.StockTransfer => CodePrefix.StockTransfer,
                JobType.FixCharge => CodePrefix.FixCharge,
                JobType.Decant => CodePrefix.Decant,
                JobType.InboundReversal => CodePrefix.InboundReversal,
                JobType.OutboundReversal => CodePrefix.OutboundReversal,
                JobType.AdjustmentReversal => CodePrefix.AdjustmentReversal,
                JobType.StockTransferReversal => CodePrefix.StockTransferReversal,
                JobType.StockTakeByLoc => CodePrefix.StockTakeByLoc,
                JobType.JobAllocation => CodePrefix.JobAllocation,
                JobType.JobQueue => CodePrefix.JobQueue,
                JobType.ExtIn => CodePrefix.ExtIn,
                JobType.ExtOut => CodePrefix.ExtOut,
                _ => throw new Exception(),
            };
        }
        private int GetMaxJobNo(JobType jobType, string jobNo)
        {
            switch (jobType)
            {
                case JobType.Outbound:
                    return (from outbound in repository.Outbounds()
                            where outbound.JobNo.StartsWith(jobNo)
                            select outbound.JobNo).ToList().DefaultIfEmpty()
                            .Max(job => job != null ? Convert.ToInt32(job.Substring(job.Length - 5)) : 0);
                case JobType.Loading:
                    return (from loading in repository.Loadings()
                            where loading.JobNo.StartsWith(jobNo)
                            select loading.JobNo).ToList().DefaultIfEmpty()
                          .Max(job => job != null ? Convert.ToInt32(job.Substring(job.Length - 5)) : 0);
                case JobType.Inbound:
                    return (from inbound in repository.Inbounds()
                            where inbound.JobNo.StartsWith(jobNo)
                            select inbound.JobNo).ToList().DefaultIfEmpty()
                            .Max(job => job != null ? Convert.ToInt32(job.Substring(job.Length - 5)) : 0);
                case JobType.Quarantine:
                    return (from ql in repository.QuarantineLogs()
                            where ql.JobNo.StartsWith(jobNo)
                            select ql.JobNo).ToList().DefaultIfEmpty()
                            .Max(job => job != null ? Convert.ToInt32(job.Substring(job.Length - 5)) : 0);

                case JobType.StockTransfer:
                    return (from st in repository.StockTransfers()
                            where st.JobNo.StartsWith(jobNo)
                            select st.JobNo).ToList().DefaultIfEmpty()
                            .Max(job => job != null ? Convert.ToInt32(job.Substring(job.Length - 5)) : 0);
                default:
                    throw new NotImplementedException();
                    //case JobType.Adjustment:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.InvAdjustmentMaster.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.Invoice:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Invoice.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.CycleCount:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.CycleCount.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.Relocation:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Relocation.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.Traffic:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Traffic.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.Split:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Split.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.InboundUpload:
                    //    l_oFilter.ClearAll();
                    //    l_oFilter.AddFilter("UPLJobNo", EFilterMethod.Like, jobno + "%");
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.InboundUploadLog.ToString(), EUtilityGetListMethod.GetUploadJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.OutboundUpload:
                    //    l_oFilter.ClearAll();
                    //    l_oFilter.AddFilter("UPLJobNo", EFilterMethod.Like, jobno + "%");
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.PickedUploadLog.ToString(), EUtilityGetListMethod.GetUploadJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.MiscellenousCharge:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Charge.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.StorageCharge:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Charge.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.StockTransfer:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.StockTransfer.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.FixCharge:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Charge.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.Decant:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.Decant.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.InboundReversal:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.InboundReversalMaster.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.OutboundReversal:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.OutboundReversalMaster.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.AdjustmentReversal:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.AdjustmentReversalMaster.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.StockTransferReversal:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.STFReversalMaster.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.StockTakeByLoc:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.StockTakeByLoc.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.JobAllocation:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.JobAllocation.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.JobQueue:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.JobQueue.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.ExtIn:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.InvTransactionPerSupplierExt.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
                    //case JobType.ExtOut:
                    //    l_oUtilityMgr.GetDataSet(ref l_oFilter, EJobTable.InvTransactionPerSupplierExt.ToString(), EUtilityGetListMethod.GetTransactionJobNoNum, ref l_oDataSet, ref l_oInnerCon);
                    //    break;
            }
        }

        public async Task<string> GetNextPIDNumber(bool addToContext = true)
        {
            //Step 1 : Get The Site ID, from TT_Owner
            var owner = await repository.GetOwnerAsync(appSettings.OwnerCode);
            if (owner == null) return null;

            var pidSearchpattern = owner.Site.Trim() + DateTime.Now.Date.ToString("yyyyMM");

            // step 3 : Get the Next SeqNo 
            var lastPIDCode = await repository.GetLastPIDCode(pidSearchpattern);

            string m_strPIDNumber = GetNextPID(lastPIDCode, pidSearchpattern);

            // Step 4 : Insert into PIDCode
            var l_oPIDCode = new PIDCode
            {
                PIDNo = m_strPIDNumber
            };
            if (addToContext)
                await repository.AddPIDCodeAsync(l_oPIDCode);

            return m_strPIDNumber;
        }

        public async Task<string> GetNextPIDNumber(string lastPIDCode, bool addToContext = true)
        {
            if (lastPIDCode != null && lastPIDCode.Length > 6)
            {
                var pidSearchpattern = lastPIDCode.Substring(0, lastPIDCode.Length - 6);
                string m_strPIDNumber = GetNextPID(lastPIDCode, pidSearchpattern);

                // Step 4 : Insert into PIDCode
                var l_oPIDCode = new PIDCode
                {
                    PIDNo = m_strPIDNumber
                };
                if (addToContext)
                    await repository.AddPIDCodeAsync(l_oPIDCode);

                return m_strPIDNumber;
            }
            return null;
        }

        private string GetNextPID(string lastPIDCode, string pidSearchpattern)
        {
            string numRecord;
            if (lastPIDCode != null && lastPIDCode.Length > 6)
                numRecord = lastPIDCode.Substring(lastPIDCode.Length - 6);
            else
                numRecord = "0";

            var l_uintNumber = Convert.ToUInt32(numRecord, 16);
            l_uintNumber += 1;
            var l_strHex = String.Format("{0:x2}", l_uintNumber);
            string m_strPIDNumber = pidSearchpattern + RepeatedCharacters('0', 6 - l_strHex.ToString().Length) + l_strHex.ToUpper();
            return m_strPIDNumber;
        }

        private string GetNextGroupPID(string lastGroupPIDCode, string pidSearchpattern)
        {
            string numRecord;
            if (lastGroupPIDCode != null && lastGroupPIDCode.Length > 6)
                numRecord = lastGroupPIDCode.Substring(lastGroupPIDCode.Length - 6);
            else
                numRecord = "0";

            var l_uintNumber = Convert.ToUInt32(numRecord, 16);
            l_uintNumber += 1;
            var l_strHex = String.Format("{0:x2}", l_uintNumber);
            string m_strPIDNumber = pidSearchpattern + RepeatedCharacters('0', 6 - l_strHex.ToString().Length) + l_strHex.ToUpper();
            return m_strPIDNumber;
        }

        public async Task<string> GetNextOrderNo(string prefix, int suffixLength)
        {
            var searchOrderPattern = prefix + DateTime.Now.ToString(ORDERNO_DATEPART_FROMAT);
            var indexOfNextNo = searchOrderPattern.Length;
            var orderNo = await repository.GetLastEKanbanOrderNo(searchOrderPattern);
            return GetNextNumber(orderNo, prefix, indexOfNextNo, suffixLength);
        }

        public async Task<string> GetNextOrderNoForStockTransfer(string prefix, int suffixLength)
        {
            var searchOrderPattern = prefix + DateTime.Now.ToString(ORDERNO_DATEPART_FROMAT);
            var indexOfNextNo = searchOrderPattern.Length;
            var orderNo = await repository.GetLastEStockTransferOrderNo(searchOrderPattern);
            return GetNextNumber(orderNo, prefix, indexOfNextNo, suffixLength);
        }

        private string GetNextNumber(string orderNo, string prefix, int startIndx, int suffixLength)
        {
            if (orderNo == null)
            {
                orderNo = prefix + DateTime.Now.ToString(ORDERNO_DATEPART_FROMAT) + "1".PadLeft(suffixLength, '0');
            }
            else
            {
                orderNo = prefix + DateTime.Now.ToString(ORDERNO_DATEPART_FROMAT) + (int.Parse(orderNo.Substring(startIndx, suffixLength)) + 1).ToString().PadLeft(suffixLength, '0');
            }
            return orderNo;
        }

        private string RepeatedCharacters(char p_cLetter, int p_intRepeat)
        {
            return String.Empty.PadLeft(p_intRepeat, p_cLetter);
        }

        public async Task<string> GetNextGroupPIDNumber()
        {
            //Step 1 : Get The Site ID, from TT_Owner
            var owner = await repository.GetOwnerAsync(appSettings.OwnerCode);
            if (owner == null) return null;

            var pidSearchpattern = "G" + owner.Site.Trim() + DateTime.Now.Date.ToString("yyyyMM");

            // step 3 : Get the Next SeqNo 
            var lastPIDCode = await repository.GetLastGroupPIDCode(pidSearchpattern);

            string m_strPIDNumber = GetNextGroupPID(lastPIDCode, pidSearchpattern);

            // Step 4 : Insert into PIDCode
            var l_oPIDCode = new PIDCode
            {
                PIDNo = m_strPIDNumber
            };

            return m_strPIDNumber;
        }

        public bool ValidateUnloadingPointId(int? id, string CustomerCode)
        {
            if (id == null)
            {
                return true;
            }
            var point = repository.GetUnloadingPoint(id.Value);
            return point != null && point.CustomerCode == CustomerCode;
        }

        public bool ValidatePalletTypeId(int id)
        {
            var palletType = repository.GetPalletType(id);
            return palletType != null;
        }

        public bool ValidateELLISPalletTypeId(int id)
        {
            var palletType = repository.GetELLISPalletType(id);
            return palletType != null;
        }

        private readonly ITTLogixRepository repository;
        private readonly AppSettings appSettings;
    }
}
