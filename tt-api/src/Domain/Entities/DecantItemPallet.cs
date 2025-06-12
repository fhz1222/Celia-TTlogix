using Domain.ValueObjects;

namespace Domain.Entities;

public class DecantItemPallet
{
    public string PalletId { get; set; } = null!;
    public int Qty { get; set; }
    public int SequenceNo { get; set; }
    public string ProductCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }

    public void CopyDataFromPallet(Pallet pallet)
    {
        ProductCode = pallet.Product.Code;
        SupplierId = pallet.Product.CustomerSupplier.SupplierId;
        Length = pallet.Length;
        Width = pallet.Width;
        Height = pallet.Height;
        NetWeight = pallet.NetWeight;
        GrossWeight = pallet.GrossWeight;
    }

    public Pallet CreateNewPallet(string palletId, string customerCode)
    {
        return new Pallet
        {
            Id = palletId,
            Product = new Product
            {
                Code = ProductCode,
                CustomerSupplier = new CustomerSupplier
                {
                    SupplierId = SupplierId,
                    CustomerCode = customerCode,
                }
            },
            Qty = Qty,
            QtyPerPkg = Qty,
            Status = StorageStatus.Putaway,
            Length = Length,
            Width = Width,
            Height = Height,
            NetWeight = NetWeight,
            GrossWeight = GrossWeight
        };
    }
}