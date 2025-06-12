IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DELJITDetails' and xtype='U')
	CREATE TABLE DELJITDetails (
		EDIID varchar(14) NOT NULL,
		FactoryID varchar(35) NOT NULL,
		LocationID varchar(25) NULL,
		DepartureLocation varchar(25) NULL,
		LineItem int NOT NULL,
		ProductCode varchar(35) NOT NULL,
		ProductDescription varchar(35) NULL,
		BlanketOrderNo varchar(35) NOT NULL,
		BlanketOrderDate datetime NULL,
		ExternalLineItem int NULL,
		PRIMARY KEY (EDIID, LineItem)
	)
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DELJITHeader' and xtype='U')
	CREATE TABLE DELJITHeader (
		EDIID varchar(14) NOT NULL,
		EDISender varchar(35) NOT NULL,
		EDIRecipient varchar(35) NOT NULL,
		EDIDate datetime NOT NULL,
		MessageType varchar(6) NOT NULL,
		MessageTypeVersion varchar(3) NOT NULL,
		MessageTypeRelease varchar(3) NOT NULL,
		DocumentNo varchar(35) NOT NULL,
		MessageFunction varchar(35) NULL,
		DocumentDate datetime NOT NULL,
		SellerQualifier varchar(3) NOT NULL,
		SellerID varchar(17) NOT NULL,
		BuyerID varchar(17) NOT NULL,
		[Status] tinyint NULL,
		[FileName] varchar(50) NOT NULL,
		PRIMARY KEY (EDIID)
	)
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DELJITQty' and xtype='U')
	CREATE TABLE DELJITQty (
		EDIID varchar(14) NOT NULL,
		LineItem int NOT NULL,
		SerialNo int NOT NULL,
		QtyQualifier varchar(3) NOT NULL,
		Qty decimal(18,6) NOT NULL,
		UOM varchar(3) NULL,
		DeliveryPlanIndicator varchar(3) NULL,
		DeliveryRequirement varchar(3) NULL,
		DateQualifier varchar(3) NULL,
		StartDate datetime NULL,
		CallOffNo varchar(35) NULL,
		PRIMARY KEY (EDIID, LineItem, SerialNo)
	)
GO