IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNDetailLog]') AND name = 'IsSAPSchedulingAgreement')
BEGIN
    ALTER TABLE dbo.ASNDetailLog ADD IsSAPSchedulingAgreement BIT NOT NULL
    CONSTRAINT DF_ASNDetailLog_IsSAPSchedulingAgreement DEFAULT 0
END
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNDetailLog]') AND name = 'PONo')
BEGIN
    ALTER TABLE dbo.ASNDetailLog ADD PONo VARCHAR(10) NULL
    CONSTRAINT DF_ASNDetailLog_PONo DEFAULT NULL
END
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNDetailLog]') AND name = 'POLineNo')
BEGIN
    ALTER TABLE dbo.ASNDetailLog ADD POLineNo VARCHAR(6) NULL
    CONSTRAINT DF_ASNDetailLog_POLineNo DEFAULT NULL
END