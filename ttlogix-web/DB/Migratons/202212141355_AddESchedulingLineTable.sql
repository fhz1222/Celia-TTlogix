IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ESchedulingLine' and xtype='U')
	CREATE TABLE [dbo].[ESchedulingLine](
		[SchedulingAgreementNo] [nvarchar](35) NOT NULL,
		[SchedulingLineNo] [int] NOT NULL,
		[FactoryID] [nvarchar](7) NOT NULL,
		[SupplierID] [nvarchar](10) NOT NULL,
		[ProductCode] [nvarchar](30) NOT NULL,
		[TotalQty] [decimal](18, 0) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[LastEDI] [nvarchar](14) NOT NULL,
		[SAPLocationID] [nvarchar](25) NOT NULL,
	 CONSTRAINT [PK_ESchedulingLine] PRIMARY KEY CLUSTERED 
	(
		[SchedulingAgreementNo] ASC,
		[SchedulingLineNo] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO