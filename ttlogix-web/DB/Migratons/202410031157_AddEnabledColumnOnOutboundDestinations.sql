IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_OutboundDestinations]') AND name = 'Enabled')
BEGIN
    ALTER TABLE dbo.TT_OutboundDestinations ADD Enabled BIT NOT NULL
    CONSTRAINT DF_TT_OutboundDestinations_Enabled DEFAULT 1
END