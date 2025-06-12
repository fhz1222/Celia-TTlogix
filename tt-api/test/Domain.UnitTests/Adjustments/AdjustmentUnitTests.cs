using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Adjustments;

public class AdjustmentUnitTests
{
    private Adjustment NewAdjustment;

    private Adjustment CompletedAdjustment;

    private Adjustment CancelledAdjustment;
    private Adjustment InProgressAdjustment;
    [SetUp]
    public void Setup()
    {
        NewAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000001",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.New,
            CreatedDate = new DateTime(2022, 10, 9),
            CreatedBy = "9999"
        };
        CompletedAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000011",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Completed,
            CreatedDate = new DateTime(2022, 10, 1),
            CreatedBy = "0001",
            CompletedBy = "0002",
            CompletedDate = new DateTime(2022, 10, 2),
        };
        CancelledAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000020",
            JobType = InventoryAdjustmentJobType.UndoZeroOut,
            Status = InventoryAdjustmentStatus.Cancelled,
            CreatedDate = new DateTime(2022, 10, 4),
            CreatedBy = "0001",
            CancelledBy = "0001",
            CancelledDate = new DateTime(2022, 10, 5),
        };

        InProgressAdjustment = new Adjustment
        {
            JobNo = "ADJ20221000002",
            JobType = InventoryAdjustmentJobType.Normal,
            Status = InventoryAdjustmentStatus.Processing,
            ReferenceNo = "XXXX91",
            CreatedDate = new DateTime(2022, 10, 19),
            CreatedBy = "2000"
        };

    }

    [Test]
    public void CanCompleteTest()
    {
        Assert.IsTrue(NewAdjustment.CanComplete);
        Assert.IsTrue(InProgressAdjustment.CanComplete);
        Assert.IsFalse(CompletedAdjustment.CanComplete);
        Assert.IsFalse(CancelledAdjustment.CanComplete);
    }

    [Test]
    public void CanEditTest()
    {
        Assert.IsTrue(NewAdjustment.CanEdit);
        Assert.IsTrue(InProgressAdjustment.CanEdit);
        Assert.IsFalse(CompletedAdjustment.CanEdit);
        Assert.IsFalse(CancelledAdjustment.CanEdit);
    }


    [Test]
    public void CanAddNewItemsTest()
    {
        Assert.IsFalse(NewAdjustment.CanAddItem);
        Assert.IsTrue(InProgressAdjustment.CanAddItem);
        Assert.IsFalse(CompletedAdjustment.CanAddItem);
        Assert.IsFalse(CancelledAdjustment.CanAddItem);

        NewAdjustment.ReferenceNo = "XXX";
        Assert.IsTrue(NewAdjustment.CanAddItem);

    }

    [Test]
    public void CompleteTest()
    {
        var userCode = "9999";
        var completedDate = DateTime.Now;
        var completedDateBeforeOperation = NewAdjustment.CompletedDate;
        var statusBeforeOperation = NewAdjustment.Status;
        NewAdjustment.Complete(userCode, completedDate);

        Assert.That(NewAdjustment.Status, Is.Not.EqualTo(statusBeforeOperation));
        Assert.That(NewAdjustment.Status, Is.EqualTo(InventoryAdjustmentStatus.Completed));
        Assert.That(completedDateBeforeOperation, Is.Not.EqualTo(completedDate));
        Assert.That(NewAdjustment.CompletedBy, Is.EqualTo(userCode));
        Assert.That(NewAdjustment.CompletedDate, Is.EqualTo(completedDate));
    }


    [Test]
    public void CancelTest()
    {
        var userCode = "1199";
        var cancelDate = DateTime.Now;
        var cancelDateBeforeOperation = CompletedAdjustment.CancelledDate;
        var statusBeforeOperation = CompletedAdjustment.Status;
        CompletedAdjustment.Cancel(userCode, cancelDate);

        Assert.That(CompletedAdjustment.Status, Is.Not.EqualTo(statusBeforeOperation));
        Assert.That(CompletedAdjustment.Status, Is.EqualTo(InventoryAdjustmentStatus.Cancelled));
        Assert.That(cancelDateBeforeOperation, Is.Not.EqualTo(cancelDate));
        Assert.That(CompletedAdjustment.CancelledBy, Is.EqualTo(userCode));
        Assert.That(CompletedAdjustment.CancelledDate, Is.EqualTo(cancelDate));
    }
}

