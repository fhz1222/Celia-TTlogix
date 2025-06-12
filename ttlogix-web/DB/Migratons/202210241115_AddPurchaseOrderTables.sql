IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EPO' and xtype='U')
	CREATE TABLE [dbo].[EPO](
		[PONo] [varchar](35) NOT NULL,
		[POLineItem] [varchar](5) NOT NULL,
		[SupplierID] [varchar](17) NOT NULL,
		[FactoryID] [varchar](7) NOT NULL,
		[ProductCode] [varchar](35) NOT NULL,
		[Qty] [decimal](18, 2) NOT NULL,
		[Status] [tinyint] NULL,
		[Revision] [int] NULL,
		[RevisedDate] [datetime] NULL,
		[LastUpdateEDI] [varchar](14) NULL,
		[TolerancePercentage] [decimal](18, 3) NULL,
		[UnlimitedFlag] [tinyint] NULL,
		[SAPLocationID] [varchar](25) NULL,
	 CONSTRAINT [PK_EPO] PRIMARY KEY CLUSTERED 
	(
		[PONo] ASC,
		[POLineItem] ASC,
		[SupplierID] ASC,
		[FactoryID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DORDERSHeader' and xtype='U')
	CREATE TABLE [dbo].[DORDERSHeader](
		[EDIID] [varchar](14) NOT NULL,
		[EDISender] [varchar](35) NOT NULL,
		[EDIRecipient] [varchar](35) NOT NULL,
		[EDIDate] [datetime] NOT NULL,
		[MessageType] [varchar](6) NOT NULL,
		[MessageTypeVersion] [varchar](3) NOT NULL,
		[MessageTypeRelease] [varchar](3) NOT NULL,
		[DocumentNo] [varchar](35) NOT NULL,
		[MessageFunction] [varchar](3) NULL,
		[DocumentDate] [datetime] NOT NULL,
		[SellerQualifier] [varchar](3) NOT NULL,
		[SellerID] [varchar](17) NOT NULL,
		[FactoryID] [varchar](7) NOT NULL,
		[PONo] [varchar](35) NOT NULL,
		[Status] [tinyint] NULL,
		[FileName] [varchar](250) NOT NULL,
	 CONSTRAINT [PK_DORDERSHeader] PRIMARY KEY CLUSTERED 
	(
		[EDIID] ASC,
		[MessageType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DORDERSDetails' and xtype='U')
	CREATE TABLE [dbo].[DORDERSDetails](
		[EDIID] [varchar](14) NOT NULL,
		[MessageType] [varchar](6) NOT NULL,
		[LineItem] [int] NOT NULL,
		[ExternalLineItem] [varchar](5) NOT NULL,
		[ProductCode] [varchar](35) NOT NULL,
		[Qty] [decimal](18, 2) NOT NULL,
		[Action] [int] NULL,
		[LocationID] [varchar](25) NULL,
		[TolerancePercentage] [decimal](18, 3) NULL,
		[UnlimitedFlag] [tinyint] NULL,
	 CONSTRAINT [PK_DORDERSDetails] PRIMARY KEY CLUSTERED 
	(
		[EDIID] ASC,
		[MessageType] ASC,
		[LineItem] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO