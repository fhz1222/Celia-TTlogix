IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNDetail]') AND name = 'IsSAPSchedulingAgreement')
BEGIN
    ALTER TABLE dbo.ASNDetail ADD IsSAPSchedulingAgreement BIT NOT NULL
    CONSTRAINT DF_ASNDetail_IsSAPSchedulingAgreement DEFAULT 0
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbi].[ASNDetailStatus]') AND name = 'IsSAPSchedulingAgreement')
BEGIN
    ALTER TABLE dbi.ASNDetailStatus ADD IsSAPSchedulingAgreement BIT NOT NULL
    CONSTRAINT DF_ASNDetailStatus_IsSAPSchedulingAgreement DEFAULT 0
END