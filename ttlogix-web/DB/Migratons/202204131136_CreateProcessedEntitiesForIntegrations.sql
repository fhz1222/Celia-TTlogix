CREATE TABLE [dbi].[ProcessedEntities](
	[Interface] [varchar](30) NOT NULL,
	[EntityID] [varchar](100) NOT NULL,
	[OutgoingID] [varchar](100) NULL,
	[MessageFilename] [varchar](200) NULL,
	[IsComplete] bit NOT NULL DEFAULT(0)
) ON [PRIMARY]
GO
