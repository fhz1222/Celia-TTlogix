using Persistence.Entities;

namespace Persistence.Extensions
{
    internal static class StorageDetailExtensions
    {
        internal static void CopyValuesFromParent(this TtStorageDetail storageDetail, TtStorageDetail parentObject)
        {
            storageDetail.InJobNo = parentObject.InJobNo;
            storageDetail.LineItem = parentObject.LineItem;
            storageDetail.SeqNo = parentObject.SeqNo;
            storageDetail.InboundDate = parentObject.InboundDate;
            storageDetail.NoOfLabel = parentObject.NoOfLabel;
            storageDetail.ControlCode1 = parentObject.ControlCode1;
            storageDetail.ControlCode2 = parentObject.ControlCode2;
            storageDetail.ControlCode3 = parentObject.ControlCode3;
            storageDetail.ControlCode4 = parentObject.ControlCode4;
            storageDetail.ControlCode5 = parentObject.ControlCode5;
            storageDetail.ControlCode6 = parentObject.ControlCode6;
            storageDetail.ControlDate = parentObject.ControlDate;
            storageDetail.Whscode = parentObject.Whscode;
            storageDetail.LocationCode = parentObject.LocationCode;
            storageDetail.SerialNo = parentObject.SerialNo;
            storageDetail.ChargedDate = parentObject.ChargedDate;
            storageDetail.DownloadBy = parentObject.DownloadBy;
            storageDetail.DownloadDate = parentObject.DownloadDate;
            storageDetail.PutawayBy = parentObject.PutawayBy;
            storageDetail.PutawayDate = parentObject.PutawayDate;
            storageDetail.Version = parentObject.Version;
            storageDetail.BondedStatus = parentObject.BondedStatus;
            storageDetail.Ownership = parentObject.Ownership;
            storageDetail.BuyingPrice = parentObject.BuyingPrice;
            storageDetail.SellingPrice = parentObject.SellingPrice;
            storageDetail.IsVmi = parentObject.IsVmi;
            storageDetail.ParentId = parentObject.Pid;
        }
    }
}
