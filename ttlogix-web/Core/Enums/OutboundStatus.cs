using System.ComponentModel;

namespace TT.Core.Enums
{
	public enum OutboundStatus
	{
		NewJob = 0,
		PartialDownload = 1,
		Downloaded = 2,
		PartialPicked = 3,
		Picked = 4,
		Packed = 5,
		OutStanding = 6,
		InTransit = 7,
		Completed = 8,
		Cancelled = 9,
		Discrepancy = 10,
		All = 11,
		// TESAG wh transfer status
		TruckDeparture = 12,
		DisrepancyStorage = 23,
	}
	public enum OutboundType
	{
		ManualEntry = 0,
		CrossDock = 1,
		EKanban = 2,
		Return = 3,
		WHSTransfer = 4,
		ScannerManualEntry = 7,
	}

	public enum EKanbanStatus
	{
		New = 0,
		Imported = 1,
		InTransit = 2,
		//DataSent - For SAP special requirement of Truck loading, this status is use to indicate
		//an EKanban is added into a Loading job.
		DataSent = 3,
		Completed = 4,
		Cancelled = 9
	}

	public enum Ownership
	{
		Supplier = 0,
		EHP = 1
	}

	public enum OutboundDetailStatus
	{
		New = 0,
		Picking = 1,
		Picked = 2
	}

	public enum ExtSystemLocation
	{
		RETURN = 0
	}

	public enum JobType
	{
		Inbound,
		Outbound,
		CycleCount,
		Traffic,
		Billing,
		Relocation,
		Adjustment,
		Split,
		Quarantine,
		Packing,
		Other,
		InboundUpload,
		OutboundUpload,
		Invoice,
		StorageCharge,
		MiscellenousCharge,
		TrafficCharge,
		StockTransfer,
		FixCharge,
		Decant,
		InboundReversal,
		OutboundReversal,
		AdjustmentReversal,
		StockTransferReversal,
		StockTakeByLoc,
		JobAllocation,
		JobQueue,
		CycleCountUpload,
		StockTransferUpload,
		Loading,
		ExtIn,
		ExtOut
	}

	public enum ETT_JobCode
	{
		Code = 0,
		Prefix = 1,
		Name = 2,
		Status = 3,
		CreatedBy = 4,
		CreatedDate = 5,
		CancelledBy = 6,
		CancelledDate = 7
	}

	public enum CodePrefix
	{
		AccessGroup = 1,
		AddressBook,
		AddressContact,
		ChargeCategory,
		ChargeCode,
		ChargeUnit,
		ControlCode,
		CycleCount,
		Inbound,
		InboundUpload,
		InventoryAdjustment,
		Invoice,
		MiscelleneousCharges,
		Other,
		Outbound,
		OutboundUpload,
		PackageType,
		Packing,
		PaymentTerm,
		ProductCode,
		Quarantine,
		Relocation,
		ShippingTerm,
		Split,
		StorageCharges,
		StockTransfer,
		SystemResource,
		Traffic,
		Truck,
		UOM,
		FixCharge,
		Decant,
		InboundReversal,
		OutboundReversal,
		AdjustmentReversal,
		StockTransferReversal,
		StockTakeByLoc,
		JobAllocation,
		JobQueue,
		CycleCountUpload,
		StockTransferUpload,
		Loading = 42,
		ExtIn,
		ExtOut
	}

	public enum StorageStatus
	{
		Incoming = 0,
		Putaway = 1,
		Allocated = 2,
		Picked = 3,
		Packed = 4,
		InTransit = 5,
		Dispatched = 6,
		Splitted = 7,
		Quarantine = 9,

		//Temporary status before Allocated
		Allocating = 10,
		Kitting = 11,
		Splitting = 12,

		//Status 21 - 30 is reserved for item locked by warehouse operation
		Transferring = 21,
		Decant = 22,
		Discrepancy = 23,

		DiscrepancyFixed = 96,
		Reversal = 97,
		ZeroOut = 98,
		Cancelled = 99
	}

	public enum StorageGroupStatus
	{
		Empty = 0,
		InUse = 1,
		Closed = 2,
		Transformed = 3
	}

	public enum LocationType
	{
		Normal = 0,
		Quarantine = 1,
		CrossDock = 2,
		Standby = 3,
		ExtSystem = 4
	}

	public enum AutoNumTable
	{
		ChargeDetail = 0,
		CycleCountDetail,
		CycleCountItem,
		DecantDetail,
		InboundDetail,
		InvAdjustmentDetail,
		InvoiceDetail,
		OutboundDetail,
		PackingList,
		PickingList,
		QuarantineLog,
		RelocationDetail,
		StorageDetail,
		StockTransferDetail,
		InboundReversalDetail,
		OutboundReversalDetail,
		AdjustmentReversalDetail,
		STFReversalDetail
	}

	public enum LoadingStatus
	{
		NewJob = 0,
		Processing = 1,
		Picked = 2,
		Confirmed = 5,
		InTransit = 6,
		Completed = 7,
		Cancelled = 9,
		All = 11
	}

	public enum InboundStatus
	{
		[Description("New Job")]
		NewJob = 0,
		[Description("Partial Download")]
		PartialDownload = 1,
		[Description("Downloaded")]
		Downloaded = 2,
		[Description("Partial Putaway")]
		PartialPutaway = 3,
		[Description("Outstanding")]
		Outstanding = 4,
		[Description("Completed")]
		Completed = 8,
		[Description("Cancelled")]
		Cancelled = 9,
		All = 10
	}

	public enum InventoryTransactionType
	{
		Inbound = 0,
		Outbound = 1,
		PositiveAdjustment = 2,
		NegativeAdjustment = 3,
		PurchaseOfAgedStock = 4,
		SalesOfAgedStock = 5,
		ReversalOfReturn = 6,
		ReversalOfInbound = 7,
		ReversalOfAdjustment = 8,
		ReversalOfSaleOfAgeStock = 9,
		ReversalOfPurchaseOfAgeStock = 10,
		ExternalSystemInbound = 11,
		ExternalSystemOutbound = 12,
		//ExternalSystemCallOff means outbound completed in VMI hub but stock get from ELX return area.
		ExternalSystemCallOff = 13,
		WHSTransferIn = 14,
		WHSTransferOut = 15,
	}

	public enum BondedStatus
	{
		NonBonded = 0,
		Bonded = 1,
	}

	public enum ValueStatus
	{
		InActive = 0,
		Active
	}

	public enum LockType
	{
		NotLocked,
		HardLocked,
		SoftLocked,
	}
	public enum Destination
	{
		Removed = 0,
		PickingList = 1,
		StorageDetail = 2
	}
	public enum ItemType
	{
		Original = 0,
		TopUp = 1
	}

	public enum Split
	{
		None = 0,
		Parent = 1,
		Child = 2
	}

	public enum WHSTransferStatus
	{
		New = 0,
		Downloaded = 1,
		Processing = 2,
		Outstanding = 7,
		Completed = 8,
		Cancelled = 10,
		All = 11,
	}

	public enum InboundType
	{
		ManualEntry = 0,
		ASN = 1,
		Return = 2,
		Excess = 3,
		ScannerManual = 4,
	}
	public enum SAPVendorType
	{
		Undefined = 0,
		Consignment = 1,
		VMI = 2,
		OwnStock = 3
	}
	public enum CycleCountStatus
	{
		New = 0,
		Download = 1,
		Counting = 2,
		Outstanding = 7,
		Completed = 8,
		Cancelled = 10,
		All = 11
	}

	public enum CycleCountDetailStatus
	{
		New = 0,
		Counted = 1
	}
	public enum QuarantineType
	{
		Onhold = 0,
		Release = 1,
	}
	public enum StockTransferStatus
	{
		New = 0,
		Processing = 1,
		Outstanding = 7,
		Completed = 8,
		Cancelled = 10,
		All = 11
	}
	public enum StockTransferType
    {
		Over90Days = 0,
		Damaged = 1,
		EStockTransfer = 2
	}

	public enum EStockTransferStatus
	{
		New = 0,
		Imported,
		InTransit,
		//DataSent - For SAP special requirement of Truck loading, this status is use to indicate
		//an EKanban is added into a Loading job.
		DataSent,
		Completed
	}
}
