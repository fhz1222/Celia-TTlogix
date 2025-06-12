using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Adjustments;

public class DecantUnitTests
{
    private Decant NewDecant;

    private Decant CompletedDecant;

    private Decant CancelledDecant;
    private Decant InProgressDecant;
    [SetUp]
    public void Setup()
    {
        NewDecant = new Decant
        {
            JobNo = "DCT20221000001",
            Status = DecantStatus.New,
            CreatedDate = new DateTime(2022, 10, 9),
            CreatedBy = "9999"
        };
        CompletedDecant = new Decant
        {
            JobNo = "DCT20221000011",
            Status = DecantStatus.Completed,
            CreatedDate = new DateTime(2022, 10, 1),
            CreatedBy = "0001",
            CompletedBy = "0002",
            CompletedDate = new DateTime(2022, 10, 2),
        };
        CancelledDecant = new Decant
        {
            JobNo = "DCT20221000020",
            Status = DecantStatus.Cancelled,
            CreatedDate = new DateTime(2022, 10, 4),
            CreatedBy = "0001",
            CancelledBy = "0001",
            CancelledDate = new DateTime(2022, 10, 5),
        };

        InProgressDecant = new Decant
        {
            JobNo = "DCT20221000002",
            Status = DecantStatus.Processing,
            ReferenceNo = "XXXX91",
            CreatedDate = new DateTime(2022, 10, 19),
            CreatedBy = "2000"
        };

    }

    [Test]
    public void CanCompleteTest()
    {
        Assert.IsTrue(NewDecant.CanComplete);
        Assert.IsTrue(InProgressDecant.CanComplete);
        Assert.IsFalse(CompletedDecant.CanComplete);
        Assert.IsFalse(CancelledDecant.CanComplete);
    }

    [Test]
    public void CanEditTest()
    {
        Assert.IsTrue(NewDecant.CanEdit);
        Assert.IsTrue(InProgressDecant.CanEdit);
        Assert.IsFalse(CompletedDecant.CanEdit);
        Assert.IsFalse(CancelledDecant.CanEdit);
    }

    [Test]
    public void CompleteTest()
    {
        var userCode = "9999";
        var completedDate = DateTime.Now;
        var completedDateBeforeOperation = NewDecant.CompletedDate;
        var statusBeforeOperation = NewDecant.Status;
        NewDecant.Complete(userCode, completedDate);

        Assert.That(NewDecant.Status, Is.Not.EqualTo(statusBeforeOperation));
        Assert.That(NewDecant.Status, Is.EqualTo(DecantStatus.Completed));
        Assert.That(completedDateBeforeOperation, Is.Not.EqualTo(completedDate));
        Assert.That(NewDecant.CompletedBy, Is.EqualTo(userCode));
        Assert.That(NewDecant.CompletedDate, Is.EqualTo(completedDate));
    }


    [Test]
    public void CancelTest()
    {
        var userCode = "1199";
        var cancelDate = DateTime.Now;
        var cancelDateBeforeOperation = CompletedDecant.CancelledDate;
        var statusBeforeOperation = CompletedDecant.Status;
        CompletedDecant.Cancel(userCode, cancelDate);

        Assert.That(CompletedDecant.Status, Is.Not.EqualTo(statusBeforeOperation));
        Assert.That(CompletedDecant.Status, Is.EqualTo(DecantStatus.Cancelled));
        Assert.That(cancelDateBeforeOperation, Is.Not.EqualTo(cancelDate));
        Assert.That(CompletedDecant.CancelledBy, Is.EqualTo(userCode));
        Assert.That(CompletedDecant.CancelledDate, Is.EqualTo(cancelDate));
    }


    [Test]
    public void AddDecantItemTest()
    {
        var oldPallet = new Pallet()
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
            Qty = 500,
            Length = 520,
            Width = 1300,
            Height = 340,
            NetWeight = 3,
            GrossWeight = 4
        };
        var noOfPalletsBeforeAdding = NewDecant.Items.Count;
        var newQuantities = new int[] { 100, 100, 100, 100, 100 };
        NewDecant.AddDecantItem(oldPallet, newQuantities);
        Assert.That(NewDecant.Items.Count, Is.EqualTo(noOfPalletsBeforeAdding + 1));

        var addedDecantItem = NewDecant.Items.Where(i => i.SourcePalletId == oldPallet.Id).SingleOrDefault();
        Assert.That(addedDecantItem, !Is.Null);
        Assert.That(addedDecantItem.SourceQty, Is.EqualTo(oldPallet.Qty));

        Assert.That(addedDecantItem.NewPallets.Count, Is.EqualTo(newQuantities.Length));
        int qtyIndex = 0;
        foreach (var newDecantPallet in addedDecantItem.NewPallets)
        {
            Assert.That(newDecantPallet.Qty, Is.EqualTo(newQuantities[qtyIndex]));
            qtyIndex++;
            Assert.That(newDecantPallet.SequenceNo, Is.EqualTo(qtyIndex));
            Assert.That(newDecantPallet.ProductCode, Is.EqualTo(oldPallet.Product.Code));
            Assert.That(newDecantPallet.SupplierId, Is.EqualTo(oldPallet.Product.CustomerSupplier.SupplierId));
            Assert.That(newDecantPallet.Length, Is.EqualTo(oldPallet.Length));
            Assert.That(newDecantPallet.Width, Is.EqualTo(oldPallet.Width));
            Assert.That(newDecantPallet.Height, Is.EqualTo(oldPallet.Height));
            Assert.That(newDecantPallet.NetWeight, Is.EqualTo(oldPallet.NetWeight));
            Assert.That(newDecantPallet.GrossWeight, Is.EqualTo(oldPallet.GrossWeight));
        }
    }


}

