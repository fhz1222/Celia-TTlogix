CREATE TABLE [dbi].[iLogIntegrationBox](
	[BoxId]  AS (concat('BID',right('0000000000'+CONVERT([nvarchar],[Id],0),(10)))),
	[MasterPalletId] [varchar](20) NOT NULL,
	[Qty] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_iLogIntegrationBoxes] PRIMARY KEY CLUSTERED 
(
	[BoxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO