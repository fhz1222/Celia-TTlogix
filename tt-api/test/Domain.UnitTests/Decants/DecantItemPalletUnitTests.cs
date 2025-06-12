using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Adjustments;

public class DecantItemPalletUnitTests
{
    [Test]
    public void CopyDataFromPalletTest()
    {
        var pallet = new Pallet()
        {
            Id = "TESA000001",
            WhsCode = "WHC1",
            Status = StorageStatus.Putaway,
            Product = new Product()
            {
                Code = "ABC",
                CustomerSupplier = new CustomerSupplier()
                {
                    CustomerCode = "CUST1",
                }
            },
            Qty = 100,
            Length = 320,
            Width = 100,
            Height = 340,
            NetWeight = 2,
            GrossWeight = 3
        };
        var decantItemPallet = new DecantItemPallet
        {
            Qty = pallet.Qty / 2
        };

        decantItemPallet.CopyDataFromPallet(pallet);

        Assert.That(decantItemPallet.Qty, !Is.EqualTo(pallet.Qty));    
        Assert.That(decantItemPallet.ProductCode, Is.EqualTo(pallet.Product.Code));
        Assert.That(decantItemPallet.SupplierId, Is.EqualTo(pallet.Product.CustomerSupplier.SupplierId));
        Assert.That(decantItemPallet.Length, Is.EqualTo(pallet.Length));
        Assert.That(decantItemPallet.Width, Is.EqualTo(pallet.Width));
        Assert.That(decantItemPallet.Height, Is.EqualTo(pallet.Height));
        Assert.That(decantItemPallet.NetWeight, Is.EqualTo(pallet.NetWeight));
        Assert.That(decantItemPallet.GrossWeight, Is.EqualTo(pallet.GrossWeight));
    }
}