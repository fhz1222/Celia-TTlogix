IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProcessedByDGTProductionDelivery' and xtype='U')
	CREATE TABLE [dbi].[ProcessedByDGTProductionDelivery](
	[OutboundJobNo] [nvarchar](15) NOT NULL
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProcessedByDGTReturn' and xtype='U')
	CREATE TABLE [dbi].[ProcessedByDGTReturn](
	[InboundJobNo] [nvarchar](15) NOT NULL
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProcessedByDGTWarehouseTransfer' and xtype='U')
	CREATE TABLE [dbi].[ProcessedByDGTWarehouseTransfer](
	[TransferJobNo] [nvarchar](15) NOT NULL
) ON [PRIMARY]
GO