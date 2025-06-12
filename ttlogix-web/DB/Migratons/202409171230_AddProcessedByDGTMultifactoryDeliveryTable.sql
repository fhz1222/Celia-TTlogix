IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProcessedByDGTMultifactoryDelivery' and xtype='U')
	CREATE TABLE [dbi].[ProcessedByDGTMultifactoryDelivery](
		[OutboundJobNo] [nvarchar](15) NOT NULL,
	 CONSTRAINT [PK_ProcessedByDGTMultifactoryDelivery] PRIMARY KEY CLUSTERED 
	(
		[OutboundJobNo] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO