IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[FactoryMaster]') AND name = 'HasActiveIntegration'
)
BEGIN
    ALTER TABLE [dbo].FactoryMaster ADD HasActiveIntegration bit NOT NULL
    CONSTRAINT DF_FactoryMaster_HasActiveIntegration DEFAULT 0
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[FactoryMaster]') AND name = 'HasSAP'
)
BEGIN
    ALTER TABLE [dbo].FactoryMaster ADD HasSAP bit NOT NULL
    CONSTRAINT DF_FactoryMaster_HasSAP DEFAULT 0
END
GO