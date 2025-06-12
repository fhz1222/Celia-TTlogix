IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='YusenCustomsIntegration' and xtype='U')
CREATE TABLE [dbi].[YusenCustomsIntegration](
	[JobNo] [varchar](15) NOT NULL UNIQUE,
	[CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
	[SentDate] [datetime] NULL
);