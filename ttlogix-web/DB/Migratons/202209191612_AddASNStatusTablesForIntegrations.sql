CREATE TABLE [dbi].[ASNHeaderStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ASNNo] [nvarchar](25) NOT NULL,
	[SupplierID] [nvarchar](10) NOT NULL,
	[FactoryID] [nvarchar](7) NOT NULL,
	[ContainerNo] [nvarchar](20) NULL,
	[BOL] [nvarchar](25) NULL,
	[ETA] [datetime] NOT NULL,
	[GRDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbi].[ASNHeaderStatus] ADD  CONSTRAINT [DF_ASNHeaderStatus_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

CREATE TABLE [dbi].[ASNDetailStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ASNHeaderStatusID] [int] NOT NULL,
	[LineItem] [int] NOT NULL,
	[ProductCode] [nvarchar](30) NOT NULL,
	[TotalQty] [int] NOT NULL,
	[PONo] [nvarchar](10) NULL,
	[POLineNo] [nvarchar](6) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO