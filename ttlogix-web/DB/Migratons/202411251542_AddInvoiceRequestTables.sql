IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestBlocklist' and xtype='U')
-- Record created on button click
-- Record removed on button click
CREATE TABLE [dbo].[InvoiceRequestBlocklist](
	[JobNo] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
	[CreatedBy] [nvarchar](10) NOT NULL,
	PRIMARY KEY (JobNo)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceBatch' and xtype='U')
-- Record created on invoice batch upload
CREATE TABLE [dbo].[InvoiceBatch](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[BatchNumber] [varchar](256) NOT NULL,
	[FactoryID] [varchar](7) NOT NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
	[CreatedBy] [nvarchar](10) NOT NULL,
	[ApprovedDate] [datetime] NULL,
	[ApprovedBy] [nvarchar](10) NULL,
	[RejectedDate] [datetime] NULL,
	[RejectedBy] [nvarchar](10) NULL,
	PRIMARY KEY (ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequest' and xtype='U')
-- Record created on invoice request issuing
CREATE TABLE [dbo].[InvoiceRequest](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FactoryID] [varchar](7) NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[JobNo] [varchar](15) NOT NULL, -- TODO to consider index
	[SupplierRefNo] [varchar](30) NOT NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
	[CreatedBy] [nvarchar](10) NOT NULL,
	[ApprovedBatchID] [int] NULL,
	PRIMARY KEY (ID),
	FOREIGN KEY (ApprovedBatchID) REFERENCES InvoiceBatch(ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestProduct' and xtype='U')
-- Record created on invoice request issuing
CREATE TABLE [dbo].[InvoiceRequestProduct](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceRequestID] [int] NOT NULL, -- TODO to consider index
	[ProductCode] [varchar](30) NOT NULL,
	[Qty] [int] NOT NULL,
	[InboundJob] [varchar](15) NOT NULL,
	[ASNNo] [varchar](25) NULL,
	[PONumber] [varchar](10) NULL,
	[POLineNo] [varchar](6) NULL,
	PRIMARY KEY (ID),
	FOREIGN KEY (InvoiceRequestID) REFERENCES InvoiceRequest(ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceBatchRequestLink' and xtype='U')
-- Record created on invoice batch upload
CREATE TABLE [dbo].[InvoiceBatchRequestLink](
	[InvoiceRequestID] [int] NOT NULL,
	[InvoiceBatchID] [int] NOT NULL,
	FOREIGN KEY (InvoiceRequestID) REFERENCES InvoiceRequest(ID),
	FOREIGN KEY (InvoiceBatchID) REFERENCES InvoiceBatch(ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoicePriceValidation' and xtype='U')
-- Record created on invoice batch price validation
-- Record removed on invoice batch upload
CREATE TABLE [dbo].[InvoicePriceValidation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Currency] [varchar](7) NOT NULL,
	[InvoiceTotalValue] [decimal](18,2) NOT NULL,
	[TTLogixTotalValue] [decimal](18,2) NOT NULL,
	[Success] bit NOT NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
	[CreatedBy] [nvarchar](10) NOT NULL,
	PRIMARY KEY (ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoicePriceValidationRequest' and xtype='U')
-- Record created on invoice batch price validation
-- Record removed on invoice batch upload
CREATE TABLE [dbo].[InvoicePriceValidationRequest](
	[InvoicePriceValidationID] [int] NOT NULL,
	[InvoiceRequestID] [int] NOT NULL,
	FOREIGN KEY (InvoicePriceValidationID) REFERENCES InvoicePriceValidation(ID),
	FOREIGN KEY (InvoiceRequestID) REFERENCES InvoiceRequest(ID)
);

IF NOT EXISTS (SELECT * FROM syscolumns WHERE ID=OBJECT_ID('[dbo].[SupplierDetail]') AND NAME='InvoiceBatchSequenceNo')
ALTER TABLE [dbo].[SupplierDetail] ADD [InvoiceBatchSequenceNo] INT NOT NULL DEFAULT(1);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceBatchCustomsAgency' and xtype='U')
-- Record created on invoice batch approval (customs clearance flow)
CREATE TABLE [dbo].[InvoiceBatchCustomsAgency](
	[InvoiceBatchID] [int] NOT NULL,
	[TruckDepartureTime] [int] NOT NULL,
	[Comment] [nvarchar](256) NULL,
	PRIMARY KEY (InvoiceBatchID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Invoice' and xtype='U')
-- Record created on invoice batch upload
CREATE TABLE [dbo].[Invoice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceBatchID] [int] NOT NULL,
	[InvoiceNumber] [nvarchar](256) NOT NULL,
	[Value] [decimal](18,2) NOT NULL,
	[Currency] [nvarchar](7) NOT NULL,
	PRIMARY KEY (ID),
	FOREIGN KEY (InvoiceBatchID) REFERENCES InvoiceBatch(ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceFile' and xtype='U')
-- Record created on invoice batch upload
-- Record removed on invoice batch rejection
CREATE TABLE [dbo].[InvoiceFile](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NOT NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	PRIMARY KEY (ID),
	FOREIGN KEY (InvoiceID) REFERENCES Invoice(ID)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestParameters' and xtype='U')
CREATE TABLE [dbo].[InvoiceRequestParameters](
	[StandardFlow] [bit] NOT NULL DEFAULT(0),
	[CustomsClearanceFlow] [bit] NOT NULL DEFAULT(0),
	[NoPriceValidation] [bit] NOT NULL DEFAULT (0),
	[RelevancyThreshold] [int] NOT NULL DEFAULT(7),
);

IF NOT EXISTS (SELECT * FROM syscolumns WHERE ID=OBJECT_ID('[dbo].[TT_StockTransferDetail]') AND NAME='Qty')
ALTER TABLE [dbo].[TT_StockTransferDetail] ADD [Qty] [decimal](18,6) NULL;

IF NOT EXISTS (SELECT * FROM Module WHERE ModuleName='Invoicing')
BEGIN
	INSERT INTO Module (Code, ParentCode, ModuleName, NavigateUrl, Target)
	VALUES ((SELECT MAX(Code) + 1 FROM Module), '', 'Invoicing', 'web/#/invoicing', 'ContentFrame');
END