using Domain.ValueObjects;

namespace Domain.Entities;

public class Decant
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? ReferenceNo { get; set; }
    public string? Remark { get; set; }
    public DecantStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    // TODO is it a concious choice of creating an aggregate? Perhaps it could be differentiate from other entities?
    public ICollection<DecantItem> Items { get; set; } = new List<DecantItem>();

    public bool CanComplete => CanEdit;
    public bool CanEdit
        => !Status.Equals(DecantStatus.Cancelled) && !Status.Equals(DecantStatus.Completed);

    public void Complete(string completedBy, DateTime completedDate)
    {
        CompletedBy = completedBy;
        CompletedDate = completedDate;
        Status = DecantStatus.Completed;
    }

    public void Cancel(string cancelledBy, DateTime cancelledDate)
    {
        CancelledBy = cancelledBy;
        CancelledDate = cancelledDate;
        Status = DecantStatus.Cancelled;
    }

    public DecantItem AddDecantItem(Pallet pallet, ICollection<int> newQuantities)
    {
        if (Items.Where(i => i.SourcePalletId == pallet.Id).FirstOrDefault() != null)
            throw new InvalidOperationException();

        var newItem = new DecantItem
        {
            SourcePalletId = pallet.Id,
            SourceQty = pallet.Qty
        };
        int seqenceNo = 0;
        foreach (var qty in newQuantities)
        {
            var decantItemPallet = new DecantItemPallet
            {
                Qty = qty,
                SequenceNo = ++seqenceNo,
            };
            decantItemPallet.CopyDataFromPallet(pallet);
            newItem.NewPallets.Add(decantItemPallet);
        }
        Items.Add(newItem);
        return newItem;
    }
}
