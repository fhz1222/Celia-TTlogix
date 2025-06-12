CREATE TABLE [dbi].[iLogIntegrationStatus](
	[IsEnabled] [bit] NOT NULL
) ON [PRIMARY]
GO
INSERT INTO [dbi].[iLogIntegrationStatus] VALUES (0)
GO
CREATE TABLE [dbi].[iLogIntegrationWarehouse](
	[WHSCode] [varchar](7) NOT NULL
) ON [PRIMARY]
GO