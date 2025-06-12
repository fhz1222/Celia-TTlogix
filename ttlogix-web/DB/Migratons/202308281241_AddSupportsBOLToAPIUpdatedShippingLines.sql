IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[APIUpdatedShippingLines]') AND name = 'SupportsBOL')
BEGIN
    ALTER TABLE dbo.APIUpdatedShippingLines ADD SupportsBOL BIT NOT NULL
    CONSTRAINT DF_APIUpdatedShippingLines_SupportsBOL DEFAULT 0
END