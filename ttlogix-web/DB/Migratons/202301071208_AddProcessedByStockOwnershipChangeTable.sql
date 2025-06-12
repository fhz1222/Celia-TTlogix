IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProcessedByStockOwnershipChange' and xtype='U')
	CREATE TABLE [dbi].[ProcessedByStockOwnershipChange](
		[TransferJobNo] [nvarchar](15) NOT NULL,
	 CONSTRAINT [PK_ProcessedByStockOwnershipChange] PRIMARY KEY CLUSTERED 
	(
		[TransferJobNo] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO