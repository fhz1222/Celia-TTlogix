IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_WHSTransferDetail]') AND name = 'Ownership')
BEGIN
    ALTER TABLE TT_WHSTransferDetail ADD Ownership BIT NULL
    CONSTRAINT DF_TT_WHSTransferDetail_Ownership DEFAULT NULL
END
GO